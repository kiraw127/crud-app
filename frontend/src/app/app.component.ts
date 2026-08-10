import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface Car { id:number; brand:string; model:string; year:number; category:string; dailyRate:number; transmission:string; seats:number; imageUrl:string; isAvailable:boolean; description?:string }
interface Rental { id:number; carId:number; car?:Car; customerName:string; phone:string; startDate:string; endDate:string; totalPrice:number; status:string }
interface Session { token:string; name:string; role:string }
const emptyCar=():Car=>({id:0,brand:'',model:'',year:new Date().getFullYear(),category:'Комфорт',dailyRate:20000,transmission:'Автомат',seats:5,imageUrl:'https://images.unsplash.com/photo-1492144534655-ae79c964c9d7?auto=format&fit=crop&w=1200&q=80',isAvailable:true,description:''});

@Component({selector:'app-root',standalone:true,imports:[CommonModule,FormsModule],templateUrl:'./app.component.html'})
export class AppComponent {
  private http=inject(HttpClient); readonly api='http://localhost:5148/api';
  cars=signal<Car[]>([]); rentals=signal<Rental[]>([]); myRentals=signal<Rental[]>([]); session=signal<Session|null>(this.readSession());
  tab=signal<'cars'|'rentals'|'mine'>('cars'); modal=signal(''); editing=signal<Car>(emptyCar()); selectedCar=signal<Car|null>(null);
  search=''; authMode:'login'|'register'='login'; auth={name:'',email:'',password:''}; authError='';
  newRental={customerName:'',phone:'',startDate:'',endDate:'',status:'Активна'};

  constructor(){this.loadCars();if(this.isAdmin())this.loadRentals();else if(this.session())this.loadMyRentals()}
  isAdmin(){return this.session()?.role==='Admin'}
  headers(){return new HttpHeaders({Authorization:`Bearer ${this.session()?.token??''}`})}
  readSession():Session|null{try{return JSON.parse(localStorage.getItem('rentauto_session')||'null')}catch{return null}}
  loadCars(){this.http.get<Car[]>(`${this.api}/cars`).subscribe(x=>this.cars.set(x))}
  loadRentals(){if(this.isAdmin())this.http.get<Rental[]>(`${this.api}/rentals`,{headers:this.headers()}).subscribe(x=>this.rentals.set(x))}
  loadMyRentals(){if(this.session()&&!this.isAdmin())this.http.get<Rental[]>(`${this.api}/rentals/me`,{headers:this.headers()}).subscribe(x=>this.myRentals.set(x))}
  availableCount(){return this.cars().filter(x=>x.isAvailable).length}
  filteredCars(){const q=this.search.toLowerCase();return this.cars().filter(c=>(`${c.brand} ${c.model} ${c.category}`).toLowerCase().includes(q))}

  openAuth(mode:'login'|'register'){this.authMode=mode;this.auth={name:'',email:'',password:''};this.authError='';this.modal.set('auth')}
  submitAuth(){
    this.authError=''; const path=this.authMode==='login'?'login':'register';
    this.http.post<Session>(`${this.api}/auth/${path}`,this.auth).subscribe({next:s=>{this.session.set(s);localStorage.setItem('rentauto_session',JSON.stringify(s));this.modal.set('');this.tab.set('cars');if(this.isAdmin())this.loadRentals();else this.loadMyRentals()},error:e=>this.authError=typeof e.error==='string'?e.error:'Не удалось выполнить вход. Проверьте данные.'});
  }
  logout(){localStorage.removeItem('rentauto_session');this.session.set(null);this.rentals.set([]);this.myRentals.set([]);this.tab.set('cars');this.modal.set('')}

  openCar(c?:Car){if(!this.isAdmin())return;this.editing.set(c?{...c}:emptyCar());this.modal.set('car')}
  saveCar(){const c=this.editing();const options={headers:this.headers()};const req=c.id?this.http.put(`${this.api}/cars/${c.id}`,c,options):this.http.post(`${this.api}/cars`,c,options);req.subscribe(()=>{this.modal.set('');this.loadCars()})}
  deleteCar(c:Car){if(this.isAdmin()&&confirm(`Удалить ${c.brand} ${c.model}?`))this.http.delete(`${this.api}/cars/${c.id}`,{headers:this.headers()}).subscribe(()=>this.loadCars())}
  openRental(c:Car){if(!this.session()){this.openAuth('login');return}if(this.isAdmin())return;this.selectedCar.set(c);this.newRental={customerName:this.session()!.name,phone:'',startDate:new Date().toISOString().slice(0,10),endDate:'',status:'Активна'};this.modal.set('rental')}
  saveRental(){const c=this.selectedCar();if(!c||!this.newRental.customerName||!this.newRental.phone||!this.newRental.endDate)return;this.http.post(`${this.api}/rentals`,{...this.newRental,carId:c.id},{headers:this.headers()}).subscribe(()=>{this.modal.set('');this.loadCars();this.loadMyRentals();this.tab.set('mine')})}
  deleteMyRental(r:Rental){if(confirm('Отменить аренду?'))this.http.delete(`${this.api}/rentals/me/${r.id}`,{headers:this.headers()}).subscribe(()=>{this.loadMyRentals();this.loadCars()})}
  deleteRental(r:Rental){if(this.isAdmin()&&confirm('Завершить и удалить аренду?'))this.http.delete(`${this.api}/rentals/${r.id}`,{headers:this.headers()}).subscribe(()=>{this.loadRentals();this.loadCars()})}
}
