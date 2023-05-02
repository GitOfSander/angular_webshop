import { Injectable, Inject } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, ActivatedRoute } from '@angular/router';
import { Subscription, Observable, forkJoin, of } from 'rxjs';
import { catchError, map } from "rxjs/operators";
import { PageService } from '../../../services/page.service';
import { Http } from "@angular/http";
import { Meta, Title } from '@angular/platform-browser';
import { Res } from "../../../models/res.model"
import { OrderService } from '../../../services/order.service';
import { CookieModule } from '../../../modules/cookie.module';
import { CommerceService } from '../../../services/commerce.service';

@Injectable()
export class CheckoutResolve implements Resolve<any> {
    private res = new Res();
  private _baseUrl: string = '';

  constructor(private http: Http, private title: Title, private meta: Meta, private commerceService: CommerceService, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
   }

    resolve(route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<any> {
        var pageId: number = route.data['pageId'];
        var websiteLanguageId: number = route.data['websiteLanguageId'];
        var guid: any = CookieModule.read('cart');

        return forkJoin([
          new PageService(this.http, this._baseUrl).getPageBundle(pageId, websiteLanguageId, true).pipe(
                map((data: any) => {
                    this.title.setTitle(data.json().title);
                    this.meta.addTags([
                        { name: 'description', content: data.json().description },
                        { name: 'keywords', content: data.json().keywords },
                        { name: 'twitter:title', content: data.json().title },
                        { name: 'twitter:description', content: data.json().description },
                        { name: 'og:title', content: data.json().title },
                        { name: 'og:description', content: data.json().description }
                    ]);

                    return data.json();
                }),
                catchError((error: any) => of(null))),
          new CommerceService(this.http, this._baseUrl).getIssuers().pipe(
                map((data: any) => {
                    return data.json();
                }),
                catchError((error: any) => of(null)))
        ]).pipe(
            map((data: any[]) => {
                return this.res = {
                    data: {
                        "issuers": data[1]
                    },
                    languages: {},
                    links: {},
                    navigations: {},
                    page: data[0],
                    reviews: {},
                    website: {},
                    websiteLanguageId: websiteLanguageId
                }
            }),
            catchError((error: any) => of(null)))
    };
}
