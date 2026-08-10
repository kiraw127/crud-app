import { Component, inject, signal } from '@angular/core';
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
  private readonly carsService = inject(CarsService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly cars = signal<Car[]>([]);
  readonly selectedCar = signal<Car | null>(null);
  search = '';

  constructor() {
    this.loadCars();
  }

  get availableCount(): number {
    return this.cars().filter((car) => car.isAvailable).length;
  }

  get filteredCars(): Car[] {
    const query = this.search.trim().toLowerCase();
    return this.cars().filter((car) =>
      `${car.brand} ${car.model} ${car.category}`.toLowerCase().includes(query),
    );
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
    this.carsService.getAll().subscribe((cars) => this.cars.set(cars));
  }
}
