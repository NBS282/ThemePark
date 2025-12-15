export interface User {
  id: string;
  codigoIdentificacion: string;
  nombre: string;
  apellido: string;
  email: string;
  fechaNacimiento: string;
  nivelMembresia: string;
  roles: string[];
  fechaRegistro: string;
}