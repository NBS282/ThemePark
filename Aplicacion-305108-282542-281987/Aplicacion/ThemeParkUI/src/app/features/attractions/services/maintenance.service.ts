import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Maintenance, CreateMaintenance } from '../models/maintenance.model';

@Injectable({
  providedIn: 'root'
})
export class MaintenanceService {
  private apiUrl = `${environment.apiUrl}/attractions`;

  constructor(private http: HttpClient) {}

  getMaintenancesByAttraction(attractionName: string): Observable<Maintenance[]> {
    return this.http.get<Maintenance[]>(`${this.apiUrl}/${attractionName}/maintenances`);
  }

  createMaintenance(attractionName: string, data: CreateMaintenance): Observable<Maintenance> {
    return this.http.post<Maintenance>(`${this.apiUrl}/${attractionName}/maintenances`, data);
  }

  deleteMaintenance(attractionName: string, maintenanceId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${attractionName}/maintenances/${maintenanceId}`);
  }
}
