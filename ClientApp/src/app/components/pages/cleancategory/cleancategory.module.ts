import { NgModule } from '@angular/core';
import { BreadcrumbsModule } from '../../modules/breadcrumbs/breadcrumbs.module';
import { HighlightFeaturesModule } from '../../modules/highlightfeatures/highlightfeatures.module';
import { ProductsModule } from '../../modules/products/products.module';
import { CleanCategoryComponent } from './cleancategory.component';
import { FooterModule } from '../../modules/footer/footer.module';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CleanCategoryRoutingModule } from './cleancategory.routing.module';
import { NgsRevealConfig, NgsRevealModule } from 'ngx-scrollreveal';

@NgModule({
  imports: [CommonModule, CleanCategoryRoutingModule, RouterModule, BreadcrumbsModule, FooterModule, HighlightFeaturesModule, ProductsModule, NgsRevealModule],
  declarations: [CleanCategoryComponent],
  providers: [
    NgsRevealConfig
  ]
})

export class CleanCategoryModule {

}
