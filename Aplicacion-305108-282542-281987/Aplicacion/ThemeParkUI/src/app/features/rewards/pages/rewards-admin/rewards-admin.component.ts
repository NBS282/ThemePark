import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { RewardsService } from '../../services/rewards.service';
import { Reward } from '../../models/reward.model';
import { CreateReward } from '../../models/create-reward.model';
import { UpdateReward } from '../../models/update-reward.model';
import { RewardCardComponent } from '../../components/reward-card/reward-card.component';
import { RewardFormComponent } from '../../components/reward-form/reward-form.component';

@Component({
  selector: 'app-rewards-admin',
  imports: [CommonModule, RewardCardComponent, RewardFormComponent],
  templateUrl: './rewards-admin.component.html',
  styleUrl: './rewards-admin.component.css'
})
export class RewardsAdminComponent implements OnInit, OnDestroy {
  rewards = signal<Reward[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  showCreateForm = signal(false);
  showEditForm = signal(false);
  rewardToEdit = signal<Reward | null>(null);
  rewardToDelete = signal<Reward | null>(null);

  private subscriptions = new Subscription();

  constructor(
    private rewardsService: RewardsService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAllRewards();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadAllRewards(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.rewardsService.getAllRewards().subscribe({
      next: (rewards) => {
        this.rewards.set(rewards);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar recompensas');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  openCreateForm(): void {
    this.showCreateForm.set(true);
    this.showEditForm.set(false);
    this.rewardToEdit.set(null);
  }

  openEditForm(rewardId: number): void {
    const reward = this.rewards().find(r => r.id === rewardId);
    if (reward) {
      this.rewardToEdit.set(reward);
      this.showEditForm.set(true);
      this.showCreateForm.set(false);
    }
  }

  closeForm(): void {
    this.showCreateForm.set(false);
    this.showEditForm.set(false);
    this.rewardToEdit.set(null);
  }

  handleCreateSubmit(formData: CreateReward): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.rewardsService.createReward(formData).subscribe({
      next: () => {
        this.successMessage.set('Recompensa creada exitosamente');
        this.closeForm();
        this.loadAllRewards();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al crear la recompensa');
        this.isLoading.set(false);
        this.closeForm();
      }
    });

    this.subscriptions.add(sub);
  }

  handleEditSubmit(formData: UpdateReward): void {
    const reward = this.rewardToEdit();
    if (!reward) {
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);

    const updateData: UpdateReward = {};

    if (formData.descripcion !== undefined && formData.descripcion !== reward.descripcion) {
      updateData.descripcion = formData.descripcion;
    }

    if (formData.costoPuntos !== undefined && formData.costoPuntos !== reward.costoPuntos) {
      updateData.costoPuntos = formData.costoPuntos;
    }

    if (formData.cantidadDisponible !== undefined && formData.cantidadDisponible !== reward.cantidadDisponible) {
      updateData.cantidadDisponible = formData.cantidadDisponible;
    }

    if (formData.nivelMembresiaRequerido !== undefined) {
      const currentMembershipValue = this.getMembershipValue(reward.nivelMembresiaRequerido);
      if (formData.nivelMembresiaRequerido !== currentMembershipValue) {
        updateData.nivelMembresiaRequerido = formData.nivelMembresiaRequerido;
      }
    }

    const sub = this.rewardsService.updateReward(reward.id, updateData).subscribe({
      next: () => {
        this.successMessage.set('Recompensa actualizada exitosamente');
        this.closeForm();
        this.loadAllRewards();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al actualizar la recompensa');
        this.isLoading.set(false);
        this.closeForm();
      }
    });

    this.subscriptions.add(sub);
  }

  private getMembershipValue(membership: string | null): number {
    if (!membership) {
      return 0;
    }
    const map: Record<string, number> = {
      'Estándar': 0,
      'Premium': 1,
      'VIP': 2
    };
    return map[membership] ?? 0;
  }

  confirmDelete(rewardId: number): void {
    const reward = this.rewards().find(r => r.id === rewardId);
    if (reward) {
      this.rewardToDelete.set(reward);
    }
  }

  cancelDelete(): void {
    this.rewardToDelete.set(null);
  }

  executeDelete(): void {
    const reward = this.rewardToDelete();
    if (!reward) {
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.rewardsService.deleteReward(reward.id).subscribe({
      next: () => {
        this.successMessage.set('Recompensa eliminada exitosamente');
        this.rewardToDelete.set(null);
        this.loadAllRewards();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al eliminar la recompensa');
        this.isLoading.set(false);
        this.rewardToDelete.set(null);
      }
    });

    this.subscriptions.add(sub);
  }

  backToList(): void {
    this.router.navigate(['/rewards']);
  }
}
