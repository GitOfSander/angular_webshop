import { ModuleWithProviders, NgModule, Injectable, Inject, APP_INITIALIZER } from '@angular/core';
import { Routes, RouterModule, Router } from '@angular/router';
import { Http, Headers } from '@angular/http';
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root',
})
export class WebsiteService {
  public website: any;
  public languages: any;
  public languageSet: boolean = false;
  private _baseUrl: string = '';

  constructor(private http: Http, @Inject('BASE_URL') baseUrl: string = '') {
    this._baseUrl = baseUrl;

    this.website = {};
    this.languages = {};
  }

  public setWebsiteData(val: any) {
    this.website = val;
  }

  public getWebsiteData() {
    return this.website;
  }

  public setLanguagesData(val: any) {
    this.languages = val;
    this.languageSet = true;
  }

  public getLanguagesData() {
    return this.languages;
  }

  public getWebsiteBundle(websiteLanguageId: number): Promise<Observable<any>> {
    return this.http
      .get(this._baseUrl + 'api/website-bundle', {
        headers: new Headers(),
        params: {
          websiteLanguageId: websiteLanguageId
        }
      }).toPromise().then((data: any) => {
        return data;
      }).catch((data: any) => {
        return data;
      });
  }

  public getWebsiteLanguageByDefaultLanguage(defaultlanguage: boolean): Observable<any> {
    return this.http
      .get(this._baseUrl + 'api/website-language-by-default-language', {
        headers: new Headers(),
        params: {
          Defaultlanguage: defaultlanguage
        }
      });
  }

  public getWebsiteLanguages(websiteLanguageId: number): Promise<Observable<any>> { 
    return this.http
      .get(this._baseUrl + 'api/website-languages', {
        headers: new Headers(),
        params: {
          websiteLanguageId: websiteLanguageId
        }
      }).toPromise().then((data: any) => {
        return data;
      }).catch((data: any) => {
        return data;
      });
  }

  public async setWebsiteBundle(websiteLanguageId: number): Promise<any> {
    return await this.getWebsiteBundle(websiteLanguageId).then((data: any) => {
      this.setWebsiteData(data.json());
    });
  }

  public async setWebsiteLanguages(websiteLanguageId: number): Promise<any> {
    return await this.getWebsiteLanguages(websiteLanguageId).then((data: any) => {
      this.setLanguagesData(data.json());
    });
  }
}
