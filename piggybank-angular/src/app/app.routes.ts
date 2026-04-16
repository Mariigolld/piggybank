import { Routes } from '@angular/router';
import { authGuard } from './core/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then(m => m.Login)
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register').then(m => m.Register)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard),
    canActivate: [authGuard]
  },
  {
    path: 'groups',
    loadComponent: () => import('./pages/groups/groups').then(m => m.Groups),
    canActivate: [authGuard]
  },
  {
    path: 'partner',
    loadComponent: () => import('./pages/partner/partner').then(m => m.Partner),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: 'dashboard' }
];
