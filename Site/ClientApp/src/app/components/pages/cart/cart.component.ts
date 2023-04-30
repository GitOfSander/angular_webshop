import { ChangeDetectorRef, Component, QueryList, Renderer2, ViewChildren, ViewEncapsulation } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from "@angular/router";
import { Res } from "../../../models/res.model";
import { CommerceService } from '../../../services/commerce.service';
import { OrderService } from '../../../services/order.service';
import { ShippingService } from '../../../services/shipping.service';
import { WebsiteService } from '../../../services/website.service';
import { NgsRevealConfig } from 'ngx-scrollreveal';

declare let $: any;

@Component({
  selector: 'cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css'],
  encapsulation: ViewEncapsulation.None
})

export class CartComponent {
  public res = new Res();
  public disabled: boolean = false;

  get website() { return this.websiteService.getWebsiteData(); };
  get order() { return this.commerceService.getOrder(); };
  get checkoutUrl() { return this.commerceService.getCheckoutUrl(); };
  get continueShoppingUrl() { return this.commerceService.getContinueShoppingUrl(); };

  constructor(private http: Http, private route: ActivatedRoute, private renderer: Renderer2, private commerceService: CommerceService, private cdRef: ChangeDetectorRef, private websiteService: WebsiteService, revealConfig: NgsRevealConfig) {
    revealConfig.duration = 450;
    revealConfig.easing = 'cubic-bezier(0.30, 0.045, 0.355, 1)';
    revealConfig.delay = 100;
    revealConfig.scale = 1;

    if (typeof document !== 'undefined') {
      this.renderer.removeAttribute(document.body, 'class');
      this.renderer.addClass(document.body, 'cart');
    }
  }

  ngOnInit() {
    this.res = this.route.snapshot.data['init'];
  }

  ngAfterViewInit() {
    this.things.changes.subscribe(t => {
      this.productsInitialized();
    })

    if (typeof document !== 'undefined') this.renderer.setAttribute(document.body, 'data-loaded', 'true');
  }

  @ViewChildren('productsInitialized') things: QueryList<any> = new QueryList<any>();

  productsInitialized() {
    this.disabled = false;
    this.cdRef.detectChanges();
  }

  public plus(id: number) {
    this.disabled = true;
    this.cdRef.detectChanges();
    var val: number = parseInt($('.cart-table tr[data-id="' + id + '"] .input-number input').val()) + 1;
    new OrderService(this.http, this.commerceService).addProductToOrder(id, this.res.websiteLanguageId, val, false)
    $('.cart-table tr[data-id="' + id + '"] .input-number input').val(val);
  }

  public min(id: number) {
    var val: number = parseInt($('.cart-table tr[data-id="' + id + '"] .input-number input').val()) - 1;

    if (val > 0) {
      this.disabled = true;
      this.cdRef.detectChanges();
      new OrderService(this.http, this.commerceService).addProductToOrder(id, this.res.websiteLanguageId, val, false);
      $('.cart-table tr[data-id="' + id + '"] .input-number input').val(val);
    }
  }

  public updateQuantity(id: number, val: string) {
    if (parseInt(val) > 0) {
      new OrderService(this.http, this.commerceService).addProductToOrder(id, this.res.websiteLanguageId, parseInt(val), false);
    } else {
      new OrderService(this.http, this.commerceService).deleteProductFromOrder(id, this.res.websiteLanguageId);
    }
  }

  public deleteProduct(id: number) {
    new OrderService(this.http, this.commerceService).deleteProductFromOrder(id, this.res.websiteLanguageId);
  }

  public updateShippingMethod(id: any) {
    new ShippingService(this.http, this.commerceService).setShippingMethod(parseInt(id), this.res.websiteLanguageId);
  }
}
