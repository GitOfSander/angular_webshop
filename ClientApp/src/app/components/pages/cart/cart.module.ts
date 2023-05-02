import { NgModule } from '@angular/core';
import { BreadcrumbsModule } from '../../modules/breadcrumbs/breadcrumbs.module';
import { HighlightFeaturesModule } from '../../modules/highlightfeatures/highlightfeatures.module';
import { CartComponent } from './cart.component';
import { FooterModule } from '../../modules/footer/footer.module';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartRoutingModule } from './cart.routing.module';
import { NgsRevealConfig, NgsRevealModule } from 'ngx-scrollreveal';

@NgModule({
  imports: [CommonModule, CartRoutingModule, RouterModule, BreadcrumbsModule, FooterModule, HighlightFeaturesModule, NgsRevealModule],
  declarations: [CartComponent],
  providers: [
    NgsRevealConfig
  ]
})

export class CartModule {
}
