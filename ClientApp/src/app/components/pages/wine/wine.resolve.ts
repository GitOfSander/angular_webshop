import { Injectable, Inject } from '@angular/core';
import { Http } from "@angular/http";
import { Meta, Title } from '@angular/platform-browser';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot, Router } from '@angular/router';
import { Observable, forkJoin, of } from 'rxjs';
import { catchError, map } from "rxjs/operators";
import { Res } from "../../../models/res.model";
import { DataService } from "../../../services/data.service";

@Injectable()
export class WineResolve implements Resolve<any> {
  private res = new Res();
  public pageId: number = 0;
  public websiteLanguageId: number = 0;
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
    this.itemUrl = route.params['itemUrl'];

    return forkJoin([
      new DataService(this.http, this._baseUrl).getDataBundleWithChilds(this.websiteLanguageId, 'wineCategories', this.itemUrl, 'wineCat', 'selectlinkedto', true).pipe(
        map((data: any) => {
          this.title.setTitle(data.json().wineCategories.pageTitle);
          this.meta.addTags([
            { name: 'description', content: data.json().wineCategories.pageDescription },
            { name: 'keywords', content: data.json().wineCategories.pageKeywords },
            { name: 'twitter:title', content: data.json().wineCategories.pageTitle },
            { name: 'twitter:description', content: data.json().wineCategories.pageDescription },
            { name: 'og:title', content: data.json().wineCategories.pageTitle },
            { name: 'og:description', content: data.json().wineCategories.pageDescription }
          ]);

          return data.json();
        }),
        catchError((error: any) => {
          this.router.navigate([route.routeConfig.path.toString().replace('/:itemUrl', '')]);

          return of(null);
        }))
    ]).pipe(
      map((data: any[]) => {
        return this.res = {
          data: {
            'wineCategory': data[0].wineCategories,
            'wines': data[0].wineCat
          },
          languages: {},
          links: {},
          navigations: {},
          page: {},
          reviews: {},
          website: {},
          websiteLanguageId: 0
        }
      }),
      catchError((error: any) => of(null)))
  };
}
