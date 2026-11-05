import { Component, inject, signal, ViewChild } from '@angular/core';
import { Category, CategoryService } from '../../services/category';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'app-category-list',
  standalone: false,
  styleUrl: './category-list.css',
  templateUrl: './category-list.html',
})
export class CategoryList {
  private categoryService = inject(CategoryService);

  displayedColumns = ['name', 'prefix', 'defaultPriority', 'ticketsIssued', 'isActive', 'actions'];
  dataSource = new MatTableDataSource<Category>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  // The table sits inside @if, so MatSort only exists once data has loaded.
  @ViewChild(MatSort) set matSort(sort: MatSort) {
    if (sort) {
      this.dataSource.sort = sort;
    }
  }

  ngOnInit() {
    this.dataSource.sortingDataAccessor = (category, column) => {
      switch (column) {
        case 'name': return category.name.toLowerCase();
        case 'prefix': return category.prefix;
        case 'defaultPriority':
          return category.defaultPriority === 'High' ? 3 : category.defaultPriority === 'Medium' ? 2 : 1;
        case 'ticketsIssued': return category.ticketsIssued;
        case 'isActive': return category.isActive ? 1 : 0;
        default: return '';
      }
    };

    // Admins see inactive categories too
    this.categoryService.getCategories(true).subscribe({
      next: categories => {
        this.dataSource.data = categories;
        this.loading.set(false);
      },
      error: err => {
        this.loading.set(false);
        this.errorMessage.set('Could not load categories.');
        console.error(err);
      },
    });
  }

  applyFilter(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.dataSource.filter = value.trim().toLowerCase();
  }
}
