import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { RewardsService } from '../../services/rewards.service';
import { Reward } from '../../models/reward.model';
import { RewardCardComponent } from '../../components/reward-card/reward-card.component';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-rewards-list',
  imports: [CommonModule, RewardCardComponent],
  templateUrl: './rewards-list.component.html',
  styleUrl: './rewards-list.component.css'
})
export class RewardsListComponent implements OnInit, OnDestroy {
  rewards = signal<Reward[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  userPoints = signal<number>(0);
  userTotalPoints = signal<number>(0);
  userSpentPoints = signal<number>(0);
  rewardToExchange = signal<Reward | null>(null);
  exchangeSuccess = signal(false);

  private subscriptions = new Subscription();

  constructor(
    private rewardsService: RewardsService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAvailableRewards();
    this.loadUserPoints();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadAvailableRewards(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.rewardsService.getAvailableRewards().subscribe({
      next: (rewards) => {
        this.rewards.set(rewards);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar recompensas disponibles');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  loadUserPoints(): void {
    const sub = this.authService.getUserPoints().subscribe({
      next: (points) => {
        this.userPoints.set(points.puntosDisponibles);
        this.userTotalPoints.set(points.puntosAcumulados);
        this.userSpentPoints.set(points.puntosGastados);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar puntos del usuario');
      }
    });

    this.subscriptions.add(sub);
  }

  viewReward(rewardId: number): void {
    this.router.navigate(['/rewards', rewardId]);
  }

  confirmExchange(rewardId: number): void {
    const reward = this.rewards().find(r => r.id === rewardId);
    if (reward) {
      this.rewardToExchange.set(reward);
    }
  }

  cancelExchange(): void {
    this.rewardToExchange.set(null);
  }

  executeExchange(): void {
    const reward = this.rewardToExchange();
    if (!reward) {
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.rewardsService.exchangeReward(reward.id).subscribe({
      next: () => {
        this.exchangeSuccess.set(true);
        this.rewardToExchange.set(null);
        this.isLoading.set(false);
        setTimeout(() => {
          this.exchangeSuccess.set(false);
          this.loadAvailableRewards();
          this.loadUserPoints();
        }, 3000);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al canjear la recompensa');
        this.isLoading.set(false);
        this.rewardToExchange.set(null);
      }
    });

    this.subscriptions.add(sub);
  }

  viewHistory(): void {
    this.router.navigate(['/rewards/history']);
  }

  goToAdminPanel(): void {
    this.router.navigate(['/rewards/admin']);
  }

  isAdmin(): boolean {
    const currentUser = this.authService.currentUser();
    return currentUser?.roles?.some(role =>
      role.toLowerCase() === 'administradorparque'
    ) ?? false;
  }
}
