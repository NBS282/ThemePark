export interface RewardExchange {
  id: number;
  rewardId: number;
  userId: string;
  puntosDescontados: number;
  puntosRestantesUsuario: number;
  fechaCanje: string;
  estado: string;
}
