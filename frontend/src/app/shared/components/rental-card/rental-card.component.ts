import { CurrencyPipe } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { Rental } from '../../../core/models/rental.model';

@Component({
  selector: 'app-rental-card',
  standalone: true,
  imports: [CurrencyPipe],
  templateUrl: './rental-card.component.html',
})
export class RentalCardComponent {
  readonly rental = input.required<Rental>();
  readonly showCustomer = input(false);
  readonly cancel = output<Rental>();
}
