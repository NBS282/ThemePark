import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user.model';
import { UserPoints } from '../models/user-points.model';
import { UserLogin } from '../../features/auth/models/user-login.model';
import { UserRegister } from '../../features/auth/models/user-register.model';
import { LoginResponse } from '../../features/auth/models/login-response.model';
import { StorageService } from './storage.service';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  currentUser = signal<User | null>(null);
  isLoading = signal(false);
  error = signal<string | null>(null);

  private apiUrl = environment.apiUrl;
  private cleanupCallbacks: (() => void)[] = [];
  private loginCallbacks: (() => void)[] = [];

  constructor(
    private http: HttpClient,
    private storageService: StorageService
  ) {}

  login(email: string, contraseña: string): void {
    this.isLoading.set(true);
    this.error.set(null);

    const credentials: UserLogin = { email, contraseña };

    this.http.post<LoginResponse>(`${this.apiUrl}/auth/login`, credentials)
      .subscribe({
        next: (response) => {
          this.storageService.saveToken(response.token);
          this.storageService.saveUser(response.usuario);
          this.currentUser.set(response.usuario);
          this.isLoading.set(false);

          this.loginCallbacks.forEach(callback => callback());
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Error al iniciar sesión');
          this.isLoading.set(false);
        }
      });
  }

  register(userData: UserRegister): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.http.post<User>(`${this.apiUrl}/users/register`, userData)
      .subscribe({
        next: (user) => {
          this.login(userData.email, userData.contraseña);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Error al registrarse');
          this.isLoading.set(false);
        }
      });
  }

  logout(): void {
    this.storageService.clearStorage();
    this.currentUser.set(null);
    this.error.set(null);

    this.cleanupCallbacks.forEach(callback => callback());
  }

  isAuthenticated(): boolean {
    return this.storageService.getToken() !== null;
  }

  loadCurrentUser(): void {
    const user = this.storageService.getUser();
    if (user) {
      this.currentUser.set(user);
      this.loginCallbacks.forEach(callback => callback());
    }
  }

  registerCleanup(callback: () => void): void {
    this.cleanupCallbacks.push(callback);
  }

  registerOnLogin(callback: () => void): void {
    this.loginCallbacks.push(callback);
  }

  refreshCurrentUser(): void {
    const user = this.currentUser();
    if (!user) return;

    this.http.get<User>(`${this.apiUrl}/users/${user.id}`).subscribe({
      next: (updatedUser) => {
        this.storageService.saveUser(updatedUser);
        this.currentUser.set(updatedUser);
      },
      error: () => {
      }
    });
  }

  getUserPoints(): Observable<UserPoints> {
    return this.http.get<UserPoints>(`${this.apiUrl}/users/points`);
  }
}
