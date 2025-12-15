import { Component, effect } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RegisterFormComponent } from '../../components/register-form/register-form.component';
import { AuthService } from '../../../../core/services/auth.service';
import { UserRegister } from '../../models/user-register.model';

@Component({
  selector: 'app-register-page',
  imports: [CommonModule, RegisterFormComponent, RouterLink],
  templateUrl: './register-page.component.html',
  styleUrl: './register-page.component.css'
})
export class RegisterPageComponent {
  constructor(
    public authService: AuthService,
    private router: Router
  ) {
    effect(() => {
      const user = this.authService.currentUser();
      if (user) {
        this.router.navigate(['/profile']);
      }
    });
  }

  onRegister(userData: UserRegister): void {
    this.authService.register(userData);
  }

  get isLoading() {
    return this.authService.isLoading();
  }

  get error() {
    return this.authService.error();
  }
}
