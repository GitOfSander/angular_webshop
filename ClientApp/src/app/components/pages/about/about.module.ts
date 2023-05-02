import { NgModule } from '@angular/core';
import { BreadcrumbsModule } from '../../modules/breadcrumbs/breadcrumbs.module';
import { CategoriesWhiteComponent } from '../../modules/categorieswhite/categorieswhite.component';
import { HighlightFeaturesModule } from '../../modules/highlightfeatures/highlightfeatures.module';
import { AboutComponent } from './about.component';
import { CommonModule } from '@angular/common';
import { FooterModule } from '../../modules/footer/footer.module';
import { RouterModule } from '@angular/router';
import { AboutRoutingModule } from './about.routing.module';
import { NgsRevealConfig, NgsRevealModule } from 'ngx-scrollreveal';

@NgModule({
  imports: [CommonModule, AboutRoutingModule, RouterModule, BreadcrumbsModule, FooterModule, HighlightFeaturesModule, NgsRevealModule],
  exports: [CategoriesWhiteComponent],
  declarations: [AboutComponent, CategoriesWhiteComponent],
  providers: [
    NgsRevealConfig
  ]
})

export class AboutModule {

}
