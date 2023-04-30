import { ModuleWithProviders, NgModule, Injectable, Inject, APP_INITIALIZER } from '@angular/core';
import { Routes, RouterModule, Router } from '@angular/router';
import { Http, Headers } from '@angular/http';
import { Observable } from "rxjs";

@Injectable()
export class DataService {
  private _data: any;
  private _baseUrl: string = '';

  constructor(private http: Http, @Inject('BASE_URL') baseUrl: string = '') {
    this._baseUrl = baseUrl;
  }

  getDataBundles(websiteLanguageId: number, callName: string): Observable<any> {
    return this.http
      .get(this._baseUrl + 'spine-api/data-bundles', {
        headers: new Headers(),
        params: {
          websiteLanguageId: websiteLanguageId,
          callName: callName
        }
      });
  }

  getDataBundle(websiteLanguageId: number, callName: string, url: string, breadcrumbs: boolean = false): Observable<any> {
    return this.http
      .get(this._baseUrl + 'spine-api/data-bundle', {
        headers: new Headers(),
        params: {
          websiteLanguageId: websiteLanguageId,
          callName: callName,
          url: url,
          breadcrumbs: breadcrumbs
        }
      });
  }

  getDataBundleWithCategories(websiteLanguageId: number, callName: string, url: string): Observable<any> {
    return this.http
      .get(this._baseUrl + 'spine-api/data-bundle-with-categories', {
        headers: new Headers(),
        params: {
          websiteLanguageId: websiteLanguageId,
          callName: callName,
          url: url
        }
      });
  }

  getDataBundlesWithCategories(websiteLanguageId: number, callName: string): Observable<any> {
    return this.http
      .get(this._baseUrl + 'spine-api/data-bundles-with-categories', {
        headers: new Headers(),
        params: {
          websiteLanguageId: websiteLanguageId,
          callName: callName
        }
      });
  }

  getDataBundleWithChilds(websiteLanguageId: number, callName: string, url: string, fieldCallName: string, type: string, breadcrumbs: boolean = false): Observable<any> {
    return this.http
      .get(this._baseUrl + 'spine-api/data-bundle-with-childs', {
        headers: new Headers(),
        params: {
          websiteLanguageId: websiteLanguageId,
          callName: callName,
          url: url,
          fieldCallName: fieldCallName,
          type: type,
          breadcrumbs: breadcrumbs
        }
      });
  }

  getMaxDataBundles(websiteLanguageId: number, callName: string, max: number): Observable<any> {
    return this.http
      .get(this._baseUrl + 'spine-api/max-data-bundles', {
        headers: new Headers(),
        params: {
          websiteLanguageId: websiteLanguageId,
          callName: callName,
          max: max
        }
      });
  }

  getMaxDataBundlesOrderByPublishDate(websiteLanguageId: number, callName: string, max: number): Observable<any> {
    return this.http
      .get(this._baseUrl + 'spine-api/max-data-bundles-order-by-publish-date', {
        headers: new Headers(),
        params: {
          websiteLanguageId: websiteLanguageId,
          callName: callName,
          max: max
        }
      });
  }
}
