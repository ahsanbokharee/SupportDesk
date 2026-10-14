import { HttpClient } from '@angular/common/http';
import { Service, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';

export interface LoginResponse {
  token: string;
  expiresAt: string;
  email: string;
  fullName: string;
  role: string;
}

const STORAGE_KEY = 'supportdesk.auth';

@Service()
export class AuthService {
  private http = inject(HttpClient);

  private session = signal<LoginResponse | null>(this.loadSession());

  currentUser = this.session.asReadonly();
  isLoggedIn = computed(() => this.session() !== null);

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>('/api/auth/login', { email, password })
      .pipe(
        tap(response => {
          localStorage.setItem(STORAGE_KEY, JSON.stringify(response));
          this.session.set(response);
        })
      );
  }

  logout() {
    localStorage.removeItem(STORAGE_KEY);
    this.session.set(null);
  }

  getToken(): string | null {
    return this.session()?.token ?? null;
  }

  private loadSession(): LoginResponse | null {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (!stored) {
      return null;
    }

    const session = JSON.parse(stored) as LoginResponse;
    if (new Date(session.expiresAt) <= new Date()) {
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
    return session;
  }
}