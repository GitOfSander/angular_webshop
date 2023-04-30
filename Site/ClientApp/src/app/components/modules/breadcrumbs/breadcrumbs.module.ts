import { NgModule } from '@angular/core';
import { BreadcrumbsComponent } from './breadcrumbs.component';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@NgModule({
  imports: [CommonModule, RouterModule],
  exports: [BreadcrumbsComponent],
  declarations: [BreadcrumbsComponent]
})

export class BreadcrumbsModule {

}
