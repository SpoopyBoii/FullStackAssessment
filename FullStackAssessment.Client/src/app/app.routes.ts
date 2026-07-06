import { Routes } from '@angular/router';
import { ProductListComponent } from './components/product-list/product-list.component';
import { ProductFormComponent } from './components/product-form/product-form.component';

export const routes: Routes = [
  { path: '', component: ProductListComponent },
  { path: 'products', component: ProductListComponent },
  { path: 'add-product', component: ProductFormComponent },
  { path: 'edit/:id', component: ProductFormComponent },
  { path: '**', redirectTo: '' }
];