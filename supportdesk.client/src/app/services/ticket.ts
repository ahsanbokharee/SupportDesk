import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';

export interface Ticket {
  id: number,
  ticketNumber: string,
  subject: string,
  description: string,
  status: string,
  priority: string,
  createdAt: Date
}

export interface CreateTicket {
  subject: string,
  description: string,
  priority: string
}


@Service()
export class TicketService {
  private http = inject(HttpClient);

  getTickets(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>('/api/tickets');
  }
  getTicket(id: number): Observable<Ticket> {
    return this.http.get<Ticket>(`/api/tickets/${id}`); 
  }
  createTicket(ticket: CreateTicket): Observable<Ticket> {
    return this.http.post<Ticket>('/api/tickets', ticket);
  }

}
