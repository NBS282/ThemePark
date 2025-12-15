import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Reward } from '../../models/reward.model';

@Component({
  selector: 'app-reward-card',
  imports: [CommonModule],
  templateUrl: './reward-card.component.html',
  styleUrl: './reward-card.component.css'
})
export class RewardCardComponent {
  reward = input.required<Reward>();
  showActions = input<boolean>(false);
  userPoints = input<number>(0);

  onView = output<number>();
  onEdit = output<number>();
  onDelete = output<number>();
  onExchange = output<number>();

  viewReward(): void {
    this.onView.emit(this.reward().id);
  }

  editReward(): void {
    this.onEdit.emit(this.reward().id);
  }

  deleteReward(): void {
    this.onDelete.emit(this.reward().id);
  }

  exchangeReward(): void {
    this.onExchange.emit(this.reward().id);
  }

  canAfford(): boolean {
    return this.userPoints() >= this.reward().costoPuntos;
  }

  isAvailable(): boolean {
    return this.reward().activa && this.reward().cantidadDisponible > 0;
  }

  getMembershipLabel(): string {
    const membership = this.reward().nivelMembresiaRequerido;
    if (!membership) {
      return 'Estándar';
    }

    if (typeof membership === 'string') {
      return membership;
    }

    const map: Record<number, string> = {
      0: 'Estándar',
      1: 'Premium',
      2: 'VIP'
    };

    return map[Number(membership)] || 'Estándar';
  }
}
