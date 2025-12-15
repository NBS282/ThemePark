import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CreateReward } from '../models/create-reward.model';
import { RewardExchange } from '../models/reward-exchange.model';
import { Reward } from '../models/reward.model';
import { UpdateReward } from '../models/update-reward.model';

@Injectable({
  providedIn: 'root'
})
export class RewardsService {
  private apiUrl = `${environment.apiUrl}/rewards`;

  isLoading = signal(false);
  error = signal<string | null>(null);

  constructor(private http: HttpClient) {}

  createReward(reward: CreateReward): Observable<Reward> {
    return this.http.post<Reward>(this.apiUrl, reward);
  }

  getAllRewards(): Observable<Reward[]> {
    return this.http.get<Reward[]>(this.apiUrl);
  }

  getRewardById(id: number): Observable<Reward> {
    return this.http.get<Reward>(`${this.apiUrl}/${id}`);
  }

  updateReward(id: number, reward: UpdateReward): Observable<Reward> {
    return this.http.put<Reward>(`${this.apiUrl}/${id}`, reward);
  }

  deleteReward(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getAvailableRewards(): Observable<Reward[]> {
    return this.http.get<Reward[]>(`${this.apiUrl}/user`);
  }

  exchangeReward(rewardId: number): Observable<RewardExchange> {
    return this.http.post<RewardExchange>(`${this.apiUrl}/${rewardId}/exchange`, {});
  }

  getUserExchanges(): Observable<RewardExchange[]> {
    return this.http.get<RewardExchange[]>(`${this.apiUrl}/user/exchanges`);
  }
}
