import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private auth = inject(AuthService);

  email = '';
  password = '';
  loading = signal(false);
  error = signal('');

  constructor() {
    if (this.auth.isLoggedIn()) {
      window.location.href = '/dashboard';
    }
  }

  async submit() {
    if (!this.email || !this.password) return;
    this.loading.set(true);
    this.error.set('');
    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: data => {
        this.auth.saveSession(data);
        window.location.href = '/dashboard';
      },
      error: err => {
        this.error.set(err.error?.message || 'Invalid credentials');
        this.loading.set(false);
      }
    });
  }
}
