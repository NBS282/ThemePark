import { Component, inject, computed, OnInit, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { TicketService } from '../../../../core/services/ticket.service';
import { EventService } from '../../../../core/services/event.service';
import { Event } from '../../../../core/models/event.model';

interface QuickAction {
  label: string;
  route: string;
  icon: string;
  roles: string[];
}

@Component({
  selector: 'app-home-page',
  imports: [CommonModule, RouterLink],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css'
})
export class HomePageComponent implements OnInit {
  private authService = inject(AuthService);
  private ticketService = inject(TicketService);
  private eventService = inject(EventService);

  isLoggedIn = computed(() => this.authService.currentUser() !== null);
  currentUser = this.authService.currentUser;
  userRoles = computed(() => this.currentUser()?.roles || []);
  primaryRole = computed(() => this.userRoles()[0] || '');

  isOnlyAdmin = computed(() => {
    const roles = this.userRoles();
    const hasAdmin = roles.some(role => role.toLowerCase() === 'administradorparque');
    const hasVisitor = roles.some(role => role.toLowerCase() === 'visitante');
    const hasOperator = roles.some(role => role.toLowerCase() === 'operadoratraccion');
    return hasAdmin && !hasVisitor && !hasOperator;
  });

  isOnlyOperator = computed(() => {
    const roles = this.userRoles();
    const hasOperator = roles.some(role => role.toLowerCase() === 'operadoratraccion');
    const hasVisitor = roles.some(role => role.toLowerCase() === 'visitante');
    const hasAdmin = roles.some(role => role.toLowerCase() === 'administradorparque');
    return hasOperator && !hasVisitor && !hasAdmin;
  });

  isAdminAndOperator = computed(() => {
    const roles = this.userRoles();
    const hasAdmin = roles.some(role => role.toLowerCase() === 'administradorparque');
    const hasOperator = roles.some(role => role.toLowerCase() === 'operadoratraccion');
    const hasVisitor = roles.some(role => role.toLowerCase() === 'visitante');
    return hasAdmin && hasOperator && !hasVisitor;
  });

  hasVisitorRole = computed(() => {
    const roles = this.userRoles();
    return roles.some(role => role.toLowerCase() === 'visitante');
  });

  totalPoints = signal<number>(0);
  totalTickets = signal<number>(0);
  membershipLevel = computed(() => this.currentUser()?.nivelMembresia || 'Básico');

  upcomingEvents = signal<Event[]>([]);

  constructor() {
    this.authService.registerOnLogin(() => {
      this.loadUserStats();
    });

    effect(() => {
      const tickets = this.ticketService.myTickets();
      const activeTickets = tickets.filter(ticket => {
        const visitDate = new Date(ticket.fechaVisita);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        return visitDate >= today;
      });
      this.totalTickets.set(activeTickets.length);
    });
  }

  quickActions = computed<QuickAction[]>(() => {
    const userRoles = this.userRoles();

    if (this.hasVisitorRole()) {
      return [
        { label: 'Comprar Ticket', route: '/tickets/purchase', icon: '🎟️', roles: ['Visitante'] },
        { label: 'Mis Tickets', route: '/tickets/my-tickets', icon: '🎫', roles: ['Visitante'] },
        { label: 'Ver Atracciones', route: '/attractions', icon: '🎢', roles: ['Visitante'] },
        { label: 'Ver Eventos', route: '/events', icon: '🎪', roles: ['Visitante'] },
        { label: 'Mis Recompensas', route: '/rewards', icon: '🎁', roles: ['Visitante'] },
      ];
    }

    if (this.isAdminAndOperator()) {
      return [
        { label: 'Panel de Control', route: '/attractions/operator', icon: '🎛️', roles: ['OperadorAtraccion'] },
        { label: 'Gestión de Usuarios', route: '/admin/users', icon: '👥', roles: ['AdministradorParque'] },
        { label: 'Gestión de Atracciones', route: '/attractions/manage', icon: '🎢', roles: ['AdministradorParque'] },
        { label: 'Gestión de Eventos', route: '/events/manage', icon: '🎪', roles: ['AdministradorParque'] },
        { label: 'Gestión de Recompensas', route: '/rewards/admin', icon: '🎁', roles: ['AdministradorParque'] },
        { label: 'Reportes', route: '/attractions/report', icon: '📈', roles: ['AdministradorParque'] },
        { label: 'Configuración de Puntuación', route: '/admin/scoring-strategies', icon: '🎯', roles: ['AdministradorParque'] },
      ];
    }

    if (this.isOnlyAdmin()) {
      return [
        { label: 'Gestión de Usuarios', route: '/admin/users', icon: '👥', roles: ['AdministradorParque'] },
        { label: 'Gestión de Atracciones', route: '/attractions/manage', icon: '🎢', roles: ['AdministradorParque'] },
        { label: 'Gestión de Eventos', route: '/events/manage', icon: '🎪', roles: ['AdministradorParque'] },
        { label: 'Gestión de Recompensas', route: '/rewards/admin', icon: '🎁', roles: ['AdministradorParque'] },
        { label: 'Reportes', route: '/attractions/report', icon: '📈', roles: ['AdministradorParque'] },
        { label: 'Configuración de Puntuación', route: '/admin/scoring-strategies', icon: '🎯', roles: ['AdministradorParque'] },
      ];
    }

    if (this.isOnlyOperator()) {
      return [
        { label: 'Panel de Control', route: '/attractions/operator', icon: '🎛️', roles: ['OperadorAtraccion'] },
      ];
    }

    return [];
  });

  ngOnInit(): void {
    if (this.isLoggedIn()) {
      this.loadUserStats();
    }
    this.loadUpcomingEvents();
  }

  private loadUserStats(): void {
    this.authService.getUserPoints().subscribe({
      next: (points) => {
        this.totalPoints.set(points.puntosDisponibles);
      },
      error: () => {
        this.totalPoints.set(0);
      }
    });

    this.ticketService.getMyTickets();
  }

  private loadUpcomingEvents(): void {
    this.eventService.getAllEvents().subscribe({
      next: (events) => {
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const upcoming = events
          .filter(event => {
            const eventDate = new Date(event.fecha);
            eventDate.setHours(0, 0, 0, 0);
            return eventDate >= today;
          })
          .sort((a, b) => new Date(a.fecha).getTime() - new Date(b.fecha).getTime())
          .slice(0, 3);

        this.upcomingEvents.set(upcoming);
      },
      error: () => {
        this.upcomingEvents.set([]);
      }
    });
  }

  getGreeting(): string {
    const hour = new Date().getHours();
    if (hour < 12) return 'Buenos días';
    if (hour < 20) return 'Buenas tardes';
    return 'Buenas noches';
  }
}
