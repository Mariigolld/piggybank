import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models';

const API = 'http://localhost:5235/api';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  login(body: LoginRequest) {
    return this.http.post<AuthResponse>(`${API}/auth/login`, body);
  }

  register(body: RegisterRequest) {
    return this.http.post<AuthResponse>(`${API}/auth/register`, body);
  }

  saveSession(data: AuthResponse) {
    localStorage.setItem('token', data.token);
    localStorage.setItem('user', JSON.stringify({
      email: data.email,
      firstName: data.firstName,
      lastName: data.lastName
    }));
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.router.navigate(['/login']);
  }

  getUser(): { email: string; firstName: string; lastName: string } | null {
    const raw = localStorage.getItem('user');
    return raw ? JSON.parse(raw) : null;
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }
}
