import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Ticket, TicketService } from '../../services/ticket';
import { NotificationService } from '../../services/notification';

@Component({
  selector: 'app-ticket-detail',
  standalone: false,
  styleUrl: './ticket-detail.css',
  templateUrl: './ticket-detail.html',
})
export class TicketDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private ticketService = inject(TicketService);
  private notify =inject(NotificationService)

  ticketDetail = signal<Ticket | null>(null);
  notFound = signal(false);

  copied(success: boolean, ticketNumber: string) {
    this.notify.success(success ? `Copied ${ticketNumber}` : 'Could not copy');
  }

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
