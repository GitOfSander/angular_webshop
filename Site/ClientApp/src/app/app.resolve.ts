import { Injectable, Inject } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, ActivatedRoute } from '@angular/router';
import { PageService } from './services/page.service';
import { Http } from "@angular/http";
import { DataService } from "./services/data.service";
import { WebsiteService } from "./services/website.service";
import { Meta, Title } from '@angular/platform-browser';
import { Res } from "./models/res.model"
import { ReviewService } from "./services/review.service";
import { map } from 'rxjs/operators';
import { CommerceService } from './services/commerce.service';
import { OrderService } from './services/order.service';
import { NavigationService } from './services/navigation.service';

@Injectable()
export class AppResolve {
  private res = new Res();
  private _baseUrl: string = '';

  constructor(private http: Http, private commerceService: CommerceService, private websiteService: WebsiteService, private navigationService: NavigationService, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
  }

  setWebsiteLanguageId(websiteLanguageId: number): Promise<any> {
    if (websiteLanguageId == 0) {
      return new WebsiteService(this.http, this._baseUrl).getWebsiteLanguageByDefaultLanguage(true)
        .pipe(map((res: Response) => res.json()))
        .toPromise()
        .then((data: any) => {
          this.res.websiteLanguageId = data.id;
        })
        .catch((err: any) => Promise.resolve());
    }

    this.res.websiteLanguageId = websiteLanguageId;

    return new Promise<any>(resolve => resolve(null));
  }

  public setPageUrls(): Promise<any> {
    return new PageService(this.http, this._baseUrl).getPageUrlsByAlternateGuidsAndSettingValues(this.res.websiteLanguageId, [], ['cart', 'checkout', 'continueShopping'])
      .pipe(map((res: Response) => res.json()))
      .toPromise()
      .then((data: any) => {
        this.commerceService.setCartUrl(data.cart)
        this.commerceService.setCheckoutUrl(data.checkout)
        this.commerceService.setContinueShoppingUrl(data.continueShopping)
      })
      .catch((err: any) => Promise.resolve());
  }

  public setNavigations(): Promise<any> {
    return this.navigationService.setNavigations(this.res.websiteLanguageId, ['mainNav']);
  }

  public setWebsiteBundle(): Promise<any> {
    return this.websiteService.setWebsiteBundle(this.res.websiteLanguageId);
  }

  public setOrder(): Promise<any> {
    return new OrderService(this.http, this.commerceService, this._baseUrl).setOrder(this.res.websiteLanguageId);
  }

  get getRes(): Res {
    return this.res;
  }
}
