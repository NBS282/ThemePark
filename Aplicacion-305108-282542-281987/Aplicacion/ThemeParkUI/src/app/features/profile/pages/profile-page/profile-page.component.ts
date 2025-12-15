import { Component, signal, OnInit, effect } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ProfileViewComponent } from '../../components/profile-view/profile-view.component';
import { ProfileEditFormComponent } from '../../components/profile-edit-form/profile-edit-form.component';
import { AuthService } from '../../../../core/services/auth.service';
import { UserService } from '../../services/user.service';
import { UserProfileUpdate } from '../../models/user-profile-update.model';

@Component({
  selector: 'app-profile-page',
  imports: [CommonModule, ProfileViewComponent, ProfileEditFormComponent],
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.css'
})
export class ProfilePageComponent implements OnInit {
  isEditing = signal(false);

  constructor(
    public authService: AuthService,
    public userService: UserService,
    private router: Router
  ) {
    effect(() => {
      const updatedUser = this.userService.updatedUser();
      if (updatedUser) {
        this.authService.currentUser.set(updatedUser);
        this.isEditing.set(false);
      }
    });
  }

  ngOnInit(): void {
    this.authService.loadCurrentUser();

    const currentUser = this.authService.currentUser();
    if (currentUser) {
      this.userService.getCurrentProfile(currentUser.id);
    }
  }

  onEdit(): void {
    this.isEditing.set(true);
  }

  onSave(userData: UserProfileUpdate): void {
    const currentUser = this.authService.currentUser();
    if (currentUser) {
      this.userService.updateProfile(currentUser.codigoIdentificacion, userData, currentUser);
    }
  }

  onCancel(): void {
    this.isEditing.set(false);
  }

  get user() {
    return this.authService.currentUser();
  }

  get isLoading() {
    return this.userService.isLoading();
  }

  get error() {
    return this.userService.error();
  }
}
