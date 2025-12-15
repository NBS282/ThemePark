import { Injectable, signal, inject } from '@angular/core';
import { UserList } from '../models/user-list.model';
import { AuthService } from '../../../core/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserCacheService {
  private updatedUsers = signal<Map<string, UserList>>(new Map());

  constructor() {
    const authService = inject(AuthService);
    authService.registerCleanup(() => this.clearCache());
  }

  updateUser(user: UserList): void {
    const current = new Map(this.updatedUsers());
    current.set(user.id, user);
    this.updatedUsers.set(current);
  }

  getUpdatedUser(userId: string): UserList | undefined {
    return this.updatedUsers().get(userId);
  }

  clearCache(): void {
    this.updatedUsers.set(new Map());
  }
}
