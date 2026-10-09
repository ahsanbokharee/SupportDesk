import { Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { TicketService } from '../../services/ticket';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-ticket-create',
  standalone: false,
  styleUrl: './ticket-create.css',
  templateUrl: './ticket-create.html',
})
export class TicketCreate {
  private fb = inject(FormBuilder);
  private ticketService = inject(TicketService);
  private router = inject(Router);

  priorities = ['Low', 'Medium', 'High'];
  saving = signal(false);
  errorMessage = signal<string | null>(null);

  form = this.fb.nonNullable.group(
    {
      subject: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.required, Validators.maxLength(200)]],
      priority: ['Medium', [Validators.required] ]
    }
  );

  submit(){
    if (this.form.invalid)
    {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.errorMessage.set(null);

    this.ticketService.createTicket(this.form.getRawValue()).subscribe(
      {
        next: ticket => this.router.navigate(['/tickets', ticket.id]),
        error: err => {
          this.saving.set(false);
        this.errorMessage.set(
          err.status === 400
            ? 'The server rejected the ticket. Please check the fields.'
            : 'Could not create the ticket. Please try again.'
        );
        console.error(err);
        }
      }
    )
  }
}
