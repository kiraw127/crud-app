import { Component, inject, signal } from '@angular/core';
import { Rental } from '../../core/models/rental.model';
import { RentalsService } from '../../core/services/rentals.service';
import { RentalCardComponent } from '../../shared/components/rental-card/rental-card.component';

@Component({
  selector: 'app-my-rentals',
  standalone: true,
  imports: [RentalCardComponent],
  templateUrl: './my-rentals.component.html',
})
export class MyRentalsComponent {
  private readonly rentalsService = inject(RentalsService);
  readonly rentals = signal<Rental[]>([]);

  constructor() {
    this.loadRentals();
  }

  cancel(rental: Rental): void {
    if (!confirm('Отменить аренду?')) return;
    this.rentalsService.cancelMine(rental.id).subscribe(() => this.loadRentals());
  }

  private loadRentals(): void {
    this.rentalsService.getMine().subscribe((rentals) => this.rentals.set(rentals));
  }
}
