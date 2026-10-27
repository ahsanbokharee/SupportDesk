import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { User, UserService } from '../../services/user';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-user-form',
  standalone: false,
  styleUrl: './user-form.css',
  templateUrl: './user-form.html',
})
export class UserForm implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private userService = inject(UserService);

  private idParam = this.route.snapshot.paramMap.get('id');
  userId = this.idParam ? Number(this.idParam) : null;
  isEdit = this.userId !== null;

  roles = ['Customer', 'Agent', 'Admin'];
  loading = signal(false);
  saving = signal(false);
  errorMessage = signal<string | null>(null);
  hidePassword = signal(true);
  user = signal<User | null>(null);

  form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
    password: ['', this.isEdit ? [] : [Validators.required, Validators.minLength(8)]],
    role: ['Customer', Validators.required],
    isActive: [true],
  });

  ngOnInit() {
    if (this.userId === null) {
      return;
    }

    this.loading.set(true);
    this.userService.getUser(this.userId).subscribe({
      next: user => {
        this.user.set(user);
        this.form.patchValue(user);
        this.loading.set(false);
      },
      error: err => {
        this.loading.set(false);
        this.errorMessage.set(err.status === 404 ? 'User not found.' : 'Could not load the user.');
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
    let request: Observable<User>;

    if (this.userId !== null) {
      const { password, ...changes } = values;
      request = this.userService.updateUser(this.userId, changes);
    } else {
      const { isActive, ...newUser } = values;
      request = this.userService.createUser(newUser);
    }

    request.subscribe({
      next: () => this.router.navigate(['/users']),
      error: err => {
        this.saving.set(false);
        this.errorMessage.set(err.error?.message ?? 'Could not save the user. Please check the fields.');
      }
    });
  }
}