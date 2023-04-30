import { animate, query, stagger, style, transition, trigger } from '@angular/animations';
import { Component, Renderer2, Inject, ViewEncapsulation } from '@angular/core';
import { Http } from '@angular/http';
import { Meta } from '@angular/platform-browser';
import { ActivatedRoute, NavigationEnd, NavigationStart, Router } from '@angular/router';
import { NgsRevealConfig } from 'ngx-scrollreveal';
import { AppResolve } from "./app.resolve";
import { Res } from "./models/res.model";
import { CommerceService } from './services/commerce.service';
import { OrderService } from './services/order.service';
import { LazyLoadService } from './services/plugins/lazyload.service';
import { WebsiteService } from './services/website.service';
//import WebFont from "webfontloader";

declare let $: any;

@Component({
  selector: 'app',
  templateUrl: './app.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [
    trigger('routerAnimation', [
      transition('* <=> *', [
        // Initial state of new route
        query(':enter',
          style({
            position: 'absolute',
            width: '100%',
            opacity: 0
          }),
          { optional: true }),

        query(':leave',
          style({
            position: 'absolute',
            width: '100%',
            opacity: 1
          }),
          { optional: true }),

        // move page off screen right on leave
        query(':leave',
          stagger(500, [
            animate('500ms ease-in-out',
              style({ opacity: 0 })),
          ]),
          { optional: true }),

        // move page in screen from left to right
        query(':enter',
          stagger(500, [
            animate('500ms ease-in-out',
              style({ opacity: 1 })),
          ]),
          { optional: true }),
      ])
    ])
  ]
})

export class AppComponent {
  public res = new Res();
  public sr: any = null;
  private _baseUrl: string = '';

  get website() { return this.websiteService.getWebsiteData(); };

  constructor(private http: Http, private route: ActivatedRoute, private router: Router, private meta: Meta, private appResolve: AppResolve, revealConfig: NgsRevealConfig, private renderer: Renderer2, private commerceService: CommerceService, private websiteService: WebsiteService, private lazyLoadService: LazyLoadService, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;

    // customize default values of ng-scrollreveal directives used by this component tree
    revealConfig.duration = 450;
    revealConfig.easing = 'cubic-bezier(0.30, 0.045, 0.355, 1)';
    revealConfig.delay = 100;
    revealConfig.scale = 1;

    this.res = this.appResolve.getRes;

    //// Change resources when visitor switch language
    //this.router.events.subscribe((evt: any) => {
    //    if (evt instanceof ActivationStart) {
    //        var websiteLanguageId: number = evt.snapshot.data['websiteLanguageId'];
    //        if (this.res.websiteLanguageId != websiteLanguageId) {
    //
    //            appResolve.setWebsiteLanguageId(websiteLanguageId).then(function (result: any) {
    //                appResolve.setWebsiteBundle();
    //                appResolve.setNavigations();
    //            });
    //
    //            this.res = this.appResolve.getRes;
    //        }
    //    }
    //});
  }

  ngOnInit() {
    this.router.events.subscribe((evt) => {
      if (evt instanceof NavigationStart) {
        this.meta.removeTag('name="description"');
        this.meta.removeTag('name="keywords"');
        this.meta.removeTag('name="twitter:title"');
        this.meta.removeTag('name="twitter:description"');
        this.meta.removeTag('name="og:title"');
        this.meta.removeTag('name="og:description"');
      }

      if (!(evt instanceof NavigationEnd)) {
        return;
      }

      if (typeof window !== 'undefined') setTimeout(() => window.scrollTo(0, 0), 500);

      if (typeof $ !== 'undefined') {
        $('.navbar-collapse').collapse('hide');
      }
    });

    //WebFont.load({
    //  //classes: false,
    //  //events: false,
    //  custom: {
    //    families: ['Lato:n1,i1,n3,i3,n4,i4,n7,i7,n9,i9',
    //      'Frank Ruhl Libre:n3,n4,n5,n7,n9',
    //      'Montserrat:n1,i1,n2,i2,n3,i3,n4,i4,n5,i5,n6,i6,n7,i7,n8,i8,n9,i9']
    //  }
    //});

    //Get order for shopping cart
    new OrderService(this.http, this.commerceService, this._baseUrl).setOrder(this.res.websiteLanguageId);

    this.lazyLoadService.setLazyLoad();
  }

  public getRouteAnimation(outlet: any) {
    return outlet.activatedRouteData['pageId'];
  }
}

export function appResolveFactory(appResolve: AppResolve, websiteLanguageId: number = 0): Function {
  return async () => {
    await appResolve.setWebsiteLanguageId(websiteLanguageId).then(async function (result: any) {
      await appResolve.setWebsiteBundle();
      await appResolve.setNavigations();
      await appResolve.setPageUrls();
    });
  }; // => required, otherwise `this` won't work inside StartupService::load
}
