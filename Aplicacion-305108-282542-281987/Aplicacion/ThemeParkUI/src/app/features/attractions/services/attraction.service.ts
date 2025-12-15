import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Attraction } from '../models/attraction.model';
import { CreateAttraction } from '../models/create-attraction.model';
import { UpdateAttraction } from '../models/update-attraction.model';
import { CapacityInfo } from '../models/capacity-info.model';
import { UsageReport } from '../models/usage-report.model';
import { Incident } from '../models/incident.model';

@Injectable({
  providedIn: 'root'
})
export class AttractionService {
  private apiUrl = `${environment.apiUrl}/attractions`;

  constructor(private http: HttpClient) {}

  getAllAttractions(): Observable<Attraction[]> {
    return this.http.get<Attraction[]>(this.apiUrl);
  }

  getAttractionByName(nombre: string): Observable<Attraction> {
    return this.http.get<Attraction>(`${this.apiUrl}/${nombre}`);
  }

  createAttraction(data: CreateAttraction): Observable<Attraction> {
    return this.http.post<Attraction>(this.apiUrl, data);
  }

  updateAttraction(nombre: string, data: UpdateAttraction): Observable<Attraction> {
    return this.http.put<Attraction>(`${this.apiUrl}/${nombre}`, data);
  }

  deleteAttraction(nombre: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${nombre}`);
  }

  getCapacity(nombre: string): Observable<CapacityInfo> {
    return this.http.get<CapacityInfo>(`${this.apiUrl}/${nombre}/capacity`);
  }

  getUsageReport(fechaInicio: string, fechaFin: string): Observable<UsageReport[]> {
    return this.http.get<UsageReport[]>(`${this.apiUrl}/report`, {
      params: { fechaInicio, fechaFin }
    });
  }

  getAllIncidents(): Observable<Incident[]> {
    return this.http.get<Incident[]>(`${this.apiUrl}/incidents`);
  }

  getIncidentsByAttraction(nombre: string): Observable<Incident[]> {
    return this.http.get<Incident[]>(`${this.apiUrl}/${nombre}/incidents`);
  }
}
