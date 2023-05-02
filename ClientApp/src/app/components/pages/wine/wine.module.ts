import { NgModule } from '@angular/core';
import { BreadcrumbsModule } from '../../modules/breadcrumbs/breadcrumbs.module';
import { HighlightFeaturesModule } from '../../modules/highlightfeatures/highlightfeatures.module';
import { WineComponent } from './wine.component';
import { FooterModule } from '../../modules/footer/footer.module';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { WineRoutingModule } from './wine.routing.module';
import { NgsRevealConfig, NgsRevealModule } from 'ngx-scrollreveal';

@NgModule({
  imports: [CommonModule, WineRoutingModule, RouterModule, BreadcrumbsModule, FooterModule, HighlightFeaturesModule, NgsRevealModule],
  declarations: [WineComponent],
  providers: [
    NgsRevealConfig
  ]
})

export class WineModule {

}
