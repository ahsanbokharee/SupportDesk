import { Component, inject, OnInit, signal } from '@angular/core';
import { Ticket, TicketService } from '../../services/ticket';

@Component({
  selector: 'app-ticket-list',
  standalone: false,
  styleUrl: './ticket-list.css',
  templateUrl: './ticket-list.html',
})

export class TicketList implements OnInit {

  private ticketService = inject(TicketService);

  tickets = signal<Ticket [] | null>(null);

  ngOnInit() {
    this.ticketService.getTickets().subscribe({
      next: result => this.tickets.set(result), 
      error: err => console.error(err)
    });
  }
}
