export interface Ticket {
  codigoQR: string;
  fechaVisita: string;
  tipoEntrada: 'general' | 'evento';
  codigoIdentificacionUsuario: string;
  fechaCompra: string;
}

export interface CreateTicketRequest {
  fechaVisita: string;
  tipoEntrada: 'general' | 'evento';
  codigoIdentificacionUsuario: string;
  eventoId?: string;
}
