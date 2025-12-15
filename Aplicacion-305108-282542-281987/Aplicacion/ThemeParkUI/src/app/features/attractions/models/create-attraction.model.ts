export interface CreateAttraction {
  nombre: string;
  tipo: number;
  edadMinima: number;
  capacidadMaxima: number;
  descripcion: string;
  points?: number;
}

export enum TipoAtraccion {
  MontañaRusa = 0,
  Simulador = 1,
  Espectaculo = 2,
  ZonaInteractiva = 3
}

export const TipoAtraccionLabels: { [key: number]: string } = {
  0: 'Montaña Rusa',
  1: 'Simulador',
  2: 'Espectáculo',
  3: 'Zona Interactiva'
};

export const TipoAtraccionStringToEnum: { [key: string]: number } = {
  'MontañaRusa': 0,
  'Simulador': 1,
  'Espectaculo': 2,
  'ZonaInteractiva': 3
};
