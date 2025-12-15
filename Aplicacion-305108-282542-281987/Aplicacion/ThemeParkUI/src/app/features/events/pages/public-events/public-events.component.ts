import { Component, OnInit, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventService } from '../../../../core/services/event.service';
import { Event } from '../../../../core/models/event.model';
import { Subscription } from 'rxjs';
import { SystemDateTimeService } from '../../../../core/services/system-datetime.service';

@Component({
  selector: 'app-public-events',
  imports: [CommonModule],
  templateUrl: './public-events.component.html',
  styleUrl: './public-events.component.css'
})
export class PublicEventsComponent implements OnInit, OnDestroy {
  events = signal<Event[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  selectedEvent = signal<Event | null>(null);

  private subscriptions = new Subscription();

  constructor(
    private eventService: EventService,
    private systemDateTimeService: SystemDateTimeService
  ) {}

  ngOnInit(): void {
    this.loadEvents();

    const dateTimeSub = this.systemDateTimeService.dateTimeUpdated.subscribe(() => {
      this.loadEvents();
    });

    this.subscriptions.add(dateTimeSub);
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

  viewDetails(event: Event): void {
    this.selectedEvent.set(event);
  }

  closeDetails(): void {
    this.selectedEvent.set(null);
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-ES', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  formatTime(timeString: string): string {
    return timeString.substring(0, 5);
  }

  isEventPast(event: Event): boolean {
    const currentDateTimeStr = this.systemDateTimeService.currentDateTime();
    if (!currentDateTimeStr) return false;

    const currentDateTime = new Date(currentDateTimeStr);
    const eventDate = new Date(event.fecha);
    const [hours, minutes] = event.hora.split(':');
    eventDate.setHours(parseInt(hours), parseInt(minutes));

    return currentDateTime > eventDate;
  }
}