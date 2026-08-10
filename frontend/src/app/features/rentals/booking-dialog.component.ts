import { CurrencyPipe } from '@angular/common';
import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Car } from '../../core/models/car.model';
import { RentalsService } from '../../core/services/rentals.service';
import { getErrorMessage } from '../../core/utils/http-error';

@Component({
  selector: 'app-booking-dialog',
  standalone: true,
  imports: [CurrencyPipe, FormsModule],
  templateUrl: './booking-dialog.component.html',
})
export class BookingDialogComponent {
  private readonly rentalsService = inject(RentalsService);

  readonly car = input.required<Car>();
  readonly closed = output<void>();
  readonly booked = output<void>();
  readonly form = {
    phone: '',
    startDate: new Date().toISOString().slice(0, 10),
    endDate: '',
  };
  error = '';

  submit(): void {
    if (!this.form.phone || !this.form.endDate) {
      this.error = 'Заполните телефон и дату окончания.';
      return;
    }

    this.rentalsService.create({ carId: this.car().id, ...this.form }).subscribe({
      next: () => this.booked.emit(),
      error: (error) => {
        this.error = getErrorMessage(error, 'Не удалось оформить аренду.');
      },
    });
  }
}
