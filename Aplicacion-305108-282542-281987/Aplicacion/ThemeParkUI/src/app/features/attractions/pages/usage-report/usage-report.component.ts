import { Component, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AttractionService } from '../../services/attraction.service';
import { UsageReport } from '../../models/usage-report.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-usage-report',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './usage-report.component.html',
  styleUrl: './usage-report.component.css'
})
export class UsageReportComponent implements OnDestroy {
  reportData = signal<UsageReport[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  hasSearched = signal(false);

  reportForm: FormGroup;

  private subscriptions = new Subscription();

  constructor(
    private attractionService: AttractionService,
    private fb: FormBuilder
  ) {
    this.reportForm = this.fb.group({
      fechaInicio: ['', [Validators.required]],
      fechaFin: ['', [Validators.required]]
    });
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  generateReport(): void {
    if (this.reportForm.invalid) {
      this.error.set('Por favor complete ambas fechas');
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);
    this.hasSearched.set(true);

    const fechaInicio = this.reportForm.get('fechaInicio')?.value;
    const fechaFin = this.reportForm.get('fechaFin')?.value;

    const sub = this.attractionService.getUsageReport(fechaInicio, fechaFin).subscribe({
      next: (data) => {
        this.reportData.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error al generar reporte');
        this.isLoading.set(false);
        this.reportData.set([]);
      }
    });

    this.subscriptions.add(sub);
  }

  getTotalVisits(): number {
    return this.reportData().reduce((sum, item) => sum + item.cantidadVisitas, 0);
  }
}
