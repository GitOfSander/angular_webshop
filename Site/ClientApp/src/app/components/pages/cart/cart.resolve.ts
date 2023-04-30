import { Injectable, Inject } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, ActivatedRoute } from '@angular/router';
import { Subscription, Observable, forkJoin, of } from 'rxjs';
import { catchError, map } from "rxjs/operators";
import { PageService } from '../../../services/page.service';
import { Http } from "@angular/http";
import { DataService } from "../../../services/data.service";
import { WebsiteService } from "../../../services/website.service";
import { Meta, Title } from '@angular/platform-browser';
import { Res } from "../../../models/res.model"

@Injectable()
export class CartResolve implements Resolve<any> {
    private res = new Res();
    public pageId: number = 0;
    public websiteLanguageId: number = 0;
  private _baseUrl: string = '';

  constructor(private http: Http, private title: Title, private meta: Meta, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
   }

    resolve(route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<any> {
        this.pageId = route.data['pageId'];
        this.websiteLanguageId = route.data['websiteLanguageId'];

        return forkJoin([
          new PageService(this.http, this._baseUrl).getPageBundle(this.pageId, this.websiteLanguageId, true).pipe(
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
                catchError((error: any) => of(null)))
        ]).pipe(
            map((data: any[]) => {
                return this.res = {
                    data: {},
                    languages: {},
                    links: {},
                    navigations: {},
                    page: data[0],
                    reviews: {},
                    website: {},
                    websiteLanguageId: this.websiteLanguageId
                }
            }),
            catchError((error: any) => of(null)))
    };
}
