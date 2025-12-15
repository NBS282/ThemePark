import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  ScoringStrategy,
  CreateScoringStrategy,
  UpdateScoringStrategy,
  ConfigurationType,
  ConfigurationSchema
} from '../models/scoring-strategy.model';

@Injectable({
  providedIn: 'root'
})
export class ScoringStrategiesService {
  private apiUrl = `${environment.apiUrl}/scoring/strategies`;

  isLoading = signal(false);
  error = signal<string | null>(null);

  private readonly nativeTypeMap: Record<string, number> = {
    'PuntuacionPorAtraccion': 0,
    'PuntuacionCombo': 1,
    'PuntuacionPorEvento': 2
  };

  constructor(private http: HttpClient) {}

  getAllStrategies(): Observable<ScoringStrategy[]> {
    return this.http.get<ScoringStrategy[]>(this.apiUrl);
  }

  getStrategyByName(nombre: string): Observable<ScoringStrategy> {
    return this.http.get<ScoringStrategy>(`${this.apiUrl}/${nombre}`);
  }

  createStrategy(strategy: CreateScoringStrategy & { isPlugin?: boolean }): Observable<ScoringStrategy> {
    const backendDto: Record<string, unknown> = {
      Nombre: strategy.nombre,
      Descripcion: strategy.descripcion
    };

    const isPlugin = strategy.isPlugin ?? false;

    if (isPlugin) {
      backendDto['PluginTypeIdentifier'] = strategy.tipoConfiguracion;
      const configPascalCase = this.transformPluginConfigToPascalCase(strategy.configuracion);
      backendDto['ConfigurationJson'] = JSON.stringify(configPascalCase);
    } else {
      const algoritmoEnum = this.nativeTypeMap[strategy.tipoConfiguracion];
      if (algoritmoEnum !== undefined) {
        backendDto['Algoritmo'] = algoritmoEnum;
        backendDto['Configuracion'] = this.transformConfigurationToPascalCase(
          strategy.tipoConfiguracion,
          strategy.configuracion
        );
      }
    }

    return this.http.post<ScoringStrategy>(this.apiUrl, backendDto);
  }

  private transformPluginConfigToPascalCase(config: Record<string, unknown>): Record<string, unknown> {
    const transformed: Record<string, unknown> = {};

    Object.keys(config).forEach(key => {
      const pascalKey = key.charAt(0).toUpperCase() + key.slice(1);
      transformed[pascalKey] = config[key];
    });

    return transformed;
  }

  private transformConfigurationToPascalCase(typeId: string, config: Record<string, unknown>): Record<string, unknown> {
    if (typeId === 'PuntuacionPorAtraccion') {
      return {
        tipo: 'PuntuacionPorAtraccion',
        Valores: {
          'MontañaRusa': Number(config['montanaRusa'] || 0),
          'Simulador': Number(config['simulador'] || 0),
          'Espectaculo': Number(config['espectaculo'] || 0),
          'ZonaInteractiva': Number(config['zonaInteractiva'] || 0)
        }
      };
    } else if (typeId === 'PuntuacionCombo') {
      return {
        tipo: 'PuntuacionCombo',
        VentanaTemporalMinutos: Number(config['ventanaTemporalMinutos'] || 0),
        BonusMultiplicador: Number(config['bonusMultiplicador'] || 0),
        MinimoAtracciones: Number(config['minimoAtracciones'] || 0)
      };
    } else if (typeId === 'PuntuacionPorEvento') {
      return {
        tipo: 'PuntuacionPorEvento',
        Evento: String(config['evento'] || ''),
        Puntos: Number(config['puntos'] || 0)
      };
    }

    return config;
  }

  updateStrategy(nombre: string, strategy: UpdateScoringStrategy & { tipoConfiguracion?: string; isPlugin?: boolean; algoritmo?: number }): Observable<ScoringStrategy> {
    const backendDto: Record<string, unknown> = {};

    const isPlugin = strategy.isPlugin ?? false;
    const typeId = strategy.tipoConfiguracion || '';

    backendDto['Descripcion'] = strategy.descripcion;

    if (!isPlugin && strategy.algoritmo !== undefined) {
      backendDto['Algoritmo'] = strategy.algoritmo;
    }

    if (strategy.configuracion !== undefined) {
      if (!isPlugin) {
        const transformedConfig = this.transformConfigurationToPascalCase(
          typeId,
          strategy.configuracion
        );
        backendDto['Configuracion'] = transformedConfig;
      } else {
        const configPascalCase = this.transformPluginConfigToPascalCase(strategy.configuracion);
        backendDto['ConfigurationJson'] = JSON.stringify(configPascalCase);
      }
    }

    return this.http.patch<ScoringStrategy>(`${this.apiUrl}/${nombre}`, backendDto);
  }

  deleteStrategy(nombre: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${nombre}`);
  }

  activateStrategy(nombre: string): Observable<ScoringStrategy> {
    return this.http.patch<ScoringStrategy>(`${this.apiUrl}/${nombre}/activate`, {});
  }

  deactivateStrategies(): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/deactivate`, {});
  }

  getAvailableTypes(): Observable<ConfigurationType[]> {
    return this.http.get<ConfigurationType[]>(`${this.apiUrl}/available-types`);
  }

  getConfigurationSchema(typeId: string): Observable<ConfigurationSchema> {
    return this.http.get<ConfigurationSchema>(`${this.apiUrl}/config-schema/${typeId}`);
  }
}
