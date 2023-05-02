import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgsRevealConfig, NgsRevealModule } from 'ngx-scrollreveal';
import { AppComponent, appResolveFactory } from './app.component';
import { AppResolve } from "./app.resolve";
import { AppRoutingModule } from './app.routing.module';
import { NavigationComponent } from "./components/modules/navigation/navigation.component";
import { ShoppingCartComponent } from './components/modules/shoppingcart/shoppingcart.component';
import { AboutResolve } from './components/pages/about/about.resolve';
import { CartResolve } from './components/pages/cart/cart.resolve';
import { CategoryResolve } from './components/pages/category/category.resolve';
import { CheckoutResolve } from './components/pages/checkout/checkout.resolve';
import { CleanCategoryResolve } from './components/pages/cleancategory/cleancategory.resolve';
import { ConfirmationResolve } from './components/pages/confirmation/confirmation.resolve';
import { HomeResolve } from './components/pages/home/home.resolve';
import { ProductResolve } from './components/pages/product/product.resolve';
import { WineResolve } from './components/pages/wine/wine.resolve';

@NgModule({
  declarations: [
    AppComponent,
    NavigationComponent,
    ShoppingCartComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpModule,
    FormsModule,
    BrowserAnimationsModule,
    NgsRevealModule,
    AppRoutingModule
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
    NgsRevealConfig,
    AppResolve,
    {
      provide: APP_INITIALIZER,
      useFactory: appResolveFactory,
      deps: [AppResolve],
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
