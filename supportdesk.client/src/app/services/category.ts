import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';

// Mirrors CategoryDto
export interface Category {
  id: number;
  name: string;
  prefix: string;
  defaultPriority: string;
  color: string;
  isActive: boolean;
  isDefault: boolean;
  ticketsIssued: number;
}

// Mirrors CreateCategoryDto
export interface CreateCategory {
  name: string;
  prefix: string;
  defaultPriority: string;
  color: string;
}

// Mirrors UpdateCategoryDto
export interface UpdateCategory extends CreateCategory {
  isActive: boolean;
}

@Service()
export class CategoryService {
  private http = inject(HttpClient);
  private baseUrl = '/api/categories';

  getCategories(includeInactive = false): Observable<Category[]> {
    const params = new HttpParams().set('includeInactive', includeInactive);
    return this.http.get<Category[]>(this.baseUrl, { params });
  }
  getCategory(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.baseUrl}/${id}`);
  }
  createCategory(category: CreateCategory): Observable<Category> {
    return this.http.post<Category>(this.baseUrl, category);
  }
  updateCategory(id: number, category: UpdateCategory): Observable<Category> {
    return this.http.put<Category>(`${this.baseUrl}/${id}`, category);
  }
}