export interface Reward {
  id: number;
  nombre: string;
  descripcion: string;
  costoPuntos: number;
  cantidadDisponible: number;
  nivelMembresiaRequerido: string | null;
  activa: boolean;
  fechaCreacion: string;
}
