import { Component, OnInit, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AttractionService } from '../../services/attraction.service';
import { Attraction } from '../../models/attraction.model';
import { TipoAtraccionLabels } from '../../models/create-attraction.model';
import { Subscription } from 'rxjs';
import { SystemDateTimeService } from '../../../../core/services/system-datetime.service';

@Component({
  selector: 'app-attractions-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './attractions-list.component.html',
  styleUrl: './attractions-list.component.css'
})
export class AttractionsListComponent implements OnInit, OnDestroy {
  attractions = signal<Attraction[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  attractionToDelete = signal<string | null>(null);

  private subscriptions = new Subscription();

  constructor(
    private attractionService: AttractionService,
    private router: Router,
    private systemDateTimeService: SystemDateTimeService
  ) {}

  ngOnInit(): void {
    this.loadAttractions();

    const dateTimeSub = this.systemDateTimeService.dateTimeUpdated.subscribe(() => {
      this.loadAttractions();
    });

    this.subscriptions.add(dateTimeSub);
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

  confirmDelete(nombre: string): void {
    this.attractionToDelete.set(nombre);
  }

  cancelDelete(): void {
    this.attractionToDelete.set(null);
  }

  deleteAttraction(): void {
    const nombre = this.attractionToDelete();
    if (!nombre) return;

    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.attractionService.deleteAttraction(nombre).subscribe({
      next: () => {
        const currentAttractions = this.attractions();
        this.attractions.set(currentAttractions.filter(a => a.nombre !== nombre));
        this.attractionToDelete.set(null);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al eliminar atracción');
        this.isLoading.set(false);
        this.attractionToDelete.set(null);
      }
    });

    this.subscriptions.add(sub);
  }

  viewAttraction(nombre: string): void {
    this.router.navigate(['/attractions/manage', nombre]);
  }

  createNew(): void {
    this.router.navigate(['/attractions/manage/new']);
  }

  getTipoLabel(tipo: string): string {
    const tipoNum = parseInt(tipo);
    if (!isNaN(tipoNum) && TipoAtraccionLabels[tipoNum]) {
      return TipoAtraccionLabels[tipoNum];
    }
    return tipo;
  }
}
