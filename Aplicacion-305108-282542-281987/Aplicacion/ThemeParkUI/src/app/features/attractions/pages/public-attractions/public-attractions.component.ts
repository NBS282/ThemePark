import { Component, OnInit, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AttractionService } from '../../services/attraction.service';
import { Attraction } from '../../models/attraction.model';
import { CapacityInfo } from '../../models/capacity-info.model';
import { TipoAtraccionLabels } from '../../models/create-attraction.model';
import { CapacityIndicatorComponent } from '../../components/capacity-indicator/capacity-indicator.component';
import { Subscription, forkJoin } from 'rxjs';
import { SystemDateTimeService } from '../../../../core/services/system-datetime.service';

interface AttractionWithCapacity {
  attraction: Attraction;
  capacity: CapacityInfo | null;
}

@Component({
  selector: 'app-public-attractions',
  imports: [CommonModule, CapacityIndicatorComponent],
  templateUrl: './public-attractions.component.html',
  styleUrl: './public-attractions.component.css'
})
export class PublicAttractionsComponent implements OnInit, OnDestroy {
  attractionsWithCapacity = signal<AttractionWithCapacity[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  selectedAttraction = signal<AttractionWithCapacity | null>(null);

  private subscriptions = new Subscription();

  constructor(
    private attractionService: AttractionService,
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
        if (attractions.length === 0) {
          this.attractionsWithCapacity.set([]);
          this.isLoading.set(false);
          return;
        }

        const capacityRequests = attractions.map(attraction =>
          this.attractionService.getCapacity(attraction.nombre)
        );

        const capacitiesSub = forkJoin(capacityRequests).subscribe({
          next: (capacities) => {
            const attractionsWithCap: AttractionWithCapacity[] = attractions.map((attraction, index) => ({
              attraction,
              capacity: capacities[index]
            }));
            this.attractionsWithCapacity.set(attractionsWithCap);
            this.isLoading.set(false);
          },
          error: () => {
            const attractionsWithCap: AttractionWithCapacity[] = attractions.map(attraction => ({
              attraction,
              capacity: null
            }));
            this.attractionsWithCapacity.set(attractionsWithCap);
            this.isLoading.set(false);
          }
        });

        this.subscriptions.add(capacitiesSub);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar atracciones');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  getTipoLabel(tipo: string): string {
    const tipoNum = parseInt(tipo);
    if (!isNaN(tipoNum) && TipoAtraccionLabels[tipoNum]) {
      return TipoAtraccionLabels[tipoNum];
    }
    return tipo;
  }

  viewDetails(item: AttractionWithCapacity): void {
    this.selectedAttraction.set(item);
  }

  closeDetails(): void {
    this.selectedAttraction.set(null);
  }
}
