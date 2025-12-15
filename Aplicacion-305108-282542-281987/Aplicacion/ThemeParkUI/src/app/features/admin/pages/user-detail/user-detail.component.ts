import { Component, OnInit, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin.service';
import { UserList } from '../../models/user-list.model';
import { Subscription } from 'rxjs';
import { EditUserRolesComponent } from '../../components/edit-user-roles/edit-user-roles.component';
import { AuthService } from '../../../../core/services/auth.service';
import { StorageService } from '../../../../core/services/storage.service';
import { UserCacheService } from '../../services/user-cache.service';

@Component({
  selector: 'app-user-detail',
  imports: [CommonModule, RouterLink, EditUserRolesComponent],
  templateUrl: './user-detail.component.html',
  styleUrl: './user-detail.component.css'
})
export class UserDetailComponent implements OnInit, OnDestroy {
  user = signal<UserList | null>(null);
  isLoading = signal(false);
  error = signal<string | null>(null);
  isEditingRoles = signal(false);

  private subscriptions = new Subscription();
  private userId: string = '';

  constructor(
    private adminService: AdminService,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private storageService: StorageService,
    private userCacheService: UserCacheService
  ) {}

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('id') || '';
    if (this.userId) {
      this.loadUser();
    } else {
      this.router.navigate(['/admin/users']);
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadUser(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.adminService.getUserById(this.userId).subscribe({
      next: (user) => {
        const cachedUser = this.userCacheService.getUpdatedUser(this.userId);

        if (cachedUser) {
          this.user.set(cachedUser);
        } else {
          this.user.set(user);
        }

        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar usuario');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  onEditRoles(): void {
    this.isEditingRoles.set(true);
  }

  onCancelEdit(): void {
    this.isEditingRoles.set(false);
  }

  onRolesUpdated(updatedUser: UserList): void {
    this.user.set(updatedUser);
    this.isEditingRoles.set(false);

    this.userCacheService.updateUser(updatedUser);

    const currentAuthUser = this.authService.currentUser();
    if (currentAuthUser && currentAuthUser.id === updatedUser.id) {
      const updatedAuthUser = {
        ...currentAuthUser,
        roles: updatedUser.roles,
        nivelMembresia: updatedUser.nivelMembresia
      };
      this.authService.currentUser.set(updatedAuthUser);
      this.storageService.saveUser(updatedAuthUser);
    }

    setTimeout(() => {
      this.loadUser();
    }, 500);
  }

  getRolesDisplay(roles: string[]): string {
    if (!roles || roles.length === 0) return 'Sin roles';
    const uniqueRoles = [...new Set(roles)];
    return uniqueRoles.join(', ');
  }

  goBack(): void {
    this.router.navigate(['/admin/users']);
  }
}
