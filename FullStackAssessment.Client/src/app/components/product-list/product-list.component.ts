import { Component, inject, signal, computed, OnInit, DestroyRef } from '@angular/core';
import { Subject } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CurrencyPipe } from '@angular/common';
import { ProductService } from '../../services/product.service';
import { Product } from '../../product.model';
import { ProductFormComponent } from '../product-form/product-form.component';
import { RouterLink } from '@angular/router';
import { NotificationService } from '../../services/notification.service';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [FormsModule, CurrencyPipe, ProductFormComponent, RouterLink],
  templateUrl: './product-list.component.html'
})
export class ProductListComponent {
  private productService = inject(ProductService);
  private notificationService = inject(NotificationService);
  private searchSubject = new Subject<string>();
  private destroyRef = inject(DestroyRef);

  products = signal<Product[]>([]);
  searchTerm = signal('');
  loading = signal(true);
  sortColumn = signal<keyof Product | null>(null);
  sortDirection = signal<'asc' | 'desc'>('asc');
  currentPage = signal(1);
  pageSize = 5;
  showQuickAddModal = signal(false);
  selectedCategory = signal<string>('');
  productTypes = signal<{id: number, name: string}[]>([]);

  filteredProducts = computed(() => {
  const term = this.searchTerm().toLowerCase();
  const cat = this.selectedCategory();

  let data = this.products().filter(p => {
    const matchesSearch = p.name.toLowerCase().includes(term) ||
                          p.description.toLowerCase().includes(term);
                          
    const matchesCategory = cat === '' || p.productTypeId?.toString() === cat;

    return matchesSearch && matchesCategory;
  });

  const column = this.sortColumn();
  if (column) {
    data = [...data].sort((a, b) => {
      const valA = a[column];
      const valB = b[column];
      if (valA < valB) return this.sortDirection() === 'asc' ? -1 : 1;
      if (valA > valB) return this.sortDirection() === 'asc' ? 1 : -1;
      return 0;
    });
  }
  
  return data;
});

  paginatedProducts = computed(() => {
    const start = (this.currentPage() - 1) * this.pageSize;
    return this.filteredProducts().slice(start, start + this.pageSize);
  });

  constructor() {
    this.productService.getProducts().subscribe(data => {
      this.products.set(data);
      this.loading.set(false);
    });
  }
  
  ngOnInit(): void {
    this.loadProducts();

    this.productService.getProductTypes().subscribe(types => {
      this.productTypes.set(types);
    });

    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntilDestroyed(this.destroyRef) // prevent memory leaks
    ).subscribe(value => {
      this.searchTerm.set(value);
    });
  }

  setSort(column: keyof Product) {
    if (this.sortColumn() === column) {
      this.sortDirection.update(d => d === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column);
      this.sortDirection.set('asc');
    }
  }

  onSearchChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchSubject.next(input.value);
  }

  resetSearch() {
    this.searchTerm.set('');
    this.currentPage.set(1);
    this.sortColumn.set(null);
    this.searchSubject.next('');
    this.selectedCategory.set('');
  }

  nextPage() {
    if (this.currentPage() < Math.ceil(this.filteredProducts().length / this.pageSize)) this.currentPage.update(v => v + 1);
  }

  prevPage() {
    if (this.currentPage() > 1) this.currentPage.update(v => v - 1);
  }

  deleteProduct(id: number): void {
  const confirmDelete = confirm('Are you sure you want to delete this product? This action cannot be undone.');
  
  if (confirmDelete) {
    this.productService.deleteProduct(id).subscribe({
      next: () => {
        this.loadProducts();
        this.notificationService.showSuccess('Product deleted successfully.');
      },
      error: (err) => console.error('Error deleting product:', err)
    });
  }
}

  toggleModal() {
    this.showQuickAddModal.update(v => !v);
  }

  onProductSaved() {
    this.showQuickAddModal.set(false);
    this.loadProducts();
  }

  loadProducts() {
    this.loading.set(true);
    this.productService.getProducts().subscribe(data => {
      this.products.set(data);
      this.loading.set(false);
    });
  }
}