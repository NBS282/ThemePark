import { Component, OnInit, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AttractionService } from '../../services/attraction.service';
import { OperatorService } from '../../services/operator.service';
import { CapacityInfo } from '../../models/capacity-info.model';
import { AccessRequest } from '../../models/access-request.model';
import { CreateIncident, Incident } from '../../models/incident.model';
import { CapacityIndicatorComponent } from '../../components/capacity-indicator/capacity-indicator.component';
import { AccessControlFormComponent } from '../../components/access-control-form/access-control-form.component';
import { IncidentFormComponent } from '../../components/incident-form/incident-form.component';
import { Subscription, interval } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { SystemDateTimeService } from '../../../../core/services/system-datetime.service';

@Component({
  selector: 'app-operator-control',
  imports: [
    CommonModule,
    CapacityIndicatorComponent,
    AccessControlFormComponent,
    IncidentFormComponent
  ],
  templateUrl: './operator-control.component.html',
  styleUrl: './operator-control.component.css'
})
export class OperatorControlComponent implements OnInit, OnDestroy {
  attractionName = signal<string>('');
  capacity = signal<CapacityInfo | null>(null);
  incidents = signal<Incident[]>([]);
  isLoadingCapacity = signal(false);
  isLoadingIncidents = signal(false);
  error = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  private subscriptions = new Subscription();

  constructor(
    private attractionService: AttractionService,
    private operatorService: OperatorService,
    private route: ActivatedRoute,
    private router: Router,
    private systemDateTimeService: SystemDateTimeService
  ) {}

  ngOnInit(): void {
    const nombre = this.route.snapshot.paramMap.get('nombre');
    if (!nombre) {
      this.router.navigate(['/attractions/operator']);
      return;
    }

    this.attractionName.set(nombre);
    this.loadCapacity();
    this.loadIncidents();

    const refreshSub = interval(30000).pipe(
      switchMap(() => this.attractionService.getCapacity(nombre))
    ).subscribe({
      next: (capacity) => {
        this.capacity.set(capacity);
      },
      error: () => {
      }
    });

    this.subscriptions.add(refreshSub);

    const dateTimeSub = this.systemDateTimeService.dateTimeUpdated.subscribe(() => {
      this.loadCapacity();
      this.loadIncidents();
    });

    this.subscriptions.add(dateTimeSub);
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadCapacity(): void {
    const nombre = this.attractionName();
    this.isLoadingCapacity.set(true);
    this.error.set(null);

    const sub = this.attractionService.getCapacity(nombre).subscribe({
      next: (capacity) => {
        this.capacity.set(capacity);
        this.isLoadingCapacity.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar capacidad');
        this.isLoadingCapacity.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  loadIncidents(): void {
    const nombre = this.attractionName();
    this.isLoadingIncidents.set(true);
    this.error.set(null);

    const sub = this.attractionService.getIncidentsByAttraction(nombre).subscribe({
      next: (incidents) => {
        const activeIncidents = incidents.filter(i => i.estado === 'Activa');
        this.incidents.set(activeIncidents);
        this.isLoadingIncidents.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al cargar incidencias');
        this.isLoadingIncidents.set(false);
      }
    });

    this.subscriptions.add(sub);
  }

  handleEntry(data: AccessRequest): void {
    const nombre = this.attractionName();
    this.error.set(null);
    this.successMessage.set(null);

    const sub = this.operatorService.registerEntry(nombre, data).subscribe({
      next: () => {
        this.successMessage.set('Entrada registrada exitosamente');
        this.loadCapacity();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        const errorMessage = err.error?.message || err.error?.details || err.message || 'Error al registrar entrada';
        this.error.set(errorMessage);
      }
    });

    this.subscriptions.add(sub);
  }

  handleExit(data: AccessRequest): void {
    const nombre = this.attractionName();
    this.error.set(null);
    this.successMessage.set(null);

    const sub = this.operatorService.registerExit(nombre, data).subscribe({
      next: () => {
        this.successMessage.set('Salida registrada exitosamente');
        this.loadCapacity();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        const errorMessage = err.error?.message || err.error?.details || err.message || 'Error al registrar salida';
        this.error.set(errorMessage);
      }
    });

    this.subscriptions.add(sub);
  }

  handleIncidentSubmit(data: CreateIncident): void {
    const nombre = this.attractionName();
    this.error.set(null);
    this.successMessage.set(null);

    const sub = this.operatorService.createIncident(nombre, data).subscribe({
      next: () => {
        this.successMessage.set('Incidencia reportada exitosamente');
        this.loadIncidents();
        this.loadCapacity();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al reportar incidencia');
      }
    });

    this.subscriptions.add(sub);
  }

  resolveIncident(incidentId: string): void {
    const nombre = this.attractionName();
    this.error.set(null);
    this.successMessage.set(null);

    const sub = this.operatorService.resolveIncident(nombre, incidentId).subscribe({
      next: () => {
        this.successMessage.set('Incidencia resuelta exitosamente');
        this.loadIncidents();
        this.loadCapacity();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al resolver incidencia');
      }
    });

    this.subscriptions.add(sub);
  }

  goBack(): void {
    this.router.navigate(['/attractions/operator']);
  }

  getScheduledDateTime(incident: Incident): string | null {
    if (!incident.fechaProgramada || !incident.horaProgramada) {
      return null;
    }

    const datePart = incident.fechaProgramada.split('T')[0];
    return `${datePart}T${incident.horaProgramada}:00`;
  }
}
