import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event } from '../models/event.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private apiUrl = `${environment.apiUrl}/events`;

  constructor(private http: HttpClient) {}

  getAllEvents(): Observable<Event[]> {
    return this.http.get<Event[]>(this.apiUrl);
  }

  getEventById(id: string): Observable<Event> {
    return this.http.get<Event>(`${this.apiUrl}/${id}`);
  }

  createEvent(data: { nombre: string; fecha: string; hora: string; aforo: number; costoAdicional: number; atraccionesIncluidas: string[] }): Observable<Event> {
    return this.http.post<Event>(this.apiUrl, data);
  }

  updateEvent(id: string, data: { nombre: string; fecha: string; hora: string; aforo: number; costoAdicional: number; atraccionesIncluidas: string[] }): Observable<Event> {
    return this.http.put<Event>(`${this.apiUrl}/${id}`, data);
  }

  deleteEvent(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
