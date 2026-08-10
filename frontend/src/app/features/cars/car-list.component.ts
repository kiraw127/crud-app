import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Car } from '../../core/models/car.model';
import { AuthService } from '../../core/services/auth.service';
import { CarsService } from '../../core/services/cars.service';
import { CarCardComponent } from '../../shared/components/car-card/car-card.component';
import { HeroComponent } from '../../shared/components/hero/hero.component';
import { BookingDialogComponent } from '../rentals/booking-dialog.component';

@Component({
  selector: 'app-car-list',
  standalone: true,
  imports: [FormsModule, CarCardComponent, HeroComponent, BookingDialogComponent],
  templateUrl: './car-list.component.html',
})
export class CarListComponent {
  constructor(
    private readonly carsService: CarsService,
    private readonly authService: AuthService,
    private readonly router: Router,
  ) {
    this.loadCars();
  }

  readonly cars = signal<Car[]>([]);
  readonly selectedCar = signal<Car | null>(null);
  search = '';

  get availableCount(): number {
    return this.cars().filter((car) => car.isAvailable).length;
  }

  updateSearch(search: string): void {
    this.search = search;
    this.loadCars();
  }

  startBooking(car: Car): void {
    if (!this.authService.session()) {
      void this.router.navigate(['/login']);
      return;
    }
    this.selectedCar.set(car);
  }

  completeBooking(): void {
    this.selectedCar.set(null);
    void this.router.navigate(['/my-rentals']);
  }

  private loadCars(): void {
    this.carsService.getAll(this.search).subscribe((cars) => this.cars.set(cars));
  }
}
