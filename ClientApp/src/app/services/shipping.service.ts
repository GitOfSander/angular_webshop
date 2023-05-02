import { ModuleWithProviders, NgModule, Injectable, Inject, APP_INITIALIZER } from '@angular/core';
import { Routes, RouterModule, Router } from '@angular/router';
import { Http, Headers, RequestOptions } from '@angular/http';
import { Observable } from "rxjs";
import { CookieModule } from '../modules/cookie.module';
import { CommerceService } from './commerce.service';
import { OrderService } from './order.service';

@Injectable()
export class ShippingService {
  private _data: any;
  private _baseUrl: string = '';

    //private _promise: Promise<any>;
  constructor(private http: Http, private commerceService: CommerceService, @Inject('BASE_URL') baseUrl: string = '') {
    this._baseUrl = baseUrl;
  }

    public updateShippingMethods(shippingMethodId: number, reserveGuid: string): Observable<any> {
        return this.http
          .post(this._baseUrl + 'spine-api/shipping-method',
                {
                    shippingMethodId: shippingMethodId,
                    reserveGuid: reserveGuid
                },
                new RequestOptions({
                    headers: new Headers({
                        'Content-Type': 'application/json; charset=utf-8',
                    })
                }));
    }

    public setShippingMethod(shippingMethodId: number, websiteLanguageId: number) {
        var guid: any = CookieModule.read('cart');

        this.updateShippingMethods(shippingMethodId, (guid !== null) ? guid : '').subscribe((data: any) => {
          new OrderService(this.http, this.commerceService, this._baseUrl).setOrder(websiteLanguageId)
        });
    }
}
