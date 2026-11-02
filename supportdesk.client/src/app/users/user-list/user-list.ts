import { Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { User, UserService } from '../../services/user';

@Component({
  selector: 'app-user-list',
  standalone: false,
  styleUrl: './user-list.css',
  templateUrl: './user-list.html',
})
export class UserList implements OnInit {
  private userService = inject(UserService);

  displayedColumns = ['name', 'role', 'isActive', 'createdAt', 'actions'];
  dataSource = new MatTableDataSource<User>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  // The table sits inside @if, so MatSort only exists once data has loaded.
  // A setter runs whenever Angular finds (or re-finds) the MatSort.
  @ViewChild(MatSort) set matSort(sort: MatSort) {
    if (sort) {
      this.dataSource.sort = sort;
    }
  }

  ngOnInit() {
    // Sort columns that aren't a single property on User
    this.dataSource.sortingDataAccessor = (user, column) => {
      switch (column) {
        case 'name': return `${user.firstName} ${user.lastName}`.toLowerCase();
        case 'role': return user.role;
        case 'isActive': return user.isActive ? 1 : 0;
        case 'createdAt': return user.createdAt;
        default: return '';
      }
    };

    this.userService.getUsers().subscribe({
      next: users => {
        this.dataSource.data = users;
        this.loading.set(false);
      },
      error: err => {
        this.loading.set(false);
        this.errorMessage.set('Could not load users.');
        console.error(err);
      },
    });
  }

  applyFilter(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.dataSource.filter = value.trim().toLowerCase();
  }

  initials(user: User) {
    return (user.firstName[0] ?? '') + (user.lastName[0] ?? '');
  }
}