import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { adminGuard, authGuard } from './services/auth-guard';

const routes: Routes = [
  { path: '', redirectTo: 'tickets', pathMatch: 'full' },
  { path: 'login', component: Login },
  {
    path: 'tickets',
    canMatch: [authGuard],
    loadChildren: () => import('./tickets/tickets-module').then(m => m.TicketsModule),
  },
  {
    path: 'users',
    canMatch: [authGuard, adminGuard],
    loadChildren: () => import('./users/users-module').then(m => m.UsersModule),
  },
  {
    path: 'categories',
    canMatch: [authGuard, adminGuard],
    loadChildren: () => import('./categories/categories-module').then(m => m.CategoriesModule),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }