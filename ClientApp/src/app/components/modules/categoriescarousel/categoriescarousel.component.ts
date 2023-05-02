import { Component, ElementRef, Directive, Input, HostBinding, ViewEncapsulation } from '@angular/core';
import { Res } from "../../../models/res.model";
import "owl.carousel";

declare let $: any;

@Component({
  selector: 'categories-carousel',
  templateUrl: './categoriescarousel.component.html',
  styleUrls: ['../../../../../node_modules/owl.carousel/dist/assets/owl.carousel.min.css', './categoriescarousel.component.css'],
  encapsulation: ViewEncapsulation.None
})

export class CategoriesCarouselComponent {
  @Input() res = new Res();

  constructor(private el: ElementRef) { }

  ngAfterViewInit() {
    this.initCarousel();
  }

  public initCarousel() {
    if (typeof $ !== 'undefined') {
      (<any>$('.categories-carousel')).owlCarousel({
        margin: 0,
        loop: true,
        lazyLoad: false,
        autoplay: 5000,
        navText: ["<svg version='1.1' id='Layer_1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' viewBox='0 0 12.3 3.9' style='enable-background:new 0 0 12.3 3.9;' xml:space='preserve'><g><path fill='#1B1B1B' d='M11.5,2.2H0.2V1.8h11.4l-1.4-1.4l0.3-0.3l1.7,1.7v0.3l-1.7,1.7l-0.3-0.3L11.5,2.2z'/></g></svg>", "<svg version='1.1' id='Layer_1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' viewBox='0 0 12.3 3.9' style='enable-background:new 0 0 12.3 3.9;' xml:space='preserve'><g><path fill='#1B1B1B' d='M11.5,2.2H0.2V1.8h11.4l-1.4-1.4l0.3-0.3l1.7,1.7v0.3l-1.7,1.7l-0.3-0.3L11.5,2.2z'/></g></svg>"],
        responsive: {
          0: {
            items: 1,
            nav: true,
            dots: true
          },
          591: {
            items: 2,
            nav: true,
            dots: true
          },
          991: {
            items: 3,
            nav: false,
            dots: false
          }
        }
      });
    }
  }
}
