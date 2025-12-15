import { User } from '../../../core/models/user.model';

export interface LoginResponse {
  token: string;
  usuario: User;
}