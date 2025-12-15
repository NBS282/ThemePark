import { Attraction } from '../../features/attractions/models/attraction.model';

export interface Event {
  id: string;
  name: string;
  fecha: string;
  hora: string;
  aforo: number;
  costoAdicional: number;
  atracciones: Attraction[];
}
