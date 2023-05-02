import { Injectable, Inject } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, ActivatedRoute, Router } from '@angular/router';
import { Subscription, Observable, forkJoin, of } from 'rxjs';
import { catchError, map } from "rxjs/operators";
import { PageService } from '../../../services/page.service';
import { Http } from "@angular/http";
import { DataService } from "../../../services/data.service";
import { WebsiteService } from "../../../services/website.service";
import { Meta, Title } from '@angular/platform-browser';
import { Res } from "../../../models/res.model"
import { ProductService } from '../../../services/product.service';

@Injectable()
export class ProductResolve implements Resolve<any> {
  private res = new Res();
  public pageId: number = 0;
  public websiteLanguageId: number = 0;
  public alternateGuid: string = '';
  public itemUrl: string = '';
  private _baseUrl: string = '';

  constructor(private http: Http, private title: Title, private meta: Meta, private router: Router, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
  }

  resolve(route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> {
    this.pageId = route.data['pageId'];
    this.websiteLanguageId = route.data['websiteLanguageId'];
    this.alternateGuid = route.data['alternateGuid'];
    this.itemUrl = route.params['itemUrl'];

    return forkJoin([
      new ProductService(this.http, this._baseUrl).getProductBundle(this.websiteLanguageId, this.alternateGuid, this.itemUrl).pipe(
        map((data: any) => {
          this.title.setTitle(data.json().pageTitle);
          this.meta.addTags([
            { name: 'description', content: data.json().pageDescription },
            { name: 'keywords', content: data.json().pageKeywords },
            { name: 'twitter:title', content: data.json().pageTitle },
            { name: 'twitter:description', content: data.json().pageDescription },
            { name: 'og:title', content: data.json().pageTitle },
            { name: 'og:description', content: data.json().pageDescription }
          ]);

          return data.json();
        }),
        catchError((error: any) => {
          this.router.navigate([route.routeConfig.path.toString().replace('/:itemUrl', '')]);

          return of(null);
        })),
      new PageService(this.http, this._baseUrl).getPageBundle(this.pageId).pipe(
        map((data: any) => {
          return data.json();
        }),
        catchError((error: any) => of(null)))
    ]).pipe(
      map((data: any[]) => {
        return this.res = {
          data: {
            'product': data[0]
          },
          languages: {},
          links: {},
          navigations: {},
          page: data[1],
          reviews: {},
          website: {},
          websiteLanguageId: this.websiteLanguageId
        }
      }),
      catchError((error: any) => of(null)))
  };
}
