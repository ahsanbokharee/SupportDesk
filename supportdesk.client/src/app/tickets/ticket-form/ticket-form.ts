import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Ticket, TicketService } from '../../services/ticket';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-ticket-form',
  standalone: false,
  styleUrl: './ticket-form.css',
  templateUrl: './ticket-form.html',
})
export class TicketForm implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private ticketService = inject(TicketService);

  private idParam = this.route.snapshot.paramMap.get('id');
  ticketId = this.idParam ? Number(this.idParam) : null;
  isEdit = this.ticketId !== null;

  priorities = ['Low', 'Medium', 'High'];
  statuses = ['Open', 'In Progress', 'Resolved', 'Closed'];

  loading = signal(false);
  saving = signal(false);
  errorMessage = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    subject: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.maxLength(4000)]],
    priority: ['Medium', Validators.required],
    status: ['Open', Validators.required],
  });

  ngOnInit() {
    if (this.ticketId === null) {
      return;
    }

    this.loading.set(true);
    this.ticketService.getTicket(this.ticketId).subscribe({
      next: ticket => {
        this.form.patchValue(ticket);
        this.loading.set(false);
      },
      error: err => {
        this.loading.set(false);
        this.errorMessage.set(
          err.status === 404 ? 'Ticket not found.' : 'Could not load the ticket.'
        );
      }
    });
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.errorMessage.set(null);

    const values = this.form.getRawValue();
    let request: Observable<Ticket>;

    if (this.ticketId !== null) {
      request = this.ticketService.updateTicket(this.ticketId, values);
    } else {
      const { status, ...newTicket } = values;
      request = this.ticketService.createTicket(newTicket);
    }

    request.subscribe({
      next: ticket => this.router.navigate(['/tickets', ticket.id]),
      error: err => {
        this.saving.set(false);
        this.errorMessage.set(
          err.status === 400
            ? 'The server rejected the ticket. Please check the fields.'
            : 'Could not save the ticket. Please try again.'
        );
        console.error(err);
      }
    });
  }
}
