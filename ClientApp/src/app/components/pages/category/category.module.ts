import { NgModule } from '@angular/core';
import { BreadcrumbsModule } from '../../modules/breadcrumbs/breadcrumbs.module';
import { HighlightFeaturesModule } from '../../modules/highlightfeatures/highlightfeatures.module';
import { ProductsModule } from '../../modules/products/products.module';
import { CategoryComponent } from './category.component';
import { FooterModule } from '../../modules/footer/footer.module';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CategoryRoutingModule } from './category.routing.module';
import { NgsRevealConfig, NgsRevealModule } from 'ngx-scrollreveal';

@NgModule({
  imports: [CommonModule, CategoryRoutingModule, RouterModule, BreadcrumbsModule, HighlightFeaturesModule, FooterModule, ProductsModule, NgsRevealModule],
  declarations: [CategoryComponent],
  providers: [
    NgsRevealConfig
  ]
})

export class CategoryModule {
}
