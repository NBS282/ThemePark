import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TicketService } from '../../../../core/services/ticket.service';
import { AuthService } from '../../../../core/services/auth.service';
import { EventService } from '../../../../core/services/event.service';
import { Event } from '../../../../core/models/event.model';
import { CreateTicketRequest } from '../../../../core/models/ticket.model';

@Component({
  selector: 'app-purchase-ticket-page',
  imports: [CommonModule, FormsModule],
  templateUrl: './purchase-ticket-page.component.html',
  styleUrl: './purchase-ticket-page.component.css'
})
export class PurchaseTicketPageComponent implements OnInit {
  fechaVisita: string = '';
  tipoEntrada: 'general' | 'evento' = 'general';
  selectedEventId: string = '';
  isLoading = false;
  error: string | null = null;
  successMessage: string | null = null;
  events = signal<Event[]>([]);
  isLoadingEvents = signal(false);

  constructor(
    private ticketService: TicketService,
    private authService: AuthService,
    private eventService: EventService,
    private router: Router
  ) {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    this.fechaVisita = tomorrow.toISOString().split('T')[0];
  }

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.isLoadingEvents.set(true);
    this.eventService.getAllEvents().subscribe({
      next: (events) => {
        this.events.set(events);
        this.isLoadingEvents.set(false);
      },
      error: () => {
        this.isLoadingEvents.set(false);
      }
    });
  }

  onTipoEntradaChange(): void {
    if (this.tipoEntrada === 'evento') {
      this.selectedEventId = '';
    }
  }

  onSubmit(): void {
    const user = this.authService.currentUser();
    if (!user) {
      this.error = 'Debe iniciar sesión para comprar tickets';
      return;
    }

    if (this.tipoEntrada === 'evento' && !this.selectedEventId) {
      this.error = 'Debe seleccionar un evento';
      return;
    }

    this.isLoading = true;
    this.error = null;
    this.successMessage = null;

    const request: CreateTicketRequest = {
      fechaVisita: this.fechaVisita,
      tipoEntrada: this.tipoEntrada,
      codigoIdentificacionUsuario: user.codigoIdentificacion
    };

    if (this.tipoEntrada === 'evento') {
      request.eventoId = this.selectedEventId;
    }

    this.ticketService.createTicket(request).subscribe({
      next: (ticket) => {
        this.successMessage = 'Ticket comprado exitosamente';
        this.isLoading = false;
        setTimeout(() => {
          this.router.navigate(['/tickets/my-tickets']);
        }, 1500);
      },
      error: (err) => {
        this.error = err.error?.message || 'Error al comprar el ticket';
        this.isLoading = false;
      }
    });
  }

  get minDate(): string {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split('T')[0];
  }
}
