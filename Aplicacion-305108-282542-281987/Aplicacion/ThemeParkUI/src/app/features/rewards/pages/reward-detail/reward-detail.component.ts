import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { RewardsService } from '../../services/rewards.service';
import { Reward } from '../../models/reward.model';

@Component({
  selector: 'app-reward-detail',
  imports: [CommonModule],
  templateUrl: './reward-detail.component.html',
  styleUrl: './reward-detail.component.css'
})
export class RewardDetailComponent implements OnInit, OnDestroy {
  reward = signal<Reward | null>(null);
  isLoading = signal(false);
  error = signal<string | null>(null);
  userPoints = signal<number>(0);
  showExchangeConfirm = signal(false);
  exchangeSuccess = signal(false);

  private subscriptions = new Subscription();
  private rewardId: number = 0;

  constructor(
    private rewardsService: RewardsService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.rewardId = parseInt(idParam, 10);
      this.loadReward();
    } else {
      this.error.set('ID de recompensa no válido');
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadReward(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.rewardsService.getRewardById(this.rewardId).subscribe({
      next: (reward) => {
        this.reward.set(reward);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar la recompensa');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  confirmExchange(): void {
    this.showExchangeConfirm.set(true);
  }

  cancelExchange(): void {
    this.showExchangeConfirm.set(false);
  }

  executeExchange(): void {
    const reward = this.reward();
    if (!reward) {
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.rewardsService.exchangeReward(reward.id).subscribe({
      next: () => {
        this.exchangeSuccess.set(true);
        this.showExchangeConfirm.set(false);
        this.loadReward();
        setTimeout(() => {
          this.router.navigate(['/rewards/history']);
        }, 2000);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al canjear la recompensa');
        this.isLoading.set(false);
        this.showExchangeConfirm.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  goBack(): void {
    this.router.navigate(['/rewards']);
  }

  canAfford(): boolean {
    const reward = this.reward();
    return reward ? this.userPoints() >= reward.costoPuntos : false;
  }

  isAvailable(): boolean {
    const reward = this.reward();
    return reward ? reward.activa && reward.cantidadDisponible > 0 : false;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-UY', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  getMembershipLabel(): string {
    const reward = this.reward();
    if (!reward) {
      return 'Estándar';
    }

    const membership = reward.nivelMembresiaRequerido;
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
