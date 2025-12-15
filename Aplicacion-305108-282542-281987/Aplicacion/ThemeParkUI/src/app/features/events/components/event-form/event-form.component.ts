import { Component, OnInit, signal, effect, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Event } from '../../../../core/models/event.model';
import { CreateEvent } from '../../models/create-event.model';
import { Attraction } from '../../../attractions/models/attraction.model';

@Component({
  selector: 'app-event-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './event-form.component.html',
  styleUrl: './event-form.component.css'
})
export class EventFormComponent implements OnInit {
  event = input<Event | null>(null);
  attractions = input<Attraction[]>([]);

  onSave = output<CreateEvent>();
  onCancel = output<void>();

  eventForm: FormGroup;
  error = signal<string | null>(null);
  selectedAttractions = signal<string[]>([]);

  constructor(private fb: FormBuilder) {
    this.eventForm = this.fb.group({
      nombre: ['', [Validators.required]],
      fecha: ['', [Validators.required]],
      hora: ['', [Validators.required]],
      aforo: [1, [Validators.required, Validators.min(1)]],
      costoAdicional: [0, [Validators.required, Validators.min(0)]]
    });

    effect(() => {
      const data = this.event();
      if (data) {
        this.eventForm.patchValue({
          nombre: data.name,
          fecha: data.fecha,
          hora: data.hora,
          aforo: data.aforo,
          costoAdicional: data.costoAdicional
        });
        this.eventForm.get('nombre')?.disable();
        const attractionNames = data.atracciones.map(a => a.nombre);
        this.selectedAttractions.set(attractionNames);
      }
    });
  }

  ngOnInit(): void {}

  get isEditMode(): boolean {
    return this.event() !== null;
  }

  toggleAttraction(nombre: string): void {
    const selected = this.selectedAttractions();
    if (selected.includes(nombre)) {
      this.selectedAttractions.set(selected.filter(n => n !== nombre));
    } else {
      this.selectedAttractions.set([...selected, nombre]);
    }
  }

  isAttractionSelected(nombre: string): boolean {
    return this.selectedAttractions().includes(nombre);
  }

  save(): void {
    if (this.eventForm.invalid) {
      this.error.set('Por favor complete todos los campos requeridos');
      return;
    }

    if (this.selectedAttractions().length === 0) {
      this.error.set('Debe seleccionar al menos una atracción');
      return;
    }

    this.error.set(null);

    const createData: CreateEvent = {
      ...this.eventForm.getRawValue(),
      atraccionesIncluidas: this.selectedAttractions()
    };

    this.onSave.emit(createData);
  }

  cancel(): void {
    this.onCancel.emit();
  }
}
