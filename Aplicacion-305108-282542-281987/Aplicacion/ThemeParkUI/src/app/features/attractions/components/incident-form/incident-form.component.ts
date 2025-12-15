import { Component, signal, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CreateIncident } from '../../models/incident.model';

@Component({
  selector: 'app-incident-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './incident-form.component.html',
  styleUrl: './incident-form.component.css'
})
export class IncidentFormComponent {
  onSubmit = output<CreateIncident>();

  incidentForm: FormGroup;
  error = signal<string | null>(null);
  showOtraPrioridad = signal(false);

  prioridades = [
    { value: 'Baja', label: 'Baja' },
    { value: 'Media', label: 'Media' },
    { value: 'Alta', label: 'Alta' },
    { value: 'Otro', label: 'Otro' }
  ];

  constructor(private fb: FormBuilder) {
    this.incidentForm = this.fb.group({
      descripcion: ['', [Validators.required]],
      prioridad: ['Media', [Validators.required]],
      otraPrioridad: ['']
    });

    this.incidentForm.get('prioridad')?.valueChanges.subscribe(value => {
      this.showOtraPrioridad.set(value === 'Otro');
      if (value === 'Otro') {
        this.incidentForm.get('otraPrioridad')?.setValidators([Validators.required]);
      } else {
        this.incidentForm.get('otraPrioridad')?.clearValidators();
        this.incidentForm.get('otraPrioridad')?.setValue('');
      }
      this.incidentForm.get('otraPrioridad')?.updateValueAndValidity();
    });
  }

  submit(): void {
    if (this.incidentForm.invalid) {
      this.error.set('Por favor complete todos los campos');
      return;
    }

    this.error.set(null);

    const formValue = this.incidentForm.value;
    const incidentData: CreateIncident = {
      descripcion: formValue.descripcion,
      prioridad: formValue.prioridad === 'Otro' ? formValue.otraPrioridad : formValue.prioridad
    };

    this.onSubmit.emit(incidentData);

    this.incidentForm.reset({
      descripcion: '',
      prioridad: 'Media',
      otraPrioridad: ''
    });
    this.showOtraPrioridad.set(false);
  }
}
