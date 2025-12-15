export interface CreateReward {
  nombre: string;
  descripcion: string;
  costoPuntos: number;
  cantidadDisponible: number;
  nivelMembresiaRequerido?: number | null;
}
