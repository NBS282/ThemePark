import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { UserList } from '../models/user-list.model';
import { UpdateUserRoles } from '../models/update-user-roles.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<UserList[]> {
    return this.http.get<UserList[]>(this.apiUrl);
  }

  getUserById(id: string): Observable<UserList> {
    return this.http.get<UserList>(`${this.apiUrl}/${id}`);
  }

  updateUserRoles(id: string, roles: UpdateUserRoles): Observable<UserList> {
    return this.http.put<UserList>(`${this.apiUrl}/${id}/admin`, roles);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
