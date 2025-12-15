import { Component, effect } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginFormComponent } from '../../components/login-form/login-form.component';
import { AuthService } from '../../../../core/services/auth.service';
import { UserLogin } from '../../models/user-login.model';

@Component({
  selector: 'app-login-page',
  imports: [CommonModule, LoginFormComponent, RouterLink],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css'
})
export class LoginPageComponent {
  constructor(
    public authService: AuthService,
    private router: Router
  ) {
    effect(() => {
      const user = this.authService.currentUser();
      if (user) {
        this.router.navigate(['/']);
      }
    });
  }

  onLogin(credentials: UserLogin): void {
    this.authService.login(credentials.email, credentials.contraseña);
  }

  get isLoading() {
    return this.authService.isLoading();
  }

  get error() {
    return this.authService.error();
  }
}
