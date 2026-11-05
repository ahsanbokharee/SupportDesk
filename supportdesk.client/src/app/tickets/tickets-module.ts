import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { MaterialModule } from '../material-module';
import { TicketList } from './ticket-list/ticket-list';
import { TicketDetail } from './ticket-detail/ticket-detail';
import { TicketForm } from './ticket-form/ticket-form';

const routes: Routes = [
    { path: '', component: TicketList },
    { path: 'new', component: TicketForm },
    { path: ':id/edit', component: TicketForm },
    { path: ':id', component: TicketDetail },
];

@NgModule({
    declarations: [TicketList, TicketDetail, TicketForm],
    imports: [CommonModule, ReactiveFormsModule, MaterialModule, RouterModule.forChild(routes)],
})
export class TicketsModule { }