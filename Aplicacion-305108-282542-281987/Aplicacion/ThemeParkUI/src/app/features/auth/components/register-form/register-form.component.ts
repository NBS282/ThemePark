import { Component, output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserRegister } from '../../models/user-register.model';

@Component({
  selector: 'app-register-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register-form.component.html',
  styleUrl: './register-form.component.css'
})
export class RegisterFormComponent {
  submitRegister = output<UserRegister>();

  registerForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.registerForm = this.fb.group({
      nombre: [''],
      apellido: [''],
      email: [''],
      contraseña: [''],
      fechaNacimiento: ['']
    });
  }

  onSubmit(): void {
    const userData: UserRegister = this.registerForm.value;
    this.submitRegister.emit(userData);
  }
}
