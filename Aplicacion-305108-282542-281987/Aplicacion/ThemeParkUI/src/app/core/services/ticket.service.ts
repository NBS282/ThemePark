import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Ticket, CreateTicketRequest } from '../models/ticket.model';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  myTickets = signal<Ticket[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getMyTickets(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.http.get<Ticket[]>(`${this.apiUrl}/tickets`)
      .subscribe({
        next: (tickets) => {
          this.myTickets.set(tickets);
          this.isLoading.set(false);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Error al cargar tickets');
          this.isLoading.set(false);
        }
      });
  }

  createTicket(request: CreateTicketRequest): Observable<Ticket> {
    return this.http.post<Ticket>(`${this.apiUrl}/tickets`, request);
  }
}
