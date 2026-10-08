import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Ticket, TicketService } from './services/ticket';


@Component({
  selector: 'app-root',
  standalone: false,
  styleUrl: './app.css',
  templateUrl: './app.html',
})

export class App {
    

  protected readonly title = signal('supportdesk.client');
}
