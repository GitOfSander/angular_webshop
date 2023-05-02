import { Injectable, Inject } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, ActivatedRoute, ParamMap } from '@angular/router';
import { Subscription, Observable, forkJoin, of } from 'rxjs';
import { catchError, map } from "rxjs/operators";
import { PageService } from '../../../services/page.service';
import { Http } from "@angular/http";
import { DataService } from "../../../services/data.service";
import { WebsiteService } from "../../../services/website.service";
import { Meta, Title } from '@angular/platform-browser';
import { Res } from "../../../models/res.model"
import { OrderService } from '../../../services/order.service';
import { CommerceService } from '../../../services/commerce.service';

@Injectable()
export class ConfirmationResolve implements Resolve<any> {
  private _baseUrl: string = '';

  constructor(private http: Http, private title: Title, private meta: Meta, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
   }

    resolve(route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<any> {
        var res = new Res();
        var pageId = route.data['pageId'];
        var websiteLanguageId = route.data['websiteLanguageId'];
        var transactionId = route.queryParams['id'];

        return forkJoin([
          new PageService(this.http, this._baseUrl).getPageBundle(pageId).pipe(
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
          new OrderService(this.http, new CommerceService(this.http), this._baseUrl).GetOrderBillingEmailAndStatusByTransactionId(transactionId).pipe(
                map((data: any) => {
                    return data.json();
                }),
                catchError((error: any) => of(null))),
          new PageService(this.http, this._baseUrl).getPageUrlsByAlternateGuidsAndSettingValues(websiteLanguageId, ['a9965b7e-27f6-435b-b033-25c5a8146609'], []).pipe(
                map((data: any) => {
                    return data.json();
                }),
                catchError((error: any) => of(null))),
        ]).pipe(
            map((data: any[]) => {
                return res = {
                    data: {
                        'order': data[1]
                    },
                    languages: {},
                    links: data[2],
                    navigations: {},
                    page: data[0],
                    reviews: {},
                    website: {},
                    websiteLanguageId: 0
                }
            }),
            catchError((error: any) => of(null)))
    };
}
