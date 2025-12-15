import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SystemDateTime, SetSystemDateTime, SystemDateTimeResponse } from '../models/system-datetime.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class SystemDateTimeService {
  private apiUrl = `${environment.apiUrl}/datetime`;

  currentDateTime = signal<string | null>(null);
  private dateTimeUpdated$ = new Subject<void>();

  dateTimeUpdated = this.dateTimeUpdated$.asObservable();

  constructor(private http: HttpClient) {
    const authService = inject(AuthService);

    authService.registerOnLogin(() => this.loadCurrentDateTime());
    authService.registerCleanup(() => this.cleanup());

    if (authService.isAuthenticated()) {
      this.loadCurrentDateTime();
    }
  }

  getSystemDateTime(): Observable<SystemDateTime> {
    return this.http.get<SystemDateTime>(this.apiUrl).pipe(
      tap(response => this.currentDateTime.set(response.fechaHora))
    );
  }

  setSystemDateTime(data: SetSystemDateTime): Observable<SystemDateTimeResponse> {
    return this.http.post<SystemDateTimeResponse>(this.apiUrl, data).pipe(
      tap(response => {
        this.currentDateTime.set(response.fechaHora);
        this.dateTimeUpdated$.next();
      })
    );
  }

  loadCurrentDateTime(): void {
    this.getSystemDateTime().subscribe({
      next: () => {},
      error: () => {
        this.currentDateTime.set(null);
      }
    });
  }

  private cleanup(): void {
    this.currentDateTime.set(null);
  }
}
