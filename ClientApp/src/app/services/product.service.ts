import { ModuleWithProviders, NgModule, Injectable, Inject, APP_INITIALIZER } from '@angular/core';
import { Routes, RouterModule, Router } from '@angular/router';
import { Http, Headers } from '@angular/http';
import { Observable } from "rxjs";

@Injectable()
export class ProductService {
  private _data: any;
  private _baseUrl: string = '';

    //private _promise: Promise<any>;
  constructor(private http: Http, @Inject('BASE_URL') baseUrl: string = '') {
    this._baseUrl = baseUrl;
  }

    getProductBundle(websiteLanguageId: number, alternateGuid: string, url: string): Observable<any> {
        return this.http
          .get(this._baseUrl + 'spine-api/product-bundle', {
                headers: new Headers(),
                params: {
                    websiteLanguageId: websiteLanguageId,
                    alternateGuid: alternateGuid,
                    url: url
                }
            });
    }

    getProductBundles(websiteLanguageId: number, alternateGuid: string): Observable<any> {
        return this.http
          .get(this._baseUrl + 'spine-api/product-bundles', {
                headers: new Headers(),
                params: {
                    websiteLanguageId: websiteLanguageId,
                    alternateGuid: alternateGuid
                }
            });
    }
}
