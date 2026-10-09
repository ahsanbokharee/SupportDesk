import { Component, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TicketList } from './tickets/ticket-list/ticket-list';
import { TicketDetail } from './tickets/ticket-detail/ticket-detail';
import { TicketForm } from './tickets/ticket-form/ticket-form';

const routes: Routes = [
  {path: '', redirectTo:'tickets', pathMatch:'full'},
  {path:'tickets', component: TicketList},
  {path:'tickets/new', component: TicketForm},
  {path:'tickets/:id/edit', component: TicketForm},
  {path:'tickets/:id', component: TicketDetail}  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
