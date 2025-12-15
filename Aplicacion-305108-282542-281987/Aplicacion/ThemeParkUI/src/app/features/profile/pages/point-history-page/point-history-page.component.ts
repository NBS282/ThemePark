import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../../../../core/services/auth.service';
import { PointHistory } from '../../../../core/models/point-history.model';

@Component({
  selector: 'app-point-history-page',
  imports: [CommonModule],
  templateUrl: './point-history-page.component.html',
  styleUrl: './point-history-page.component.css'
})
export class PointHistoryPageComponent implements OnInit, OnDestroy {
  pointHistory = signal<PointHistory[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  totalPointsGained = signal<number>(0);
  totalPointsSpent = signal<number>(0);

  private subscriptions = new Subscription();

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPointHistory();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadPointHistory(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.authService.getUserPointHistory().subscribe({
      next: (history) => {
        this.pointHistory.set(history);

        const gained = history
          .filter(h => h.tipo === 'Ganancia')
          .reduce((sum, h) => sum + h.puntos, 0);

        const spent = history
          .filter(h => h.tipo === 'Gasto')
          .reduce((sum, h) => sum + Math.abs(h.puntos), 0);

        this.totalPointsGained.set(gained);
        this.totalPointsSpent.set(spent);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar el historial de puntos');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  goToProfile(): void {
    this.router.navigate(['/profile']);
  }

  goToRewards(): void {
    this.router.navigate(['/rewards']);
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getTypeClass(tipo: string): string {
    return tipo === 'Ganancia' ? 'type-gain' : 'type-spent';
  }

  getTypeIcon(tipo: string): string {
    return tipo === 'Ganancia' ? '↑' : '↓';
  }
}
