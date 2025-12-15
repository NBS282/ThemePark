import { Component, computed, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CapacityInfo } from '../../models/capacity-info.model';

@Component({
  selector: 'app-capacity-indicator',
  imports: [CommonModule],
  templateUrl: './capacity-indicator.component.html',
  styleUrl: './capacity-indicator.component.css'
})
export class CapacityIndicatorComponent {
  capacity = input.required<CapacityInfo>();

  statusClass = computed(() => {
    const cap = this.capacity();
    if (cap.tieneIncidencia) return 'status-incident';
    if (cap.aforoActual >= cap.capacidadMaxima) return 'status-full';
    if (cap.porcentajeOcupacion >= 90) return 'status-full';
    if (cap.porcentajeOcupacion >= 70) return 'status-high';
    if (cap.porcentajeOcupacion >= 40) return 'status-medium';
    return 'status-low';
  });

  statusText = computed(() => {
    const cap = this.capacity();
    if (cap.tieneIncidencia) return 'CERRADA POR INCIDENCIA';
    if (cap.aforoActual >= cap.capacidadMaxima) return 'LLENA';
    if (cap.porcentajeOcupacion >= 90) return 'CASI LLENA';
    if (cap.porcentajeOcupacion >= 70) return 'ALTA OCUPACIÓN';
    if (cap.porcentajeOcupacion >= 40) return 'OCUPACIÓN MEDIA';
    return 'DISPONIBLE';
  });
}
