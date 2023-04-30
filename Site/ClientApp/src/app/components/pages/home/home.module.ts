import { NgModule } from '@angular/core';
import { CategoriesCarouselComponent } from '../../modules/categoriescarousel/categoriescarousel.component';
import { HeaderComponent } from '../../modules/header/header.component';
import { HighlightFeaturesModule } from '../../modules/highlightfeatures/highlightfeatures.module';
import { CommonModule } from '@angular/common';
import { FooterModule } from '../../modules/footer/footer.module';
import { RouterModule } from '@angular/router';
import { HomeRoutingModule } from './home.routing.module';
import { HomeComponent } from './home.component';
import { NgsRevealConfig, NgsRevealModule } from 'ngx-scrollreveal';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    FooterModule,
    HighlightFeaturesModule,
    NgsRevealModule,
    HomeRoutingModule
  ],
  exports: [
    CategoriesCarouselComponent,
    HeaderComponent
  ],
  declarations: [
    HomeComponent,
    CategoriesCarouselComponent,
    HeaderComponent
  ],
  providers: [
    NgsRevealConfig
  ]
})
export class HomeModule {
}
