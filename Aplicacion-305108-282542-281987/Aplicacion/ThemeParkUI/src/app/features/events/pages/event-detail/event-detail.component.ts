import { Component, OnInit, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../../../core/services/event.service';
import { Event } from '../../../../core/models/event.model';
import { CreateEvent } from '../../models/create-event.model';
import { EventFormComponent } from '../../components/event-form/event-form.component';
import { AttractionService } from '../../../attractions/services/attraction.service';
import { Attraction } from '../../../attractions/models/attraction.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-event-detail',
  imports: [CommonModule, EventFormComponent],
  templateUrl: './event-detail.component.html',
  styleUrl: './event-detail.component.css'
})
export class EventDetailComponent implements OnInit, OnDestroy {
  event = signal<Event | null>(null);
  attractions = signal<Attraction[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  isSaving = signal(false);

  private eventId: string | null = null;
  private subscriptions = new Subscription();

  constructor(
    private eventService: EventService,
    private attractionService: AttractionService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('id');

    this.loadAttractions();

    if (this.eventId && this.eventId !== 'new') {
      this.loadEvent(this.eventId);
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  get isEditMode(): boolean {
    return this.eventId !== null && this.eventId !== 'new';
  }

  loadAttractions(): void {
    const sub = this.attractionService.getAllAttractions().subscribe({
      next: (attractions) => {
        this.attractions.set(attractions);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar atracciones');
      }
    });

    this.subscriptions.add(sub);
  }

  loadEvent(id: string): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.eventService.getEventById(id).subscribe({
      next: (event) => {
        this.event.set(event);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar evento');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  handleSave(data: CreateEvent): void {
    this.isSaving.set(true);
    this.error.set(null);

    const request$ = this.isEditMode && this.eventId
      ? this.eventService.updateEvent(this.eventId, data)
      : this.eventService.createEvent(data);

    const sub = request$.subscribe({
      next: () => {
        this.isSaving.set(false);
        this.router.navigate(['/events/manage']);
      },
      error: (err) => {
        this.error.set(err.error?.message || `Error al ${this.isEditMode ? 'actualizar' : 'crear'} evento`);
        this.isSaving.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  handleCancel(): void {
    this.router.navigate(['/events/manage']);
  }
}
