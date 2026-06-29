import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { SlicePipe } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { Category, SubStep, TodoItem, TodoService } from '../../core/services/todo.service';

@Component({
  selector: 'app-todos',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, SlicePipe],
  templateUrl: './todos.html',
  styleUrl: './todos.scss'
})
export class TodosComponent implements OnInit {
  private todoService = inject(TodoService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);

  categories: Category[] = [];
  todos: TodoItem[] = [];
  selectedCategoryId: number | undefined = undefined;

  activeTodo: TodoItem | null = null;
  newSubStepTitle = '';

  searchQuery = '';
  private searchTimer: any;

  isCreatingCategory = false;
  newCategoryTitle = '';

  toast: { text: string; icon: string; isError?: boolean } | null = null;
  private toastTimer: any;

  todoForm: FormGroup = this.fb.group({
    title: ['', Validators.required],
    dueDate: ['']
  });

  ngOnInit() {
    this.loadCategories();
    this.loadTodos();
  }

  get currentCategoryTitle(): string {
    if (!this.selectedCategoryId) return 'Усі завдання';
    return this.categories.find(c => c.id === this.selectedCategoryId)?.name || 'Завдання';
  }

  showToast(text: string, icon = '⚡', isError = false) {
    if (this.toastTimer) clearTimeout(this.toastTimer);
    this.toast = { text, icon, isError };
    this.cdr.detectChanges();
    this.toastTimer = setTimeout(() => {
      this.toast = null;
      this.cdr.detectChanges();
    }, 3000);
  }

  loadCategories() {
    this.todoService.getCategories().subscribe(res => {
      this.categories = res;
      if (this.categories.length === 0) this.initDefaultCategories();
      this.cdr.detectChanges();
    });
  }

  initDefaultCategories() {
    this.todoService.createCategory('Робота').subscribe(() => this.loadCategories());
    this.todoService.createCategory('Особисте').subscribe();
  }

  onSearch() {
    if (this.searchTimer) clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => {
      this.loadTodos();
    }, 350);
  }

  loadTodos() {
    this.todoService.getTodos(this.selectedCategoryId, this.searchQuery).subscribe(res => {
      this.todos = res;
      if (this.activeTodo) {
        this.activeTodo = this.todos.find(t => t.id === this.activeTodo?.id) || null;
      }
      this.cdr.detectChanges();
    });
  }

  selectCategory(id?: number) {
    this.selectedCategoryId = id;
    this.activeTodo = null;
    this.loadTodos();
  }

  selectTodo(item: TodoItem) {
    this.activeTodo = item;
    if (!this.activeTodo.subSteps) this.activeTodo.subSteps = [];

    this.todoService.getTodoById(item.id).subscribe(fullItem => {
      if (this.activeTodo && this.activeTodo.id === item.id) {
        this.activeTodo = fullItem;
        const idx = this.todos.findIndex(t => t.id === item.id);
        if (idx !== -1) this.todos[idx] = fullItem;
        this.cdr.detectChanges();
      }
    });
  }

  onAddTodo() {
    if (this.todoForm.invalid) return;
    const catId = this.selectedCategoryId || (this.categories[0]?.id ?? 1);
    const formVal = this.todoForm.value;

    const payload = {
      title: formVal.title,
      categoryId: catId,
      dueDate: formVal.dueDate || undefined
    };

    const tempId = Date.now();
    const optimisticItem: TodoItem = {
      id: tempId, title: payload.title, isCompleted: false, dueDate: payload.dueDate, categoryId: payload.categoryId, subSteps: []
    };

    this.todos = [optimisticItem, ...this.todos];
    this.todoForm.reset();
    this.showToast('Завдання створено', '✨');
    this.cdr.detectChanges();

    this.todoService.createTodo(payload).subscribe({
      next: (realItem) => {
        this.todos = this.todos.map(t => t.id === tempId ? realItem : t);
        this.cdr.detectChanges();
      },
      error: () => {
        this.todos = this.todos.filter(t => t.id !== tempId);
        this.showToast('Помилка збереження на сервері', '⚠️', true);
      }
    });
  }

  onUpdateTodoTitle() {
    if (!this.activeTodo || !this.activeTodo.title.trim()) return;

    this.todoService.updateTodo(this.activeTodo).subscribe({
      next: () => {
        this.showToast('Зміни збережено', '✏️');

        const idx = this.todos.findIndex(t => t.id === this.activeTodo?.id);
        if (idx !== -1 && this.activeTodo) this.todos[idx].title = this.activeTodo.title;
        this.cdr.detectChanges();
      },
      error: () => this.showToast('Не вдалося оновити назву', '❌', true)
    });
  }

  onToggle(id: number) {
    const item = this.todos.find(t => t.id === id);
    if (!item) return;
    item.isCompleted = !item.isCompleted;
    this.showToast(item.isCompleted ? 'Виконано!' : 'Повернуто в роботу', item.isCompleted ? '🎉' : '🔄');
    this.cdr.detectChanges();

    this.todoService.toggleTodo(id).subscribe({
      error: () => {
        item.isCompleted = !item.isCompleted;
        this.showToast('Помилка синхронізації', '❌', true);
      }
    });
  }

  onDelete(id: number) {
    const backup = [...this.todos];
    this.todos = this.todos.filter(t => t.id !== id);
    if (this.activeTodo?.id === id) this.activeTodo = null;
    this.showToast('Завдання видалено', '🗑️');
    this.cdr.detectChanges();

    this.todoService.deleteTodo(id).subscribe({
      error: () => {
        this.todos = backup;
        this.showToast('Помилка видалення', '❌', true);
      }
    });
  }

  onAddSubStep(event: Event) {
    event.preventDefault();
    if (!this.activeTodo || !this.newSubStepTitle.trim()) return;

    const title = this.newSubStepTitle.trim();
    this.newSubStepTitle = '';

    this.todoService.createSubStep(this.activeTodo.id, title).subscribe(step => {
      if (this.activeTodo) {
        if (!this.activeTodo.subSteps) this.activeTodo.subSteps = [];
        this.activeTodo.subSteps = [...this.activeTodo.subSteps, step];
        this.cdr.detectChanges();
      }
    });
  }

  onUpdateSubStep(step: SubStep) {
    if (!this.activeTodo || !step.title.trim()) return;

    this.todoService.updateSubStep(this.activeTodo.id, step).subscribe({
      next: () => this.showToast('Крок оновлено', '✏️'),
      error: () => this.showToast('Помилка оновлення кроку', '❌', true)
    });
  }

  onToggleSubStep(step: SubStep) {
    if (!this.activeTodo) return;
    step.isCompleted = !step.isCompleted;
    this.todoService.toggleSubStep(this.activeTodo.id, step.id).subscribe();
  }

  onDeleteSubStep(stepId: number) {
    if (!this.activeTodo || !this.activeTodo.subSteps) return;
    this.activeTodo.subSteps = this.activeTodo.subSteps.filter(s => s.id !== stepId);
    this.todoService.deleteSubStep(this.activeTodo.id, stepId).subscribe();
  }

  onAddCategory(event: Event) {
    event.preventDefault();
    if (!this.newCategoryTitle.trim()) {
      this.isCreatingCategory = false;
      return;
    }

    const name = this.newCategoryTitle.trim();
    this.newCategoryTitle = '';
    this.isCreatingCategory = false;

    this.todoService.createCategory(name).subscribe({
      next: (newCat) => {
        this.categories = [...this.categories, newCat];
        this.selectCategory(newCat.id);
        this.showToast('Список створено', '📁');
      },
      error: () => this.showToast('Не вдалося створити список', '❌', true)
    });
  }

  onDeleteCategory(catId: number, event: Event) {
    event.stopPropagation();
    const backup = [...this.categories];
    this.categories = this.categories.filter(c => c.id !== catId);

    if (this.selectedCategoryId === catId) {
      this.selectCategory(undefined);
    }

    this.showToast('Список видалено', '🗑️');
    this.cdr.detectChanges();

    this.todoService.deleteCategory(catId).subscribe({
      error: () => {
        this.categories = backup;
        this.showToast('Помилка видалення списку', '❌', true);
      }
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
