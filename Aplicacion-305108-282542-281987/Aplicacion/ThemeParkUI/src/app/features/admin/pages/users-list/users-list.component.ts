import { Component, OnInit, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin.service';
import { UserList } from '../../models/user-list.model';
import { Subscription } from 'rxjs';
import { UserCacheService } from '../../services/user-cache.service';

@Component({
  selector: 'app-users-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './users-list.component.html',
  styleUrl: './users-list.component.css'
})
export class UsersListComponent implements OnInit, OnDestroy {
  users = signal<UserList[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  userToDelete = signal<string | null>(null);

  private subscriptions = new Subscription();

  constructor(
    private adminService: AdminService,
    private router: Router,
    private userCacheService: UserCacheService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadUsers(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.adminService.getAllUsers().subscribe({
      next: (users) => {
        const updatedUsers = users.map(user => {
          const cachedUser = this.userCacheService.getUpdatedUser(user.id);
          if (cachedUser) {
            return cachedUser;
          }
          return user;
        });

        this.users.set(updatedUsers);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar usuarios');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  confirmDelete(userId: string): void {
    this.userToDelete.set(userId);
  }

  cancelDelete(): void {
    this.userToDelete.set(null);
  }

  deleteUser(): void {
    const userId = this.userToDelete();
    if (!userId) return;

    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.adminService.deleteUser(userId).subscribe({
      next: () => {
        const currentUsers = this.users();
        this.users.set(currentUsers.filter(u => u.id !== userId));
        this.userToDelete.set(null);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al eliminar usuario');
        this.isLoading.set(false);
        this.userToDelete.set(null);
      }
    });

    this.subscriptions.add(sub);
  }

  viewUser(userId: string): void {
    this.router.navigate(['/admin/users', userId]);
  }

  getRolesDisplay(roles: string[]): string {
    if (!roles || roles.length === 0) return 'Sin roles';
    const uniqueRoles = [...new Set(roles)];
    return uniqueRoles.join(', ');
  }
}
