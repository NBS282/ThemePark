import { Component, computed, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { User } from '../../../../core/models/user.model';

@Component({
  selector: 'app-profile-view',
  imports: [CommonModule],
  templateUrl: './profile-view.component.html',
  styleUrl: './profile-view.component.css'
})
export class ProfileViewComponent {
  user = input<User | null>(null);
  onEdit = output<void>();

  uniqueRoles = computed(() => {
    const currentUser = this.user();
    if (!currentUser || !currentUser.roles) return '';
    const uniqueRolesArray = [...new Set(currentUser.roles)];
    return uniqueRolesArray.join(', ');
  });

  editProfile(): void {
    this.onEdit.emit();
  }
}
