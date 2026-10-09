import { Component, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TicketList } from './tickets/ticket-list/ticket-list';
import { TicketDetail } from './tickets/ticket-detail/ticket-detail';
import { TicketCreate } from './tickets/ticket-create/ticket-create';

const routes: Routes = [
  {path: '', redirectTo:'tickets', pathMatch:'full'},
  {path:'tickets', component: TicketList},
  {path:'tickets/new', component: TicketCreate},
  {path:'tickets/:id', component: TicketDetail}  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
