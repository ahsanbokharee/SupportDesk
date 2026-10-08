import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Ticket, TicketService } from '../../services/ticket';

@Component({
  selector: 'app-ticket-detail',
  standalone: false,
  styleUrl: './ticket-detail.css',
  templateUrl: './ticket-detail.html',
})
export class TicketDetail {
  private route = inject(ActivatedRoute);
  private ticketService = inject(TicketService);

  ticketDetail = signal<Ticket | null>(null);
  notFound = signal(false);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.ticketService.getTicket(id).subscribe(
      {
        next: result => this.ticketDetail.set(result),
        error: err => {
          if (err.status === 404) {
            this.notFound.set(true);
          }
          console.log(err);
        }
      }
    )
  }

}
