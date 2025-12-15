export interface Maintenance {
  id: string;
  atraccion: string;
  fecha: string;
  horaInicio: string;
  duracionMinutos: number;
  descripcion: string;
  estado: string;
  incidentId?: string;
}

export interface CreateMaintenance {
  fecha: string;
  horaInicio: string;
  duracionMinutos: number;
  descripcion: string;
}
