import { Component, OnInit, signal, effect, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Attraction } from '../../models/attraction.model';
import { CreateAttraction, TipoAtraccionLabels } from '../../models/create-attraction.model';
import { UpdateAttraction } from '../../models/update-attraction.model';

@Component({
  selector: 'app-attraction-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './attraction-form.component.html',
  styleUrl: './attraction-form.component.css'
})
export class AttractionFormComponent implements OnInit {
  attraction = input<Attraction | null>(null);

  onSave = output<CreateAttraction | UpdateAttraction>();
  onCancel = output<void>();

  attractionForm: FormGroup;
  error = signal<string | null>(null);

  tiposAtraccion = [
    { value: 0, label: 'Montaña Rusa' },
    { value: 1, label: 'Simulador' },
    { value: 2, label: 'Espectáculo' },
    { value: 3, label: 'Zona Interactiva' }
  ];

  constructor(private fb: FormBuilder) {
    this.attractionForm = this.fb.group({
      nombre: ['', [Validators.required]],
      tipo: [0, [Validators.required]],
      edadMinima: [0, [Validators.required, Validators.min(0)]],
      capacidadMaxima: [1, [Validators.required, Validators.min(1)]],
      descripcion: ['', [Validators.required]],
      points: [0]
    });

    effect(() => {
      const data = this.attraction();
      if (data) {
        this.attractionForm.patchValue({
          nombre: data.nombre,
          descripcion: data.descripcion,
          capacidadMaxima: data.capacidadMaxima,
          edadMinima: data.edadMinima
        });
        this.attractionForm.get('nombre')?.disable();
      }
    });
  }

  ngOnInit(): void {}

  get isEditMode(): boolean {
    return this.attraction() !== null;
  }

  save(): void {
    if (this.attractionForm.invalid) {
      this.error.set('Por favor complete todos los campos requeridos');
      return;
    }

    this.error.set(null);

    if (this.isEditMode) {
      const updateData: UpdateAttraction = {
        descripcion: this.attractionForm.get('descripcion')?.value,
        capacidadMaxima: this.attractionForm.get('capacidadMaxima')?.value,
        edadMinima: this.attractionForm.get('edadMinima')?.value
      };
      this.onSave.emit(updateData);
    } else {
      const createData: CreateAttraction = this.attractionForm.value;
      this.onSave.emit(createData);
    }
  }

  cancel(): void {
    this.onCancel.emit();
  }
}
