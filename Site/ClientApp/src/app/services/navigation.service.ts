import { ModuleWithProviders, NgModule, Injectable, Inject, APP_INITIALIZER } from '@angular/core';
import { Routes, RouterModule, Router } from '@angular/router';
import { Http, Headers } from '@angular/http';
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root',
})
export class NavigationService {
    public navigations: any;
  private _baseUrl: string = '';

  constructor(private http: Http, @Inject('BASE_URL') baseUrl: string = '') {
    this._baseUrl = baseUrl;

        this.navigations = {};
    }

    public setNavigationsData(val: any) {
        this.navigations = val;
    }

    public getNavigationsData() {
        return this.navigations;
    }

    public getNavigation(websiteLanguageId: number, callName: string): Observable<any> {
        return this.http
          .get(this._baseUrl + 'spine-api/navigation', {
                headers: new Headers(),
                params: {
                    websiteLanguageId: websiteLanguageId,
                    callName: callName
                }
            });
    }

    public getNavigations(websiteLanguageId: number, callNames: string[]): Promise<Observable<any>> {
        return this.http
          .get(this._baseUrl + 'spine-api/navigations', {
                headers: new Headers(),
                params: {
                    websiteLanguageId: websiteLanguageId,
                    callNames: callNames
                }
            }).toPromise().then((data: any) => {
                return data;
            }).catch((data: any) => {
                return data;
            });
    }

    public async setNavigations(websiteLanguageId: number, callNames: string[]): Promise<any> {
        return await this.getNavigations(websiteLanguageId, callNames).then((data: any) => {
            this.setNavigationsData(data.json());
        });
    }
}
