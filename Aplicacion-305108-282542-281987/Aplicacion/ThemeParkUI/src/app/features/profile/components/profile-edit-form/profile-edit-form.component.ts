import { Component, OnInit, input, output, effect } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { User } from '../../../../core/models/user.model';
import { UserProfileUpdate } from '../../models/user-profile-update.model';

@Component({
  selector: 'app-profile-edit-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile-edit-form.component.html',
  styleUrl: './profile-edit-form.component.css'
})
export class ProfileEditFormComponent implements OnInit {
  user = input<User | null>(null);
  onSave = output<UserProfileUpdate>();
  onCancel = output<void>();

  editForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.editForm = this.fb.group({
      nombre: [''],
      apellido: [''],
      email: [''],
      contraseña: [''],
      fechaNacimiento: ['']
    });

    effect(() => {
      const currentUser = this.user();
      if (currentUser) {
        this.editForm.patchValue({
          nombre: currentUser.nombre,
          apellido: currentUser.apellido,
          email: currentUser.email,
          fechaNacimiento: currentUser.fechaNacimiento
        });
      }
    });
  }

  ngOnInit(): void {
    const currentUser = this.user();
    if (currentUser) {
      this.editForm.patchValue({
        nombre: currentUser.nombre,
        apellido: currentUser.apellido,
        email: currentUser.email,
        fechaNacimiento: currentUser.fechaNacimiento
      });
    }
  }

  onSubmit(): void {
    const userData: UserProfileUpdate = this.editForm.value;
    this.onSave.emit(userData);
  }

  cancel(): void {
    this.onCancel.emit();
  }
}
