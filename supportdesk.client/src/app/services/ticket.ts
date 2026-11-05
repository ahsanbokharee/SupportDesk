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
  createdAt: string;            // was Date: JSON has no date type (see below)
  categoryId: number;           // ← new
  categoryName: string;         // ← new
  categoryColor: string;        // ← new
  previousNumbers: string[];    // ← new: old keys, e.g. ["BILL-3"]
}

export interface CreateTicket {
  subject: string,
  description: string,
  priority: string
  categoryId?: number | null;   // ← new: null/missing = default category
}

export interface UpdateTicket {
  subject: string;
  description: string;
  priority: string;
  status: string;
  categoryId: number;           // ← new: required ([Range(1, ...)] on the server)
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
  categoryId?: number | null;   // ← new
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDir?: 'asc' | 'desc';
}

@Service()
export class TicketService {
  private http = inject(HttpClient);
  private baseUrl = '/api/tickets';

  getTickets(query: TicketQuery = {}): Observable<PagedResult<Ticket>> {
    let params = new HttpParams();

    for (const [key, value] of Object.entries(query)) {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, value);
      }
    }
    return this.http.get<PagedResult<Ticket>>(this.baseUrl, { params });
  }
  getTicket(id: number): Observable<Ticket> {
    return this.http.get<Ticket>(`${this.baseUrl}/${id}`);
  }
  createTicket(ticket: CreateTicket): Observable<Ticket> {
    return this.http.post<Ticket>(this.baseUrl, ticket);
  }
  updateTicket(id: number, ticket: UpdateTicket): Observable<Ticket> {
    return this.http.put<Ticket>(`${this.baseUrl}/${id}`, ticket);
  }

}
