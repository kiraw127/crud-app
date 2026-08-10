import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateRentalRequest, Rental } from '../models/rental.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RentalsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/rentals`;

  getAll(): Observable<Rental[]> {
    return this.http.get<Rental[]>(this.apiUrl);
  }

  getMine(): Observable<Rental[]> {
    return this.http.get<Rental[]>(`${this.apiUrl}/me`);
  }

  create(request: CreateRentalRequest): Observable<Rental> {
    return this.http.post<Rental>(this.apiUrl, request);
  }

  cancelMine(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/me/${id}`);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
