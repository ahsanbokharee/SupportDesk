import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable } from 'rxjs';
import { Ticket, TicketService } from '../../services/ticket';
import { Category, CategoryService } from '../../services/category';
import { NotificationService } from '../../services/notification';

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
  private categoryService = inject(CategoryService);
  private notify = inject(NotificationService);
  private destroyRef = inject(DestroyRef);

  private idParam = this.route.snapshot.paramMap.get('id');
  ticketId = this.idParam ? Number(this.idParam) : null;
  isEdit = this.ticketId !== null;

  priorities = ['Low', 'Medium', 'High'];
  statuses = ['Open', 'In Progress', 'Resolved', 'Closed'];

  loading = signal(false);
  saving = signal(false);
  errorMessage = signal<string | null>(null);
  ticket = signal<Ticket | null>(null);
  categories = signal<Category[]>([]);

  form = this.fb.nonNullable.group({
    subject: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.maxLength(4000)]],
    priority: ['Medium', Validators.required],
    status: ['Open', Validators.required],
    categoryId: this.fb.control<number | null>(null, Validators.required),
  });

  // Active categories, plus the ticket's own category if it has since been deactivated,
  // so editing an old ticket still shows (and keeps) its category.
  categoryOptions = computed(() => {
    const list = this.categories();
    const t = this.ticket();
    if (t && !list.some(c => c.id === t.categoryId)) {
      return [...list, {
        id: t.categoryId, name: `${t.categoryName} (inactive)`, prefix: '',
        color: t.categoryColor, defaultPriority: t.priority,
        isActive: false, isDefault: false, ticketsIssued: 0,
      }];
    }
    return list;
  });

  ngOnInit() {
    this.categoryService.getCategories().subscribe(categories => {
      this.categories.set(categories);
      if (!this.isEdit) {
        // New ticket: preselect the default category (General) and its priority
        const defaultCategory = categories.find(c => c.isDefault);
        if (defaultCategory) {
          this.form.patchValue({ categoryId: defaultCategory.id, priority: defaultCategory.defaultPriority });
        }
      }
    });

    // New ticket: picking a category applies its default priority,
    // unless the user has already chosen a priority themselves.
    this.form.controls.categoryId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(id => {
        if (this.isEdit || this.form.controls.priority.dirty) {
          return;
        }
        const category = this.categories().find(c => c.id === id);
        if (category) {
          this.form.controls.priority.setValue(category.defaultPriority);
        }
      });

    if (this.ticketId === null) {
      return;
    }

    // Edit: load the ticket and fill the form (patchValue copies categoryId too)
    this.loading.set(true);
    this.ticketService.getTicket(this.ticketId).subscribe({
      next: ticket => {
        this.ticket.set(ticket);
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

  // True when editing and a different category is picked: the ticket will get a new number
  movingCategory() {
    return this.isEdit && this.form.controls.categoryId.value !== this.ticket()?.categoryId;
  }
  // The category currently picked, for the select's trigger (the closed field)
  selectedCategory() {
    const id = this.form.controls.categoryId.value;
    return this.categoryOptions().find(c => c.id === id);
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
      request = this.ticketService.updateTicket(this.ticketId, {
        ...values,
        categoryId: values.categoryId!,   // required by the validator, so never null here
      });
    } else {
      const { status, ...newTicket } = values;
      request = this.ticketService.createTicket(newTicket);
    }

    // Remember the number before saving, to tell the user if a move changed it
    const previousNumber = this.ticket()?.ticketNumber;

    request.subscribe({
      next: ticket => {
        if (!this.isEdit) {
          this.notify.success(`Ticket ${ticket.ticketNumber} created`);
        } else if (previousNumber && ticket.ticketNumber !== previousNumber) {
          this.notify.success(`Ticket moved: ${previousNumber} is now ${ticket.ticketNumber}`);
        } else {
          this.notify.success('Ticket saved');
        }
        this.router.navigate(['/tickets', ticket.id]);
      },
      error: err => {
        this.saving.set(false);
        this.errorMessage.set(
          err.error?.message ??   // the server's own words, e.g. "That category is no longer active."
          (err.status === 400
            ? 'The server rejected the ticket. Please check the fields.'
            : 'Could not save the ticket. Please try again.')
        );
        console.error(err);
      }
    });
  }
}