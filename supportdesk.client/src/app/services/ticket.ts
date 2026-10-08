import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';

export interface Ticket {
  id: number,
  ticketNumber: string,
  subject: string,
  status: string,
  priority: string,
  createdAt: Date
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

}
