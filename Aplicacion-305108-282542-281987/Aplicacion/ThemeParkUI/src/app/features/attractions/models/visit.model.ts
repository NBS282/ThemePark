export interface Visit {
  id: string;
  idUsuario: string;
  nombreAtraccion: string;
  horaEntrada: string;
  horaSalida?: string;
  estaActiva: boolean;
  puntosObtenidos: number;
}
