export interface Attraction {
  nombre: string;
  tipo: string;
  edadMinima: number;
  capacidadMaxima: number;
  descripcion: string;
  fechaCreacion: string;
  fechaModificacion?: string;
  tieneIncidencia: boolean;
}
