import { ModuleWithProviders, NgModule, Injectable, Inject, APP_INITIALIZER } from '@angular/core';
import { Routes, RouterModule, Router } from '@angular/router';
import { Http, Headers } from '@angular/http';
import { Observable } from "rxjs";

@Injectable()
export class ReviewService {
  private _data: any;
  private _baseUrl: string = '';


  constructor(private http: Http, @Inject('BASE_URL') baseUrl: string = '') {
    this._baseUrl = baseUrl;
  }

    getReviewBundles(websiteId: number, callName: string): Observable<any> {
        return this.http
          .get(this._baseUrl + 'spine-api/review-bundles', {
                headers: new Headers(),
                params: {
                    websiteId: websiteId,
                    callName: callName
                }
            });
    }
}
