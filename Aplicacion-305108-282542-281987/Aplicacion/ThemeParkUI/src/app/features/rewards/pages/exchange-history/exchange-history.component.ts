import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription, forkJoin } from 'rxjs';
import { RewardsService } from '../../services/rewards.service';
import { RewardExchange } from '../../models/reward-exchange.model';
import { Reward } from '../../models/reward.model';
import { ExchangeHistoryItemComponent } from '../../components/exchange-history-item/exchange-history-item.component';

interface ExchangeWithReward {
  exchange: RewardExchange;
  rewardName: string;
}

@Component({
  selector: 'app-exchange-history',
  imports: [CommonModule, ExchangeHistoryItemComponent],
  templateUrl: './exchange-history.component.html',
  styleUrl: './exchange-history.component.css'
})
export class ExchangeHistoryComponent implements OnInit, OnDestroy {
  exchanges = signal<ExchangeWithReward[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  totalPointsSpent = signal<number>(0);

  private subscriptions = new Subscription();
  private rewardsCache = new Map<number, string>();

  constructor(
    private rewardsService: RewardsService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadExchangeHistory();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadExchangeHistory(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.rewardsService.getUserExchanges().subscribe({
      next: (exchanges) => {
        if (exchanges.length === 0) {
          this.exchanges.set([]);
          this.totalPointsSpent.set(0);
          this.isLoading.set(false);
          return;
        }

        const uniqueRewardIds = [...new Set(exchanges.map(e => e.rewardId))];
        const rewardRequests = uniqueRewardIds.map(id =>
          this.rewardsService.getRewardById(id)
        );

        const rewardsSub = forkJoin(rewardRequests).subscribe({
          next: (rewards: Reward[]) => {
            rewards.forEach(reward => {
              this.rewardsCache.set(reward.id, reward.nombre);
            });

            const exchangesWithRewards: ExchangeWithReward[] = exchanges.map(exchange => ({
              exchange,
              rewardName: this.rewardsCache.get(exchange.rewardId) || 'Recompensa desconocida'
            }));

            this.exchanges.set(exchangesWithRewards);

            const total = exchanges.reduce((sum, ex) => sum + ex.puntosDescontados, 0);
            this.totalPointsSpent.set(total);

            this.isLoading.set(false);
          },
          error: (err) => {
            const exchangesWithRewards: ExchangeWithReward[] = exchanges.map(exchange => ({
              exchange,
              rewardName: 'Recompensa desconocida'
            }));
            this.exchanges.set(exchangesWithRewards);

            const total = exchanges.reduce((sum, ex) => sum + ex.puntosDescontados, 0);
            this.totalPointsSpent.set(total);

            this.isLoading.set(false);
          }
        });

        this.subscriptions.add(rewardsSub);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar el historial de canjes');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  goToRewards(): void {
    this.router.navigate(['/rewards']);
  }
}
