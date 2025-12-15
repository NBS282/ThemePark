import { Component, output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserLogin } from '../../models/user-login.model';

@Component({
  selector: 'app-login-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.css'
})
export class LoginFormComponent {
  submitLogin = output<UserLogin>();

  loginForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.loginForm = this.fb.group({
      email: [''],
      contraseña: ['']
    });
  }

  onSubmit(): void {
    const credentials: UserLogin = this.loginForm.value;
    this.submitLogin.emit(credentials);
  }
}
