import { Component, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TicketList } from './tickets/ticket-list/ticket-list';
import { TicketDetail } from './tickets/ticket-detail/ticket-detail';
import { TicketForm } from './tickets/ticket-form/ticket-form';
import { Login } from './auth/login/login';
import { authGuard } from './services/auth-guard';

const routes: Routes = [
  { path: '', redirectTo: 'tickets', pathMatch: 'full' },
  { path: 'login', component: Login },
  {
    path: 'tickets',
    canActivate: [authGuard],
    children: [
      { path: '', component: TicketList },
      { path: 'new', component: TicketForm },
      { path: ':id/edit', component: TicketForm },
      { path: ':id', component: TicketDetail }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
