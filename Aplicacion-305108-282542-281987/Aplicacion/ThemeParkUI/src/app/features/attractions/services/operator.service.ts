import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AccessRequest } from '../models/access-request.model';
import { Visit } from '../models/visit.model';
import { Incident, CreateIncident } from '../models/incident.model';

@Injectable({
  providedIn: 'root'
})
export class OperatorService {
  private apiUrl = `${environment.apiUrl}/attractions`;

  constructor(private http: HttpClient) {}

  registerEntry(nombre: string, data: AccessRequest): Observable<Visit> {
    return this.http.post<Visit>(`${this.apiUrl}/${nombre}/access`, data);
  }

  registerExit(nombre: string, data: AccessRequest): Observable<Visit> {
    return this.http.post<Visit>(`${this.apiUrl}/${nombre}/exit`, data);
  }

  createIncident(nombre: string, data: CreateIncident): Observable<Incident> {
    return this.http.post<Incident>(`${this.apiUrl}/${nombre}/incidents`, data);
  }

  resolveIncident(nombre: string, incidentId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${nombre}/incidents/${incidentId}`);
  }
}
