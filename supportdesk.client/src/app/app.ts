import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Ticket, TicketService } from './services/ticket';


@Component({
  selector: 'app-root',
  standalone: false,
  styleUrl: './app.css',
  templateUrl: './app.html',
})

export class App implements OnInit {
  //public forecasts: WeatherForecast[] = [];
  //public forecasts = signal<WeatherForecast[] | null>(null);
  //public tickets = signal<Ticket [] | null>(null);
  private ticketService = inject(TicketService);

  tickets = signal<Ticket [] | null>(null);

  //constructor(private http: HttpClient) {}

  ngOnInit() {
    this.ticketService.getTickets().subscribe({
      next: result => this.tickets.set(result), 
      error: err => console.error(err)
    });
  }


  

  protected readonly title = signal('supportdesk.client');
}
