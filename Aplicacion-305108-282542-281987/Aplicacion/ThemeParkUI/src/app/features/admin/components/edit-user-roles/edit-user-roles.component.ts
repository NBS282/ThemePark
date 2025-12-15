import { Component, OnInit, signal, OnDestroy, input, output, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../services/admin.service';
import { UserList } from '../../models/user-list.model';
import { UpdateUserRoles, RoleStringToEnum, MembershipStringToEnum, RolEnum } from '../../models/update-user-roles.model';
import { Subscription } from 'rxjs';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-edit-user-roles',
  imports: [CommonModule],
  templateUrl: './edit-user-roles.component.html',
  styleUrl: './edit-user-roles.component.css'
})
export class EditUserRolesComponent implements OnInit, OnDestroy {
  user = input.required<UserList>();
  onCancel = output<void>();
  onRolesUpdated = output<UserList>();

  availableRoles = ['OperadorAtraccion', 'Visitante', 'AdministradorParque'];
  availableMemberships = ['Estándar', 'Premium', 'VIP'];
  selectedRoles = signal<Set<string>>(new Set());
  selectedMembership = signal<string>('Estándar');
  isLoading = signal(false);
  error = signal<string | null>(null);

  private subscriptions = new Subscription();

  constructor(
    private adminService: AdminService,
    private authService: AuthService
  ) {
    effect(() => {
      const currentUser = this.user();
      if (currentUser) {
        this.initializeFromUser();
      }
    });
  }

  ngOnInit(): void {
    this.initializeFromUser();
  }

  private initializeFromUser(): void {
    const currentUser = this.user();

    if (currentUser && currentUser.roles) {
      const uniqueRoles = new Set(currentUser.roles);
      this.selectedRoles.set(uniqueRoles);
    }

    if (currentUser && currentUser.nivelMembresia) {
      this.selectedMembership.set(currentUser.nivelMembresia);
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  toggleRole(role: string): void {
    const current = new Set(this.selectedRoles());
    if (current.has(role)) {
      current.delete(role);
    } else {
      current.add(role);
    }
    this.selectedRoles.set(current);
  }

  isRoleSelected(role: string): boolean {
    return this.selectedRoles().has(role);
  }

  get showMembershipSelector(): boolean {
    return this.selectedRoles().has('Visitante');
  }

  cancel(): void {
    this.onCancel.emit();
  }

  save(): void {
    if (this.selectedRoles().size === 0) {
      this.error.set('Debe seleccionar al menos un rol');
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);

    const rolesArray = Array.from(this.selectedRoles());

    const rolesNumeros = rolesArray.map(roleStr => {
      const roleNum = RoleStringToEnum[roleStr];
      if (roleNum === undefined) {
        return RolEnum.Visitante;
      }
      return roleNum;
    });

    const nivelMembresiaNumero = MembershipStringToEnum[this.selectedMembership()];

    const updateData: UpdateUserRoles = {
      roles: rolesNumeros,
      nivelMembresia: nivelMembresiaNumero !== undefined ? nivelMembresiaNumero : 0
    };

    const currentUser = this.user();
    const sub = this.adminService.updateUserRoles(currentUser.id, updateData).subscribe({
      next: (updatedUser) => {
        this.isLoading.set(false);
        this.onRolesUpdated.emit(updatedUser);

        const loggedInUser = this.authService.currentUser();
        if (loggedInUser && loggedInUser.id === updatedUser.id) {
          this.authService.refreshCurrentUser();
        }
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al actualizar roles');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }
}
