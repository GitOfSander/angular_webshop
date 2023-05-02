import { ChangeDetectorRef, Component, ElementRef, Renderer2, ViewEncapsulation } from '@angular/core';
import { Http } from '@angular/http';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import "owl.carousel";
import { Res } from '../../../models/res.model';
import { CommerceService } from '../../../services/commerce.service';
import { OrderService } from '../../../services/order.service';
import { LazyLoadService } from '../../../services/plugins/lazyload.service';
import { WebsiteService } from '../../../services/website.service';
import { NgsRevealConfig } from 'ngx-scrollreveal';

declare let $: any;

@Component({
  selector: 'product',
  templateUrl: './product.component.html',
  styleUrls: ['../../../../../node_modules/owl.carousel/dist/assets/owl.carousel.min.css',
    './product.component.css'],
  encapsulation: ViewEncapsulation.None
})

export class ProductComponent {
  public res = new Res();
  public productRemoved: boolean = false;
  public vidHtml: any;
  public carHtml: any;
  public id: number = 0;
  public title: string = '';
  public image: any;
  public schema: any = [];
  public carousel: any;
  public buyDisabled: boolean = false;


  get website() { return this.websiteService.getWebsiteData(); };
  get checkoutUrl() { return this.commerceService.getCheckoutUrl(); }

  constructor(private http: Http, private route: ActivatedRoute, private renderer: Renderer2, private commerceService: CommerceService, private cdRef: ChangeDetectorRef, private elementRef: ElementRef, private websiteService: WebsiteService, private sanitizer: DomSanitizer, private lazyLoadService: LazyLoadService, revealConfig: NgsRevealConfig) {
    revealConfig.duration = 450;
    revealConfig.easing = 'cubic-bezier(0.30, 0.045, 0.355, 1)';
    revealConfig.delay = 100;
    revealConfig.scale = 1;

    if (typeof document !== 'undefined') {
      this.renderer.removeAttribute(document.body, 'class');
      this.renderer.addClass(document.body, 'product');
    }
  }

  ngOnInit() {
    this.res = this.route.snapshot.data['init'];

    this.setJsonLdScheme();
  }

  ngAfterViewInit() {
    this.route.data.subscribe((data: any) => {
      this.res = data['init'];

      $('.product-video').removeClass('play');
      this.setVideo();
      this.initVideo();
      this.initCarousel();

      this.lazyLoadService.update();
    });

    $('#productModal').on('hidden.bs.modal', function (e) {
      $('html').removeClass('modal-open');
    });

    if (typeof document !== 'undefined') this.renderer.setAttribute(document.body, 'data-loaded', 'true');
  }

  public setJsonLdScheme() {
    var product = this.res.data.product;
    var stock = "http://schema.org/InStock";
    if (product.manageStock && product.stockQuantity == 0 && product.backorders == 'no' || !product.manageStock && product.stockStatus == 'out') {
      var stock = "http://schema.org/OutOfStock";
    }

    var url = (typeof window !== 'undefined' ? window.location.href : '');
    this.schema.push({
      "@context": "http://schema.org",
      "@type": "Product",
      "name": product.resources.title,
      "image": product.files.pImage ? product.files.pImage[0].compressedPath : '',
      "url": url,
      "description": product.resources.description,
      "offers": {
        "@type": "Offer",
        "priceCurrency": product.currency,
        "price": product.priceClean,
        "priceValidUntil": product.promoTill != '' ? product.promoTill : null,
        "itemCondition": "http://schema.org/NewCondition",
        "availability": stock,
        "seller": {
          "@type": "Organization",
          "name": "DOMEIN.nl"
        }
      }
    });

    this.cdRef.detectChanges();
  }


  public async showModal(image: any, title: string, id: number) {
    if (!this.buyDisabled) {
      this.id = id;
      this.title = title;
      this.image = image;
      this.buyDisabled = true;
      this.cdRef.detectChanges();

      await this.addProduct(id);
      $('html').addClass('modal-open');
      $('#productModal').modal('show');
      this.buyDisabled = false;
      this.cdRef.detectChanges();
    }
  }

  //@ViewChildren('carouselInitCallback') carouselInitCallback: QueryList<any> = new QueryList<any>();

  public initCarousel() {
    if (this.carousel) {
      $('.product-carousel').trigger('destroy.owl.carousel').removeClass('owl-loaded');
      $('.product-carousel').find('.owl-stage-outer').children().unwrap();
    }

    if (!$('.product-carousel').hasClass('owl-loaded')) {
      var html: string = '';
      for (let key in this.res.data.product.files.image) {
        html += '<div class="item"><img class="owl-lazy" data-src="' + this.res.data.product.files.image[key].compressedPath + '" alt="' + this.res.data.product.files.image[key].alt + '" /></div>';
      }

      this.carHtml = this.sanitizer.bypassSecurityTrustHtml(html);
      this.cdRef.detectChanges();
    }

    this.carousel = (<any>$('.product-carousel')).owlCarousel({
      margin: 0,
      loop: true,
      navText: ["<svg version='1.1' id='Isolation_Mode' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' viewBox='0 0 6.4 11.4' style='enable-background:new 0 0 6.4 11.4;' xml:space='preserve'><path d='M0.7,11.2l-0.5-0.5l5-5l-5-5l0.5-0.5l5.6,5.5L0.7,11.2z'/></svg>",
        "<svg version='1.1' id='Isolation_Mode' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' viewBox='0 0 6.4 11.4' style='enable-background:new 0 0 6.4 11.4;' xml:space='preserve'><path d='M0.7,11.2l-0.5-0.5l5-5l-5-5l0.5-0.5l5.6,5.5L0.7,11.2z'/></svg>"],
      items: 1,
      nav: true,
      center: true,
      autoplay: 5000,
      lazyLoad: true
    });
  }

  public initVideo() {
    var self = this;
    $('#vid').on('ended', function (data: any) { self.restartVideo('#vid'); });

    //function myFunction(x: any) {
    //    if (x.matches) { 
    //        $('body').removeClass('desktop');
    //    } else {
    //        $('body').addClass('desktop');
    //    }
    //}
    //
    //var x = window.matchMedia("(max-width: 991.5px)")
    //myFunction(x) // Call listener function at run time
    //x.addListener(myFunction) // Attach listener function on state changes
  }

  public restartVideo(id: any) {
    $(id)[0].currentTime = .1, $(id)[0].play();
  }

  public setVideo() {
    if (this.res.data.product.files.video) {
      var video: string = '<video data-ignore id="vid" class="embed-responsive-item" muted playsinline loop poster="' + (this.res.data.product.files.poster ? this.res.data.product.files.poster[0].compressedPath : "") + '" title="' + (this.res.data.product.files.poster ? this.res.data.product.files.poster[0].alt : "") + '">';

      for (let key in this.res.data.product.files.video) {
        video += '<source src="' + this.res.data.product.files.video[key].originalPath + '" type="' + this.getVideoType(this.res.data.product.files.video[key].originalPath) + '">'
      }

      video += '</video>'

      this.vidHtml = this.sanitizer.bypassSecurityTrustHtml(video);
      this.cdRef.detectChanges();
    }
  }

  public getVideoType(file: any) {
    var extension: string = file.slice((file.lastIndexOf(".") - 1 >>> 0) + 2);

    switch (extension.toLowerCase()) {
      case 'ogv':
        return 'video/ogg';
      case 'mp4':
        return 'video/mp4';
      case 'webm':
        return 'video/webm';
      default:
        return '';
    }
  }

  public playVideo(index: any) {
    if ($('#vid')[0].paused) {
      $('#vid')[0].play();
      $('.product-video').addClass('play');
    } else {
      $('#vid')[0].pause();
      $('#vid')[0].currentTime = .1;
      $('.product-video').removeClass('play');
    }
  }

  public async addProduct(id: number) {
    await new OrderService(this.http, this.commerceService).addProductToOrder(id, this.res.websiteLanguageId).then((data: any) => {
      this.res.data.product.backorders = (data.backorders != null) ? data.backorders : this.res.data.product.backorders;
      this.res.data.product.maxPerOrder = (data.maxPerOrder != null) ? data.maxPerOrder : this.res.data.product.maxPerOrder;
      this.res.data.product.stockQuantity = (data.stockQuantity != null) ? data.stockQuantity : this.res.data.product.stockQuantity;
      this.productRemoved = (data.productRemoved != null) ? data.productRemoved : false;
      this.cdRef.detectChanges();
    });
  }
}
