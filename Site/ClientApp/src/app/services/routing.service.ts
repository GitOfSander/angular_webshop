import { ModuleWithProviders, NgModule, Injectable, Inject, APP_INITIALIZER } from '@angular/core';
import { Routes, RouterModule, Router } from '@angular/router';
import { Http, Headers } from '@angular/http';
import { Observable } from "rxjs";

@Injectable()
export class RoutingService {
  private _baseUrl: string = '';

  constructor(private http: Http, @Inject('BASE_URL') baseUrl: string = '') {
    this._baseUrl = baseUrl;
  }

    getBreadcrumbs(pageId: number): Observable<any> {
        return this.http
          .get(this._baseUrl + 'spine-api/breadcrumbs', {
                headers: new Headers(),
                params: {
                    pageId: pageId
                }
            });
    }

    getRouteByAlternateGuidAndWebsiteLanguageId(alternateGuid: string, websiteLanguageId: number): Observable<any> {
        return this.http
          .get(this._baseUrl + 'spine-api/route-by-alternate-guid-and-website-language-id', {
                headers: new Headers(),
                params: {
                    alternateGuid: alternateGuid,
                    websiteLanguageId: websiteLanguageId
                }
            });
    }
}
