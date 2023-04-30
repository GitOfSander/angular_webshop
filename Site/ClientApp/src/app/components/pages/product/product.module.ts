import { NgModule } from '@angular/core';
import { NgxJsonLdModule } from '@ngx-lite/json-ld';
import { BreadcrumbsModule } from '../../modules/breadcrumbs/breadcrumbs.module';
import { HighlightFeaturesModule } from '../../modules/highlightfeatures/highlightfeatures.module';
import { ProductComponent } from './product.component';
import { FooterModule } from '../../modules/footer/footer.module';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductRoutingModule } from './product.routing.module';
import { NgsRevealConfig, NgsRevealModule } from 'ngx-scrollreveal';

@NgModule({
  imports: [CommonModule, ProductRoutingModule, RouterModule, BreadcrumbsModule, HighlightFeaturesModule, FooterModule, NgxJsonLdModule, NgsRevealModule],
  declarations: [ProductComponent],
  providers: [
    NgsRevealConfig
  ]
})

export class ProductModule {

}
