import { NgModule } from '@angular/core';
import { HighlightFeaturesModule } from '../../modules/highlightfeatures/highlightfeatures.module';
import { ConfirmationComponent } from './confirmation.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ConfirmationRoutingModule } from './confirmation.routing.module';
import { NgsRevealConfig, NgsRevealModule } from 'ngx-scrollreveal';

@NgModule({
  imports: [CommonModule, ConfirmationRoutingModule, RouterModule, HighlightFeaturesModule, NgsRevealModule],
  declarations: [ConfirmationComponent],
  providers: [
    NgsRevealConfig
  ]
})

export class ConfirmationModule {

}
