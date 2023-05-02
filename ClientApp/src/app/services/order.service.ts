import { ModuleWithProviders, NgModule, Injectable, Inject, APP_INITIALIZER, ChangeDetectorRef } from '@angular/core';
import { Routes, RouterModule, Router } from '@angular/router';
import { Http, Headers, RequestOptions } from '@angular/http';
import { Observable } from "rxjs";
import { CookieModule } from '../modules/cookie.module';
import { CommerceService } from './commerce.service';

@Injectable()
export class OrderService {
  private _data: any;
  private _baseUrl: string = '';

    //private _promise: Promise<any>;
  constructor(private http: Http, private commerceService: CommerceService = new CommerceService(http), @Inject('BASE_URL') baseUrl: string = '') {
    this._baseUrl = baseUrl;
  }

    public getOrderBundleByReserveGuid(reserveGuid: string, websiteLanguageId: number, lockPrices: boolean): Promise<Observable<any>> {
        return this.http
          .get(this._baseUrl + 'spine-api/order-bundle-by-reserve-guid', {
                headers: new Headers(),
                params: {
                    reserveGuid: reserveGuid,
                    websiteLanguageId: websiteLanguageId,
                    lockPrices: lockPrices
                }
            }).toPromise().then((data: any) => {
                return data;
            }).catch((data: any) => {
                return data;
            });
    }

    public GetOrderBillingEmailAndStatusByTransactionId(transactionId: string): Observable<any> {
        return this.http
          .get(this._baseUrl + 'spine-api/order-billing-email-and-status-by-transaction-id', {
                headers: new Headers(),
                params: {
                    transactionId: transactionId
                }
            });
    }

    public async insertOrderLine(productId: number, reserveGuid: string, quantity: number, increment: boolean, websiteLanguageId: number): Promise<Observable<any>> {
        return await this.http
          .post(this._baseUrl + 'spine-api/order-line',
                    {
                        productId: productId,
                        reserveGuid: reserveGuid,
                        quantity: quantity,
                        increment: increment,
                        websiteLanguageId: websiteLanguageId
                    },
                    new RequestOptions({
                        headers: new Headers({
                            'Content-Type': 'application/json; charset=utf-8',
                        })
                })).toPromise().then((data: any) => {
                    return data;
                }).catch((data: any) => {
                    return data;
                });
    }

    public deleteOrderLine(productId: number, reserveGuid: string, websiteLanguageId: number): Observable<any> {
        return this.http
          .post(this._baseUrl + 'spine-api/delete-order-line',
                {
                    productId: productId,
                    reserveGuid: reserveGuid,
                    websiteLanguageId: websiteLanguageId
                },
                new RequestOptions({
                    headers: new Headers({
                        'Content-Type': 'application/json; charset=utf-8',
                    })
                }));
    }

    public async addProductToOrder(id: number, websiteLanguageId: number, quantity: number = 1, increment: boolean = true): Promise<Observable<any>> {
        var guid: any = CookieModule.read('cart');

        return await this.insertOrderLine(id, (guid !== null) ? guid : '', quantity, increment, websiteLanguageId).then((data: any) => {
            var res: any = data.json();

            CookieModule.write('cart', res.order.guid, 365);
            this.commerceService.setOrder(res.order);

          return res;
        }).catch((data: any) => {
          this.setOrder(websiteLanguageId, false);
          return data.json();
        });
    }

    public deleteProductFromOrder(id: number, websiteLanguageId: number) {
        var guid: any = CookieModule.read('cart');

        this.deleteOrderLine(id, (guid !== null) ? guid : '', websiteLanguageId).subscribe((data: any) => {
          this.commerceService.setOrder(data.json());
          },
          (error: any) => {
            this.setOrder(websiteLanguageId, false);
          }
        )
    }

    public async setOrder(websiteLanguageId: number, lockPrices: boolean = false): Promise<any>{
        var guid: any = CookieModule.read('cart');

        return await this.getOrderBundleByReserveGuid(guid !== null ? guid : '', websiteLanguageId, lockPrices).then((data: any) => {
          this.commerceService.setOrder(data.json());
        });
    }
}
