import { Component, OnInit, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AttractionService } from '../../services/attraction.service';
import { Attraction } from '../../models/attraction.model';
import { TipoAtraccionLabels } from '../../models/create-attraction.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-operator-dashboard',
  imports: [CommonModule],
  templateUrl: './operator-dashboard.component.html',
  styleUrl: './operator-dashboard.component.css'
})
export class OperatorDashboardComponent implements OnInit, OnDestroy {
  attractions = signal<Attraction[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  private subscriptions = new Subscription();

  constructor(
    private attractionService: AttractionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAttractions();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadAttractions(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.attractionService.getAllAttractions().subscribe({
      next: (attractions) => {
        this.attractions.set(attractions);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar atracciones');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  operateAttraction(nombre: string): void {
    this.router.navigate(['/attractions/operator', nombre]);
  }

  manageMaintenance(nombre: string): void {
    this.router.navigate(['/attractions/operator', nombre, 'maintenance']);
  }

  getTipoLabel(tipo: string): string {
    const tipoNum = parseInt(tipo);
    if (!isNaN(tipoNum) && TipoAtraccionLabels[tipoNum]) {
      return TipoAtraccionLabels[tipoNum];
    }
    return tipo;
  }
}
