import { Component, effect, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Reward } from '../../models/reward.model';

@Component({
  selector: 'app-reward-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reward-form.component.html',
  styleUrl: './reward-form.component.css'
})
export class RewardFormComponent {
  reward = input<Reward | null>(null);
  isEditMode = input<boolean>(false);

  onSubmit = output<{ nombre: string; descripcion: string; costoPuntos: number; cantidadDisponible: number; nivelMembresiaRequerido: number | null }>();
  onCancel = output<void>();

  rewardForm: FormGroup;
  isLoading = signal(false);

  membershipLevels = [
    { value: 0, label: 'Estándar' },
    { value: 1, label: 'Premium' },
    { value: 2, label: 'VIP' }
  ];

  constructor(private fb: FormBuilder) {
    this.rewardForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.minLength(3)]],
      descripcion: ['', [Validators.required, Validators.minLength(10)]],
      costoPuntos: [0, [Validators.required, Validators.min(1)]],
      cantidadDisponible: [0, [Validators.required, Validators.min(0)]],
      nivelMembresiaRequerido: [0, [Validators.required]]
    });

    effect(() => {
      const currentReward = this.reward();
      const editMode = this.isEditMode();

      if (currentReward && editMode) {
        setTimeout(() => {
          this.loadRewardData(currentReward);
          this.rewardForm.get('nombre')?.disable();
        }, 0);
      } else {
        this.rewardForm.get('nombre')?.enable();
      }
    });
  }

  private loadRewardData(reward: Reward): void {
    const membershipValue = this.getMembershipValue(reward.nivelMembresiaRequerido);

    this.rewardForm.setValue({
      nombre: reward.nombre,
      descripcion: reward.descripcion,
      costoPuntos: reward.costoPuntos,
      cantidadDisponible: reward.cantidadDisponible,
      nivelMembresiaRequerido: membershipValue
    });

    this.rewardForm.markAsPristine();
    this.rewardForm.markAsUntouched();
  }

  private getMembershipValue(membership: string | null): number {
    if (!membership) {
      return 0;
    }

    if (typeof membership === 'string') {
      const map: Record<string, number> = {
        'Estándar': 0,
        'Premium': 1,
        'VIP': 2
      };
      return map[membership] ?? 0;
    }

    const numValue = Number(membership);
    if (numValue >= 0 && numValue <= 2) {
      return numValue;
    }

    return 0;
  }

  submitForm(): void {
    if (this.rewardForm.invalid) {
      this.rewardForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);

    const formValue = this.rewardForm.getRawValue();

    this.onSubmit.emit({
      nombre: formValue.nombre,
      descripcion: formValue.descripcion,
      costoPuntos: Number(formValue.costoPuntos),
      cantidadDisponible: Number(formValue.cantidadDisponible),
      nivelMembresiaRequerido: Number(formValue.nivelMembresiaRequerido)
    });
  }

  cancel(): void {
    this.onCancel.emit();
  }

  getErrorMessage(fieldName: string): string {
    const control = this.rewardForm.get(fieldName);
    if (!control || !control.touched || !control.errors) {
      return '';
    }

    if (control.errors['required']) {
      return 'Este campo es requerido';
    }
    if (control.errors['minlength']) {
      return `Mínimo ${control.errors['minlength'].requiredLength} caracteres`;
    }
    if (control.errors['min']) {
      return `El valor mínimo es ${control.errors['min'].min}`;
    }

    return '';
  }
}
