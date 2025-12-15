import { Component, signal, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CreateMaintenance } from '../../models/maintenance.model';

@Component({
  selector: 'app-maintenance-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './maintenance-form.component.html',
  styleUrl: './maintenance-form.component.css'
})
export class MaintenanceFormComponent {
  onSubmit = output<CreateMaintenance>();

  maintenanceForm: FormGroup;
  error = signal<string | null>(null);

  constructor(private fb: FormBuilder) {
    this.maintenanceForm = this.fb.group({
      fecha: ['', [Validators.required]],
      horaInicio: ['', [Validators.required, Validators.pattern(/^([0-1][0-9]|2[0-3]):[0-5][0-9]$/)]],
      duracionMinutos: ['', [Validators.required, Validators.min(1)]],
      descripcion: ['', [Validators.required]]
    });
  }

  submit(): void {
    if (this.maintenanceForm.invalid) {
      this.error.set('Por favor complete todos los campos correctamente');
      return;
    }

    this.error.set(null);

    const maintenanceData: CreateMaintenance = {
      fecha: this.maintenanceForm.value.fecha,
      horaInicio: this.maintenanceForm.value.horaInicio,
      duracionMinutos: parseInt(this.maintenanceForm.value.duracionMinutos),
      descripcion: this.maintenanceForm.value.descripcion
    };

    this.onSubmit.emit(maintenanceData);

    this.maintenanceForm.reset({
      fecha: '',
      horaInicio: '',
      duracionMinutos: '',
      descripcion: ''
    });
  }

  getMinDate(): string {
    const today = new Date();
    return today.toISOString().split('T')[0];
  }
}
