import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable } from 'rxjs';
import { Category, CategoryService } from '../../services/category';
import { NotificationService } from '../../services/notification';

@Component({
  selector: 'app-category-form',
  standalone: false,
  styleUrl: './category-form.css',
  templateUrl: './category-form.html',
})
export class CategoryForm implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private categoryService = inject(CategoryService);
  private notify = inject(NotificationService);
  private destroyRef = inject(DestroyRef);

  private idParam = this.route.snapshot.paramMap.get('id');
  categoryId = this.idParam ? Number(this.idParam) : null;
  isEdit = this.categoryId !== null;

  priorities = ['Low', 'Medium', 'High'];
  swatches = ['#607d8b', '#1e88e5', '#00897b', '#43a047', '#fb8c00', '#e53935', '#8e24aa', '#6d4c41'];

  loading = signal(false);
  saving = signal(false);
  errorMessage = signal<string | null>(null);
  category = signal<Category | null>(null);

  form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    prefix: ['', [Validators.required, Validators.pattern(/^[A-Z]{2,6}$/)]],
    defaultPriority: ['Medium', Validators.required],
    color: ['#1e88e5', [Validators.required, Validators.pattern(/^#[0-9a-fA-F]{6}$/)]],
    isActive: [true],
  });

  ngOnInit() {
    // Prefixes are capital letters only: upper-case as the user types and drop anything else
    this.form.controls.prefix.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(value => {
        const cleaned = value.toUpperCase().replace(/[^A-Z]/g, '');
        if (cleaned !== value) {
          this.form.controls.prefix.setValue(cleaned, { emitEvent: false });
        }
      });

    if (this.categoryId === null) {
      return;
    }

    this.loading.set(true);
    this.categoryService.getCategory(this.categoryId).subscribe({
      next: category => {
        this.category.set(category);
        this.form.patchValue(category);
        if (category.ticketsIssued > 0) {
          // Same rule as the server: the prefix is locked once tickets exist
          this.form.controls.prefix.disable();
        }
        this.loading.set(false);
      },
      error: err => {
        this.loading.set(false);
        this.errorMessage.set(err.status === 404 ? 'Category not found.' : 'Could not load the category.');
      },
    });
  }

  prefixLocked() {
    return (this.category()?.ticketsIssued ?? 0) > 0;
  }

  // Preview: the number the NEXT ticket in this category would get
  sampleNumber() {
    const prefix = this.form.controls.prefix.value || 'PREFIX';
    return `${prefix}-${(this.category()?.ticketsIssued ?? 0) + 1}`;
  }

  pickColor(color: string) {
    this.form.controls.color.setValue(color);
    this.form.controls.color.markAsDirty();
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.errorMessage.set(null);

    // getRawValue includes DISABLED controls (the locked prefix); .value would leave it out
    const values = this.form.getRawValue();
    let request: Observable<Category>;

    if (this.categoryId !== null) {
      request = this.categoryService.updateCategory(this.categoryId, values);
    } else {
      const { isActive, ...newCategory } = values;
      request = this.categoryService.createCategory(newCategory);
    }

    request.subscribe({
      next: category => {
        this.notify.success(this.isEdit ? `${category.name} saved` : `${category.name} created`);
        this.router.navigate(['/categories']);
      },
      error: err => {
        this.saving.set(false);
        this.errorMessage.set(
          err.error?.message ??   // e.g. "The prefix BILL is already used by another category."
          (err.status === 400
            ? 'The server rejected the category. Please check the fields.'
            : 'Could not save the category. Please try again.')
        );
        console.error(err);
      },
    });
  }
}