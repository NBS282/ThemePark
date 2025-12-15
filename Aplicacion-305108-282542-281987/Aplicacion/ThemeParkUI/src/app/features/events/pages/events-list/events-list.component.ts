import { Component, OnInit, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { EventService } from '../../../../core/services/event.service';
import { Event } from '../../../../core/models/event.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-events-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './events-list.component.html',
  styleUrl: './events-list.component.css'
})
export class EventsListComponent implements OnInit, OnDestroy {
  events = signal<Event[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  eventToDelete = signal<string | null>(null);

  private subscriptions = new Subscription();

  constructor(
    private eventService: EventService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadEvents(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.eventService.getAllEvents().subscribe({
      next: (events) => {
        this.events.set(events);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar eventos');
        this.isLoading.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  confirmDelete(id: string): void {
    this.eventToDelete.set(id);
  }

  cancelDelete(): void {
    this.eventToDelete.set(null);
  }

  deleteEvent(): void {
    const id = this.eventToDelete();
    if (!id) return;

    this.isLoading.set(true);
    this.error.set(null);

    const sub = this.eventService.deleteEvent(id).subscribe({
      next: () => {
        const currentEvents = this.events();
        this.events.set(currentEvents.filter(e => e.id !== id));
        this.eventToDelete.set(null);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al eliminar evento');
        this.isLoading.set(false);
        this.eventToDelete.set(null);
      }
    });

    this.subscriptions.add(sub);
  }

  viewEvent(id: string): void {
    this.router.navigate(['/events/manage', id]);
  }

  createNew(): void {
    this.router.navigate(['/events/manage/new']);
  }
}
