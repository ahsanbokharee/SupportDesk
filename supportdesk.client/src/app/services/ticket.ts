import { HttpClient, HttpParams } from '@angular/common/http';
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

export interface UpdateTicket {
  subject: string;
  description: string;
  priority: string;
  status: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface TicketQuery {
  search?: string;
  status?: string;
  priority?: string;
  page?: number;
  pageSize?: number;
}

@Service()
export class TicketService {
  private http = inject(HttpClient);

  getTickets(query: TicketQuery = {}): Observable<PagedResult<Ticket>> {
    let params = new HttpParams();

    for (const [key, value] of Object.entries(query)) {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, value);
      }
    }

    return this.http.get<PagedResult<Ticket>>('/api/tickets', { params });
  }
  getTicket(id: number): Observable<Ticket> {
    return this.http.get<Ticket>(`/api/tickets/${id}`);
  }
  createTicket(ticket: CreateTicket): Observable<Ticket> {
    return this.http.post<Ticket>('/api/tickets', ticket);
  }
  updateTicket(id: number, ticket: UpdateTicket): Observable<Ticket> {
    return this.http.put<Ticket>(`/api/tickets/${id}`, ticket);
  }

}
