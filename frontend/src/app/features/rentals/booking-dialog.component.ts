import { CurrencyPipe } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Car } from '../../core/models/car.model';
import { RentalsService } from '../../core/services/rentals.service';

@Component({
  selector: 'app-booking-dialog',
  standalone: true,
  imports: [CurrencyPipe, FormsModule],
  templateUrl: './booking-dialog.component.html',
})
export class BookingDialogComponent {
  constructor(private readonly rentalsService: RentalsService) {}

  readonly car = input.required<Car>();
  readonly closed = output<void>();
  readonly booked = output<void>();
  readonly form = {
    phone: '',
    startDate: new Date().toISOString().slice(0, 10),
    endDate: '',
  };

  submit(): void {
    if (!this.form.phone || !this.form.endDate) {
      return;
    }

    this.rentalsService.create({ carId: this.car().id, ...this.form }).subscribe({
      next: () => this.booked.emit(),
    });
  }
}
