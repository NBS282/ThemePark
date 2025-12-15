import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../../../core/models/user.model';
import { UserProfileUpdate } from '../models/user-profile-update.model';
import { environment } from '../../../../environments/environment';
import { StorageService } from '../../../core/services/storage.service';
import { AuthService } from '../../../core/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  isLoading = signal(false);
  error = signal<string | null>(null);
  updatedUser = signal<User | null>(null);

  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private storageService: StorageService
  ) {
    const authService = inject(AuthService);
    authService.registerCleanup(() => this.cleanup());
  }

  getCurrentProfile(userId: string): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.http.get<User>(`${this.apiUrl}/users/${userId}`)
      .subscribe({
        next: (user) => {
          this.storageService.saveUser(user);
          this.updatedUser.set(user);
          this.isLoading.set(false);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Error al cargar perfil');
          this.isLoading.set(false);
        }
      });
  }

  updateProfile(codigoIdentificacion: string, userData: UserProfileUpdate, currentUser: User): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.http.put<Partial<User>>(`${this.apiUrl}/users/${codigoIdentificacion}`, userData)
      .subscribe({
        next: (partialUser) => {
          const completeUser: User = {
            ...partialUser,
            id: partialUser.id || currentUser.id,
            codigoIdentificacion: currentUser.codigoIdentificacion,
            roles: currentUser.roles,
            nombre: partialUser.nombre || currentUser.nombre,
            apellido: partialUser.apellido || currentUser.apellido,
            email: partialUser.email || currentUser.email,
            fechaNacimiento: partialUser.fechaNacimiento || currentUser.fechaNacimiento,
            nivelMembresia: partialUser.nivelMembresia || currentUser.nivelMembresia,
            fechaRegistro: partialUser.fechaRegistro || currentUser.fechaRegistro
          };

          this.storageService.saveUser(completeUser);
          this.updatedUser.set(completeUser);
          this.isLoading.set(false);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Error al actualizar perfil');
          this.isLoading.set(false);
        }
      });
  }

  private cleanup(): void {
    this.updatedUser.set(null);
    this.isLoading.set(false);
    this.error.set(null);
  }
}
