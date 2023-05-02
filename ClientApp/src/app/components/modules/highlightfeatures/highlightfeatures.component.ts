import { Component, ElementRef, Directive, Input, HostBinding, ViewEncapsulation, QueryList, ViewChildren } from '@angular/core';
import { Res } from "../../../models/res.model";
import "owl.carousel";

declare let $: any;

@Component({
    selector: 'highlight-features',
    templateUrl: './highlightfeatures.component.html',
    styleUrls: ['../../../../../node_modules/owl.carousel/dist/assets/owl.carousel.min.css',
        './highlightfeatures.component.css'],
    encapsulation: ViewEncapsulation.None
})

export class HighlightFeaturesComponent {
    @Input() res = new Res();
    @Input() website: any = {};

    constructor() { }

  public initCarousel() {
    if (typeof $ !== 'undefined') {
      (<any>$('.highlight-features-carousel')).owlCarousel({
        margin: 0,
        autoWidth: true,
        nav: false,
        dots: false,
        responsive: {
          0: {
            items: 1,
            loop: true,
            autoplay: true
          },
          991: {
            items: 3,
            loop: false,
            autoplay: false
          }
        }
      });
    }
  }
}
