export interface UserRanking {
  nombre: string;
  apellido: string;
  email: string;
}

export interface Ranking {
  posicion: number;
  usuario: UserRanking;
  puntos: number;
}
