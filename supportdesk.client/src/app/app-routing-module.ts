import { Component, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TicketList } from './tickets/ticket-list/ticket-list';
import { TicketDetail } from './tickets/ticket-detail/ticket-detail';
import { TicketForm } from './tickets/ticket-form/ticket-form';
import { Login } from './auth/login/login';
import { adminGuard, authGuard } from './services/auth-guard';
import { UserList } from './users/user-list/user-list';
import { UserForm } from './users/user-form/user-form';

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
  {
    path: 'users',
    canActivate: [authGuard, adminGuard],
    children: [
      { path: '', component: UserList },
      { path: 'new', component: UserForm },
      { path: ':id/edit', component: UserForm },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
