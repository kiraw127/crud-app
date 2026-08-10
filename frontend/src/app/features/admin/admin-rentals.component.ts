import { Component, inject, signal } from '@angular/core';
import { Rental } from '../../core/models/rental.model';
import { RentalsService } from '../../core/services/rentals.service';
import { RentalCardComponent } from '../../shared/components/rental-card/rental-card.component';

@Component({
  selector: 'app-admin-rentals',
  standalone: true,
  imports: [RentalCardComponent],
  templateUrl: './admin-rentals.component.html',
})
export class AdminRentalsComponent {
  private readonly rentalsService = inject(RentalsService);
  readonly rentals = signal<Rental[]>([]);

  constructor() {
    this.loadRentals();
  }

  finishRental(rental: Rental): void {
    if (!confirm('Завершить и удалить аренду?')) return;
    this.rentalsService.delete(rental.id).subscribe(() => this.loadRentals());
  }

  private loadRentals(): void {
    this.rentalsService.getAll().subscribe((rentals) => this.rentals.set(rentals));
  }
}
