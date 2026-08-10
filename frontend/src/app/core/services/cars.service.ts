import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Car, SaveCarRequest } from '../models/car.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CarsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/cars`;

  getAll(): Observable<Car[]> {
    return this.http.get<Car[]>(this.apiUrl);
  }

  create(request: SaveCarRequest): Observable<Car> {
    return this.http.post<Car>(this.apiUrl, request);
  }

  update(id: number, request: SaveCarRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
