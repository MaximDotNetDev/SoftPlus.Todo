import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface SubStep {
  id: number;
  title: string;
  isCompleted: boolean;
  taskId: number;
}

export interface TodoItem {
  id: number;
  title: string;
  isCompleted: boolean;
  dueDate?: string;
  categoryId: number;
  subSteps?: SubStep[];
}

export interface Category {
  id: number;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class TodoService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7289/api';

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/categories`);
  }

  createCategory(name: string): Observable<Category> {
    return this.http.post<Category>(`${this.apiUrl}/categories`, { name });
  }

  deleteCategory(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/categories/${id}`);
  }

  getTodos(categoryId?: number, searchTerm?: string): Observable<TodoItem[]> {
    let params: string[] = [];
    if (categoryId) params.push(`categoryId=${categoryId}`);
    if (searchTerm && searchTerm.trim()) params.push(`searchTerm=${encodeURIComponent(searchTerm.trim())}`);

    const query = params.length ? `?${params.join('&')}` : '';
    const url = `${this.apiUrl}/tasks${query}`;

    return this.http.get<any>(url).pipe(
      map(res => {
        if (res && res.items) return res.items;
        if (res && res.data) return res.data;
        if (Array.isArray(res)) return res;
        return [];
      })
    );
  }

  getTodoById(id: number): Observable<TodoItem> {
    return this.http.get<TodoItem>(`${this.apiUrl}/tasks/${id}`);
  }

  createTodo(todo: { title: string; categoryId: number; dueDate?: string }): Observable<TodoItem> {
    return this.http.post<TodoItem>(`${this.apiUrl}/tasks`, todo);
  }

  updateTodo(todo: TodoItem): Observable<any> {
    const payload = {
      id: todo.id,
      title: todo.title,
      dueDate: todo.dueDate || null,
      categoryId: todo.categoryId
    };
    return this.http.put(`${this.apiUrl}/tasks/${todo.id}`, payload);
  }

  toggleTodo(id: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/tasks/${id}/toggle`, {});
  }

  deleteTodo(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/tasks/${id}`);
  }

  createSubStep(taskId: number, title: string): Observable<SubStep> {
    return this.http.post<SubStep>(`${this.apiUrl}/tasks/${taskId}/substeps`, { title });
  }

  updateSubStep(taskId: number, step: SubStep): Observable<any> {
    const payload = {
      id: step.id,
      title: step.title,
      isCompleted: step.isCompleted
    };
    return this.http.put(`${this.apiUrl}/tasks/${taskId}/substeps/${step.id}`, payload);
  }

  toggleSubStep(taskId: number, subStepId: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/tasks/${taskId}/substeps/${subStepId}/toggle`, {});
  }

  deleteSubStep(taskId: number, subStepId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/tasks/${taskId}/substeps/${subStepId}`);
  }
}
