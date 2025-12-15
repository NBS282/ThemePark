import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { SystemDateTimeService } from '../../core/services/system-datetime.service';

@Component({
  selector: 'app-main-layout',
  imports: [CommonModule, RouterLink, RouterOutlet],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css'
})
export class MainLayoutComponent {
  menuOpen = signal(false);
  profileMenuOpen = signal(false);

  isAdmin = computed(() => {
    const user = this.authService.currentUser();
    if (!user || !user.roles) {
      return false;
    }
    return user.roles.some(role => role.toLowerCase() === 'administradorparque');
  });

  isOperator = computed(() => {
    const user = this.authService.currentUser();
    if (!user || !user.roles) {
      return false;
    }
    return user.roles.some(role => role.toLowerCase() === 'operadoratraccion');
  });

  isVisitor = computed(() => {
    const user = this.authService.currentUser();
    if (!user || !user.roles) {
      return false;
    }
    return user.roles.some(role => role.toLowerCase() === 'visitante');
  });

  constructor(
    public authService: AuthService,
    public systemDateTimeService: SystemDateTimeService,
    private router: Router
  ) {}

  toggleMenu(): void {
    this.menuOpen.set(!this.menuOpen());
    this.profileMenuOpen.set(false);
  }

  closeMenu(): void {
    this.menuOpen.set(false);
  }

  toggleProfileMenu(): void {
    this.profileMenuOpen.set(!this.profileMenuOpen());
    this.menuOpen.set(false);
  }

  closeProfileMenu(): void {
    this.profileMenuOpen.set(false);
  }

  logout(): void {
    this.authService.logout();
    this.menuOpen.set(false);
    this.profileMenuOpen.set(false);
    this.router.navigate(['/']);
  }

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  get currentUser() {
    return this.authService.currentUser();
  }

  formatSystemDateTime(datetime: string | null): string {
    if (!datetime) {
      return 'No configurada';
    }
    return new Date(datetime).toLocaleString('es-ES', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
