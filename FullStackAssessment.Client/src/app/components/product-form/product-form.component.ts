import { Component, inject, OnInit, signal, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './product-form.component.html'
})
export class ProductFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private notificationService = inject(NotificationService);

  @Output() formSaved = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  productForm: FormGroup;
  isEditMode = signal(false);
  productId: number | null = null;
  productTypes = signal<{id: number, name: string}[]>([]);

  constructor() {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      price: [0, Validators.required],
      productTypeId: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.productService.getProductTypes().subscribe(types => {
      this.productTypes.set(types);
    });

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.productId = Number(id);
      
      this.productService.getProductById(this.productId).subscribe(product => {
        this.productForm.patchValue(product);
      });
    }
  }

  saveProduct() {
    if (this.productForm.valid) {
      const productData = this.productForm.value;

      const saveAction: Observable<any> = this.isEditMode() && this.productId
        ? this.productService.updateProduct(this.productId, productData)
        : this.productService.createProduct(productData);

      saveAction.subscribe({
        next: () => {
          this.notificationService.showSuccess(this.isEditMode() ? 'Product updated successfully.' : 'Product created successfully.');
          this.formSaved.emit();
          this.goBack();
        },
        error: (err) => {
          this.notificationService.showError('Failed to save product.');
          console.error(err);
        }
      })
    }
  }

  goBack() {
    this.cancel.emit();
    this.router.navigate(['/']);
  }
}