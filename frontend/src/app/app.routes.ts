import { Routes } from '@angular/router';
import { adminGuard, authGuard } from './core/guards/auth.guard';
import { MainLayoutComponent } from './layout/main-layout.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: 'cars',
        loadComponent: () =>
          import('./features/cars/car-list.component').then((module) => module.CarListComponent),
      },
      {
        path: 'my-rentals',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/rentals/my-rentals.component').then(
            (module) => module.MyRentalsComponent,
          ),
      },
      {
        path: 'admin/cars',
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/admin/admin-cars.component').then(
            (module) => module.AdminCarsComponent,
          ),
      },
      {
        path: 'admin/rentals',
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/admin/admin-rentals.component').then(
            (module) => module.AdminRentalsComponent,
          ),
      },
      {
        path: 'login',
        loadComponent: () =>
          import('./features/auth/auth-page.component').then((module) => module.AuthPageComponent),
      },
      { path: '', pathMatch: 'full', redirectTo: 'cars' },
      { path: '**', redirectTo: 'cars' },
    ],
  },
];
