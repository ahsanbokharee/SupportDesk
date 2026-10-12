import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { PagedResult, Ticket, TicketService } from '../../services/ticket';
import { FormBuilder } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-ticket-list',
  standalone: false,
  styleUrl: './ticket-list.css',
  templateUrl: './ticket-list.html',
})

export class TicketList implements OnInit {
  private ticketService = inject(TicketService);
  private fb = inject(FormBuilder);
  private destroyRef = inject(DestroyRef);

  statuses = ['Open', 'In Progress', 'Resolved', 'Closed'];
  priorities = ['Low', 'Medium', 'High'];
  pageSize = 10;

  result = signal<PagedResult<Ticket> | null>(null);
  page = signal(1);
  sortBy = signal('createdAt');
  sortDir = signal<'asc' | 'desc'>('desc');
  loading = signal(false);
  errorMessage = signal<string | null>(null);

  filters = this.fb.nonNullable.group({
    search: [''],
    status: [''],
    priority: [''],
  });

  ngOnInit() {
    this.filters.valueChanges
      .pipe(debounceTime(300), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.page.set(1);
        this.load();
      });

    this.load();
  }

  load() {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.ticketService
      .getTickets({
        ...this.filters.getRawValue(),
        page: this.page(),
        pageSize: this.pageSize,
        sortBy: this.sortBy(),
        sortDir: this.sortDir(),
      })
      .subscribe({
        next: result => {
          this.result.set(result);
          this.loading.set(false);
        },
        error: err => {
          this.loading.set(false);
          this.errorMessage.set('Could not load tickets.');
          console.error(err);
        }
      });
  }

  goToPage(page: number) {
    this.page.set(page);
    this.load();
  }

  clearFilters() {
    this.filters.reset();
  }
  
  sort(column: string) {
    if (this.sortBy() === column) {
      this.sortDir.set(this.sortDir() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortBy.set(column);
      this.sortDir.set(column === 'subject' ? 'asc' : 'desc');
    }
    this.page.set(1);
    this.load();
  }

  sortIndicator(column: string): string {
    if (this.sortBy() !== column) {
      return '';
    }
    return this.sortDir() === 'asc' ? ' ▲' : ' ▼';
  }
}
