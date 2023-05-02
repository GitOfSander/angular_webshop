import { APP_INITIALIZER, Injectable, Injector, Inject, NgModule } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { Routes, Router, RouterModule } from '@angular/router';
import { of } from 'rxjs';
import { catchError, map } from "rxjs/operators";
import { AboutResolve } from './components/pages/about/about.resolve';
import { CartResolve } from './components/pages/cart/cart.resolve';
import { CategoryResolve } from './components/pages/category/category.resolve';
import { CheckoutResolve } from './components/pages/checkout/checkout.resolve';
import { CleanCategoryResolve } from './components/pages/cleancategory/cleancategory.resolve';
import { ConfirmationResolve } from './components/pages/confirmation/confirmation.resolve';
import { HomeResolve } from './components/pages/home/home.resolve';
import { ProductResolve } from './components/pages/product/product.resolve';
import { WineResolve } from './components/pages/wine/wine.resolve';



@Injectable()
export class ConfigService {
  private _configData: any = null;
  private _promise: Promise<any> = new Promise<any>(resolve => resolve(null));
  private _promiseDone: boolean = false;
  private _baseUrl : string = '';

  constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
  }

  loadConfig(): Promise<any> {
    this._configData = null;

    if (this._promiseDone) {
      //console.log("In Config Service. Promise is already complete.");
      return Promise.resolve();
    }
    console.log(this._baseUrl);
    //console.log("In Config Service. Loading config data.");
    this._promise = this.http
      .get(this._baseUrl + 'spine-api/routes', { headers: new Headers() })
      .pipe(map((data: any) => { return data.json(); }), catchError((error: any) => of(null)))
      .toPromise()
      .then((data: any) => {
        this._promiseDone = true;
        this._configData = data;
      })
      .catch((err: any) => { this._promiseDone = true; return Promise.resolve(); });
    return this._promise;
  }


  get configData(): any {
    return this._configData;
  }
}

/* Hack to lazy load components with dynamically loaded routes */
const routes: Routes = [
  {
    path: '~home',
    loadChildren: './components/pages/home/home.module#HomeModule'
  },
  {
    path: '~about',
    loadChildren: './components/pages/about/about.module#AboutModule'
  },
  {
    path: '~category',
    loadChildren: './components/pages/category/category.module#CategoryModule'
  },
  {
    path: '~cart',
    loadChildren: './components/pages/cart/cart.module#CartModule'
  },
  {
    path: '~checkout',
    loadChildren: './components/pages/checkout/checkout.module#CheckoutModule'
  },
  {
    path: '~cleancategory',
    loadChildren: './components/pages/cleancategory/cleancategory.module#CleanCategoryModule'
  },
  {
    path: '~confirmation',
    loadChildren: './components/pages/confirmation/confirmation.module#ConfirmationModule'
  },
  {
    path: '~product',
    loadChildren: './components/pages/product/product.module#ProductModule'
  },
  {
    path: '~wine',
    loadChildren: './components/pages/wine/wine.module#WineModule'
  },
  {
    path: '',
    redirectTo: '',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  entryComponents: [
  ],
  providers: [
    HomeResolve,
    AboutResolve,
    CategoryResolve,
    CartResolve,
    CheckoutResolve,
    CleanCategoryResolve,
    ConfirmationResolve,
    ProductResolve,
    WineResolve,
    ConfigService,
    { provide: APP_INITIALIZER, useFactory: configServiceFactory, deps: [Injector, ConfigService], multi: true },
  ]
})

export class AppRoutingModule {
  constructor(private config: ConfigService) { }
}

export function configServiceFactory(injector: Injector, configService: ConfigService): Function {
  return () => {
    return configService
      .loadConfig()
      .then(res => {
        var router: Router = injector.get(Router);
        var routesArray: Array<Object> = Array<Object>();
        if (configService.configData != null) {
          configService.configData.forEach(function (obj: any) {
            routesArray.push({
              path: obj.url,
              loadChildren: getComponent(obj.controller),
              //component: getComponent(obj.controller),
              resolve: {
                init: getResolve(obj.action),
              },
              data: {
                pageId: obj.pageId,
                websiteId: obj.websiteId,
                websiteLanguageId: obj.websiteLanguageId,
                alternateGuid: obj.alternateGuid
              },
              runGuardsAndResolvers: 'paramsOrQueryParamsChange'
            });
          });
        }

        routesArray.push({
          path: '',
          redirectTo: '',
          pathMatch: 'full'
        });

        routesArray.push({
            path: '**',
            redirectTo: '/'
        });

        router.resetConfig(routesArray);
      });
  }
}

export function getComponent(component: string): string {
  switch (component) {
    case 'HomeComponent':
      return './components/pages/home/home.module#HomeModule';
    case 'ConfirmationComponent':
      return './components/pages/confirmation/confirmation.module#ConfirmationModule';
    case 'AboutComponent':
      return './components/pages/about/about.module#AboutModule';
    case 'WineComponent':
      return './components/pages/wine/wine.module#WineModule';
    case 'CategoryComponent':
      return './components/pages/category/category.module#CategoryModule';
    case 'CleanCategoryComponent':
      return './components/pages/cleancategory/cleancategory.module#CleanCategoryModule';
    case 'ProductComponent':
      return './components/pages/product/product.module#ProductModule';
    case 'CartComponent':
      return './components/pages/cart/cart.module#CartModule';
    case 'CheckoutComponent':
      return './components/pages/checkout/checkout.module#CheckoutModule';
    default:
      return './components/pages/home/home.module#HomeModule';
  }
}


//export function getComponent(component: string): Function {
//  switch (component) {
//    case 'HomeComponent':
//      return () => require('./components/pages/home/home.module')['HomeModule'];
//    case 'ConfirmationComponent':
//      return () => require('./components/pages/confirmation/confirmation.module')['ConfirmationModule'];
//    case 'AboutComponent':
//      return () => require('./components/pages/about/about.module')['AboutModule'];
//    case 'WineComponent':
//      return () => require('./components/pages/wine/wine.module')['WineModule'];
//    case 'CategoryComponent':
//      return () => require('./components/pages/category/category.module')['CategoryModule'];
//    case 'CleanCategoryComponent':
//      return () => require('./components/pages/cleancategory/cleancategory.module')['CleanCategoryModule'];
//    case 'ProductComponent':
//      return () => require('./components/pages/product/product.module')['ProductModule'];
//    case 'CartComponent':
//      return () => require('./components/pages/cart/cart.module')['CartModule'];
//    case 'CheckoutComponent':
//      return () => require('./components/pages/checkout/checkout.module')['CheckoutModule'];
//    default:
//      return () => require('./components/pages/home/home.module')['HomeModule'];
//  }
//}

export function getResolve(resolve: string): Function {
  switch (resolve) {
    case 'HomeResolve':
      return HomeResolve;
    case 'ConfirmationResolve':
      return ConfirmationResolve;
    case 'AboutResolve':
      return AboutResolve;
    case 'WineResolve':
      return WineResolve;
    case 'CategoryResolve':
      return CategoryResolve;
    case 'CleanCategoryResolve':
      return CleanCategoryResolve;
    case 'ProductResolve':
      return ProductResolve;
    case 'CartResolve':
      return CartResolve;
    case 'CheckoutResolve':
      return CheckoutResolve;
    default:
      return HomeResolve;
  }
}
