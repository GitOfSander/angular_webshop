import { Component, ElementRef, Directive, Input, HostBinding, Inject, Injectable, EventEmitter } from '@angular/core';
import { Router, ActivationStart, ActivatedRouteSnapshot, RouterStateSnapshot, ActivatedRoute } from '@angular/router';
import { Res } from '../models/res.model';
import { AppResolve } from "../app.resolve";
import { OrderService } from './order.service'
import { CookieModule } from '../modules/cookie.module';
import { Http, Headers } from '@angular/http';
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root',
})
export class CommerceService {
    public order: any;
    public cartUrl: string;
    public checkoutUrl: string;
  public continueShoppingUrl: string;
  private _baseUrl: string = '';


  constructor(private http: Http, @Inject('BASE_URL') baseUrl: string = '') {
    this._baseUrl = baseUrl;

        this.order = {};
        this.cartUrl = '';
        this.checkoutUrl = '';
        this.continueShoppingUrl = '';
    }

    public getIssuers(): Observable<any> {
        return this.http
          .get(this._baseUrl + 'spine-api/issuers', {
                headers: new Headers()
            });
    }

    //Cart
    public setOrder(val: any) {
        this.order = val;
    }

    public getOrder() {
        return this.order;
    }

    //Cart url
    public setCartUrl(val: string) {
        this.cartUrl = val;
    }

    public getCartUrl() {
        return this.cartUrl;
    }

    //Checkout url
    public setCheckoutUrl(val: string) {
        this.checkoutUrl = val;
    }

    public getCheckoutUrl() {
        return this.checkoutUrl;
    }

    //Continue shopping url
    public setContinueShoppingUrl(val: string) {
        this.continueShoppingUrl = val;
    }

    public getContinueShoppingUrl() {
        return this.continueShoppingUrl;
    }
}
