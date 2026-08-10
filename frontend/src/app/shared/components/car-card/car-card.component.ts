import { CurrencyPipe } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { Car } from '../../../core/models/car.model';

@Component({
  selector: 'app-car-card',
  standalone: true,
  imports: [CurrencyPipe],
  templateUrl: './car-card.component.html',
})
export class CarCardComponent {
  readonly car = input.required<Car>();
  readonly adminMode = input(false);
  readonly edit = output<Car>();
  readonly remove = output<Car>();
  readonly rent = output<Car>();
}
