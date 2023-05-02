import { NgModule } from '@angular/core';
import { ProductsComponent } from './products.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@NgModule({
  imports: [CommonModule, RouterModule],
  exports: [ProductsComponent],
  declarations: [ProductsComponent]
})

export class ProductsModule {

}
