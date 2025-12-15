import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { SystemDateTimeService } from '../../../../core/services/system-datetime.service';

@Component({
  selector: 'app-system-datetime-config',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './system-datetime-config.component.html',
  styleUrl: './system-datetime-config.component.css'
})
export class SystemDatetimeConfigComponent implements OnInit {
  datetimeForm: FormGroup;
  currentDateTime = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  error = signal<string | null>(null);
  isLoading = signal(false);

  constructor(
    private fb: FormBuilder,
    private systemDateTimeService: SystemDateTimeService
  ) {
    this.datetimeForm = this.fb.group({
      fechaHora: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadCurrentDateTime();
  }

  loadCurrentDateTime(): void {
    this.isLoading.set(true);
    this.systemDateTimeService.getSystemDateTime().subscribe({
      next: (response) => {
        this.currentDateTime.set(response.fechaHora);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar fecha del sistema');
        this.isLoading.set(false);
      }
    });
  }

  submit(): void {
    if (this.datetimeForm.invalid) {
      this.error.set('Por favor complete todos los campos');
      return;
    }

    this.error.set(null);
    this.successMessage.set(null);
    this.isLoading.set(true);

    const fechaHoraValue = this.datetimeForm.value.fechaHora;
    const date = new Date(fechaHoraValue);

    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const formattedDateTime = `${year}-${month}-${day}T${hours}:${minutes}`;

    this.systemDateTimeService.setSystemDateTime({ fechaHora: formattedDateTime }).subscribe({
      next: (response) => {
        this.successMessage.set(response.mensaje || 'Fecha actualizada correctamente');
        this.currentDateTime.set(response.fechaHora);
        this.isLoading.set(false);
        this.datetimeForm.reset();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al actualizar fecha del sistema');
        this.isLoading.set(false);
      }
    });
  }

  formatDateTime(datetime: string): string {
    return new Date(datetime).toLocaleString('es-ES', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
