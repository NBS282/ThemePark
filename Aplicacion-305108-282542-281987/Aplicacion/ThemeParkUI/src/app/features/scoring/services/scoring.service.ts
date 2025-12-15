import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Ranking } from '../models/ranking.model';

@Injectable({
  providedIn: 'root'
})
export class ScoringService {
  private apiUrl = `${environment.apiUrl}/scoring`;

  isLoading = signal(false);
  error = signal<string | null>(null);

  constructor(private http: HttpClient) {}

  getDailyRanking(): Observable<Ranking[]> {
    return this.http.get<Ranking[]>(`${this.apiUrl}/ranking`);
  }
}
