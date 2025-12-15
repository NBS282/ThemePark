export interface ScoringStrategy {
  nombre: string;
  descripcion: string;
  activa: boolean;
  tipoConfiguracion: string;
  configuracion: ScoringConfiguration;
}

export interface CreateScoringStrategy {
  nombre: string;
  descripcion: string;
  tipoConfiguracion: string;
  configuracion: ScoringConfiguration;
}

export interface UpdateScoringStrategy {
  descripcion?: string;
  configuracion?: ScoringConfiguration;
}

export interface ScoringConfiguration {
  [key: string]: unknown;
}

export interface PuntuacionPorAtraccionConfig extends ScoringConfiguration {
  puntosPorAtraccion: number;
}

export interface PuntuacionComboConfig extends ScoringConfiguration {
  puntosPorCombo: number;
  cantidadAtracciones: number;
}

export interface PuntuacionPorEventoConfig extends ScoringConfiguration {
  puntosPorEvento: number;
  tipoEvento: string;
}

export interface ConfigurationType {
  id: string;
  name: string;
  description: string;
  isPlugin: boolean;
}

export interface ConfigurationSchema {
  isPlugin: boolean;
  typeIdentifier: string;
  name: string;
  description: string;
  schema: {
    type: string;
    properties: Record<string, ConfigurationProperty>;
    required: string[];
  };
}

export interface ConfigurationProperty {
  name: string;
  displayName: string;
  type: string;
  required: boolean;
  description: string;
}

export interface PluginScoringStrategy extends ScoringStrategy {
  esPlugin: true;
  pluginTypeIdentifier: string;
  configurationJson: string;
  algoritmo?: number;
}

export interface NativeScoringStrategy extends ScoringStrategy {
  esPlugin?: false;
  algoritmo: number;
}

export function isPluginStrategy(strategy: ScoringStrategy): strategy is PluginScoringStrategy {
  return 'esPlugin' in strategy && (strategy as PluginScoringStrategy).esPlugin === true;
}

export function isNativeStrategy(strategy: ScoringStrategy): strategy is NativeScoringStrategy {
  return 'algoritmo' in strategy && !isPluginStrategy(strategy);
}

export interface ScoringStrategyFormData {
  nombre: string;
  descripcion: string;
  tipoConfiguracion: string;
  configuracion: Record<string, unknown>;
  isPlugin: boolean;
}

export interface UpdateScoringStrategyData {
  descripcion: string;
  configuracion: Record<string, unknown>;
  tipoConfiguracion: string;
  isPlugin: boolean;
  algoritmo?: number;
}
