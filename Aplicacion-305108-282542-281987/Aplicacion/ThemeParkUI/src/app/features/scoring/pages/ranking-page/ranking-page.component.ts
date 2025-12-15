import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { ScoringService } from '../../services/scoring.service';
import { Ranking } from '../../models/ranking.model';

@Component({
  selector: 'app-ranking-page',
  imports: [CommonModule],
  templateUrl: './ranking-page.component.html',
  styleUrl: './ranking-page.component.css'
})
export class RankingPageComponent implements OnInit, OnDestroy {
  rankings = signal<Ranking[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  private subscriptions = new Subscription();

  constructor(private scoringService: ScoringService) {}

  ngOnInit(): void {
    this.loadRanking();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadRanking(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.scoringService.getDailyRanking().subscribe({
      next: (rankings) => {
        this.rankings.set(rankings);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar el ranking diario');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  getMedalIcon(position: number): string {
    switch (position) {
      case 1: return '🥇';
      case 2: return '🥈';
      case 3: return '🥉';
      default: return '';
    }
  }
}
