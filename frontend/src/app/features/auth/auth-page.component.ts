import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-auth-page',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './auth-page.component.html',
})
export class AuthPageComponent {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
  ) {}

  mode: 'login' | 'register' = 'login';
  form = { name: '', email: '', password: '' };

  submit(): void {
    const request =
      this.mode === 'login'
        ? this.authService.login({ email: this.form.email, password: this.form.password })
        : this.authService.register(this.form);

    request.subscribe({
      next: (session) => {
        const destination = session.role === 'Admin' ? '/admin/cars' : '/cars';
        void this.router.navigate([destination]);
      },
    });
  }

  toggleMode(): void {
    this.mode = this.mode === 'login' ? 'register' : 'login';
  }
}
