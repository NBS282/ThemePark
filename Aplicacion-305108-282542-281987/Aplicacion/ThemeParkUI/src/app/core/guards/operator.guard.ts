import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const operatorGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const currentUser = authService.currentUser();

  if (!currentUser) {
    router.navigate(['/login']);
    return false;
  }

  const isOperator = currentUser.roles && currentUser.roles.some(role =>
    role.toLowerCase() === 'operadoratraccion'
  );

  if (!isOperator) {
    router.navigate(['/']);
    return false;
  }

  return true;
};
