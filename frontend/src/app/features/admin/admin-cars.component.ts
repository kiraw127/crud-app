import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Car, SaveCarRequest, createEmptyCar } from '../../core/models/car.model';
import { CarsService } from '../../core/services/cars.service';
import { CarCardComponent } from '../../shared/components/car-card/car-card.component';
import { HeroComponent } from '../../shared/components/hero/hero.component';

@Component({
  selector: 'app-admin-cars',
  standalone: true,
  imports: [FormsModule, CarCardComponent, HeroComponent],
  templateUrl: './admin-cars.component.html',
})
export class AdminCarsComponent {
  constructor(private readonly carsService: CarsService) {
    this.loadCars();
  }

  readonly cars = signal<Car[]>([]);
  readonly editingCar = signal<Car | null>(null);
  search = '';

  get availableCount(): number {
    return this.cars().filter((car) => car.isAvailable).length;
  }

  updateSearch(search: string): void {
    this.search = search;
    this.loadCars();
  }

  createCar(): void {
    this.editingCar.set(createEmptyCar());
  }

  editCar(car: Car): void {
    this.editingCar.set({ ...car });
  }

  saveCar(): void {
    const car = this.editingCar();
    if (!car) return;

    const request = this.toSaveRequest(car);
    const onSaved = () => {
      this.editingCar.set(null);
      this.loadCars();
    };

    if (car.id) {
      this.carsService.update(car.id, request).subscribe(onSaved);
    } else {
      this.carsService.create(request).subscribe(onSaved);
    }
  }

  deleteCar(car: Car): void {
    if (!confirm(`Удалить ${car.brand} ${car.model}?`)) return;
    this.carsService.delete(car.id).subscribe(() => this.loadCars());
  }

  private loadCars(): void {
    this.carsService.getAll(this.search).subscribe((cars) => this.cars.set(cars));
  }

  private toSaveRequest(car: Car): SaveCarRequest {
    const { id: _id, isAvailable: _isAvailable, ...request } = car;
    return request;
  }
}
