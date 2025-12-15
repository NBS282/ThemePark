import { Component, OnInit, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AttractionService } from '../../services/attraction.service';
import { Attraction } from '../../models/attraction.model';
import { CreateAttraction } from '../../models/create-attraction.model';
import { UpdateAttraction } from '../../models/update-attraction.model';
import { AttractionFormComponent } from '../../components/attraction-form/attraction-form.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-attraction-detail',
  imports: [CommonModule, AttractionFormComponent],
  templateUrl: './attraction-detail.component.html',
  styleUrl: './attraction-detail.component.css'
})
export class AttractionDetailComponent implements OnInit, OnDestroy {
  attraction = signal<Attraction | null>(null);
  isLoading = signal(false);
  error = signal<string | null>(null);
  isSaving = signal(false);

  private attractionName: string | null = null;
  private subscriptions = new Subscription();

  constructor(
    private attractionService: AttractionService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.attractionName = this.route.snapshot.paramMap.get('nombre');

    if (this.attractionName && this.attractionName !== 'new') {
      this.loadAttraction(this.attractionName);
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  get isEditMode(): boolean {
    return this.attractionName !== null && this.attractionName !== 'new';
  }

  loadAttraction(nombre: string): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.attractionService.getAttractionByName(nombre).subscribe({
      next: (attraction) => {
        this.attraction.set(attraction);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar atracción');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  handleSave(data: CreateAttraction | UpdateAttraction): void {
    this.isSaving.set(true);
    this.error.set(null);

    if (this.isEditMode && this.attractionName) {
      const sub = this.attractionService.updateAttraction(this.attractionName, data as UpdateAttraction).subscribe({
        next: (updatedAttraction) => {
          this.attraction.set(updatedAttraction);
          this.isSaving.set(false);
          this.router.navigate(['/attractions/manage']);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Error al actualizar atracción');
          this.isSaving.set(false);
        }
      });

      this.subscriptions.add(sub);
    } else {
      const sub = this.attractionService.createAttraction(data as CreateAttraction).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.router.navigate(['/attractions/manage']);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Error al crear atracción');
          this.isSaving.set(false);
        }
      });

      this.subscriptions.add(sub);
    }
  }

  handleCancel(): void {
    this.router.navigate(['/attractions/manage']);
  }
}
