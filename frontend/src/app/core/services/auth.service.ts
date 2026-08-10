import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { LoginRequest, RegisterRequest, Session } from '../models/auth.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private readonly storageKey = 'rentauto_session';

  readonly session = signal<Session | null>(this.readSession());
  readonly isAdmin = computed(() => this.session()?.role === 'Admin');

  login(request: LoginRequest): Observable<Session> {
    return this.http
      .post<Session>(`${this.apiUrl}/login`, request)
      .pipe(tap((session) => this.saveSession(session)));
  }

  register(request: RegisterRequest): Observable<Session> {
    return this.http
      .post<Session>(`${this.apiUrl}/register`, request)
      .pipe(tap((session) => this.saveSession(session)));
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    this.session.set(null);
  }

  private saveSession(session: Session): void {
    localStorage.setItem(this.storageKey, JSON.stringify(session));
    this.session.set(session);
  }

  private readSession(): Session | null {
    try {
      return JSON.parse(localStorage.getItem(this.storageKey) ?? 'null');
    } catch {
      return null;
    }
  }
}
