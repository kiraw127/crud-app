import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { getErrorMessage } from '../../core/utils/http-error';

@Component({
  selector: 'app-auth-page',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './auth-page.component.html',
})
export class AuthPageComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  mode: 'login' | 'register' = 'login';
  form = { name: '', email: '', password: '' };
  error = '';

  submit(): void {
    this.error = '';
    const request =
      this.mode === 'login'
        ? this.authService.login({ email: this.form.email, password: this.form.password })
        : this.authService.register(this.form);

    request.subscribe({
      next: (session) => {
        const destination = session.role === 'Admin' ? '/admin/cars' : '/cars';
        void this.router.navigate([destination]);
      },
      error: (error) => {
        this.error = getErrorMessage(error, 'Не удалось выполнить вход.');
      },
    });
  }

  toggleMode(): void {
    this.mode = this.mode === 'login' ? 'register' : 'login';
    this.error = '';
  }
}
