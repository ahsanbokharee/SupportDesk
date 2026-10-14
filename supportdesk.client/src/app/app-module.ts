import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { TicketList } from './tickets/ticket-list/ticket-list';
import { TicketDetail } from './tickets/ticket-detail/ticket-detail';
import { ReactiveFormsModule } from '@angular/forms';
import { TicketForm } from './tickets/ticket-form/ticket-form';
import { Login } from './auth/login/login';
import { authInterceptor } from './services/auth-interceptor';


@NgModule({
  declarations: [App, TicketList, TicketDetail, TicketForm, Login],
  imports: [BrowserModule, AppRoutingModule, ReactiveFormsModule],
  providers: [provideBrowserGlobalErrorListeners(),  provideHttpClient(withInterceptors([authInterceptor]))],
  bootstrap: [App],
 })
export class AppModule {}
