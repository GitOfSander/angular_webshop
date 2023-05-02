import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { BreadcrumbsModule } from '../../modules/breadcrumbs/breadcrumbs.module';
import { HighlightFeaturesModule } from '../../modules/highlightfeatures/highlightfeatures.module';
import { CheckoutComponent } from './checkout.component';
import { FooterModule } from '../../modules/footer/footer.module';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CheckoutRoutingModule } from './checkout.routing.module';
import { NgsRevealConfig, NgsRevealModule } from 'ngx-scrollreveal';

@NgModule({
  imports: [CommonModule, RouterModule, BreadcrumbsModule, HighlightFeaturesModule, FooterModule, ReactiveFormsModule, CheckoutRoutingModule, NgsRevealModule],
  declarations: [CheckoutComponent],
  providers: [
    NgsRevealConfig
  ]
})

export class CheckoutModule {
}
