import { ModuleWithProviders, NgModule, Injectable, Inject, APP_INITIALIZER } from '@angular/core';
import { Routes, RouterModule, Router } from '@angular/router';
import { Http, Headers } from '@angular/http';
import { Observable } from "rxjs";

@Injectable()
export class PageService {
  private _data: any;
  private _baseUrl: string = '';

  //private _promise: Promise<any>;

  constructor(private http: Http, @Inject('BASE_URL') baseUrl: string = '') {
    this._baseUrl = baseUrl;
  }

  getPageBundle(pageId: number, websiteLanguageId: number = 0, breadcrumbs: boolean = false, addProductJson: boolean = false): Observable<any> {
    return this.http
      .get(this._baseUrl + 'spine-api/page-bundle', {
        headers: new Headers(),
        params: {
          pageId: pageId,
          websiteLanguageId: websiteLanguageId,
          breadcrumbs: breadcrumbs,
          addProductJson: addProductJson
        }
      });
  }

  getPageBundlesByType(websiteLanguageId: number, type: string, addProductJson: boolean = false): Observable<any> {
    return this.http
      .get(this._baseUrl + 'spine-api/page-bundles-by-type', {
        headers: new Headers(),
        params: {
          websiteLanguageId: websiteLanguageId,
          type: type,
          addProductJson: addProductJson
        }
      });
  }

  getPageUrlsByAlternateGuidsAndSettingValues(websiteLanguageId: number, alternateGuids: string[], settingValues: string[] = []): Observable<any> {
    return this.http
      .get(this._baseUrl + 'spine-api/page-urls-by-alternate-guids-and-setting-values', {
        headers: new Headers(),
        params: {
          websiteLanguageId: websiteLanguageId,
          alternateGuids: alternateGuids,
          settingValues: settingValues
        }
      });
  }
}
