import { HttpClientModule } from '@angular/common/http';
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { TicketList } from './tickets/ticket-list/ticket-list';
import { TicketDetail } from './tickets/ticket-detail/ticket-detail';
import { ReactiveFormsModule } from '@angular/forms';
import { TicketForm } from './tickets/ticket-form/ticket-form';

@NgModule({
  declarations: [App, TicketList, TicketDetail, TicketForm],
  imports: [BrowserModule, HttpClientModule, AppRoutingModule, ReactiveFormsModule],
  providers: [provideBrowserGlobalErrorListeners()],
  bootstrap: [App],
})
export class AppModule {}
