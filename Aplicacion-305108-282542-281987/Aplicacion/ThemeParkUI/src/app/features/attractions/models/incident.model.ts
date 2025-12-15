export interface Incident {
  id: string;
  atraccion: string;
  descripcion: string;
  fechaCreacion: string;
  estado: string;
  maintenanceId?: string;
  fechaProgramada?: string;
  horaProgramada?: string;
}

export interface CreateIncident {
  descripcion: string;
  prioridad: string;
}
