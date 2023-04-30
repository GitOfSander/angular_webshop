import { Component, ElementRef, Directive, Input, HostBinding, ViewEncapsulation, ViewChildren, QueryList, ChangeDetectorRef } from '@angular/core';
import { Router, ActivationStart, RouterStateSnapshot, ActivatedRoute } from '@angular/router';
import { Res } from '../../../models/res.model';
import { OrderService } from '../../../services/order.service'
import { CookieModule } from '../../../modules/cookie.module';
import { Http } from '@angular/http';
import { CommerceService } from '../../../services/commerce.service';
import { trigger, transition, animate, style, group, query, state } from '@angular/animations';

declare let $: any;

@Component({
    selector: 'shopping-cart',
    templateUrl: './shoppingcart.component.html',
    styleUrls: ['./shoppingcart.component.css'],
    encapsulation: ViewEncapsulation.None, 
    animations: [
        trigger('popUp', [
            transition('* <=> *', [
                style({ transform: 'scale(1)' }),
                animate('300ms ease-in', style({ transform: 'scale(1.8)' }))
            ])
        ])
    ]
})
    
export class ShoppingCartComponent {
    @Input() res = new Res();
    @Input() website: any = {};

    get order() { return this.commerceService.getOrder(); };
    get cartUrl() { return this.commerceService.getCartUrl(); }
    get checkoutUrl() { return this.commerceService.getCheckoutUrl(); }

    public disabled: boolean = false;

  constructor(private http: Http, private route: ActivatedRoute, private router: Router, private commerceService: CommerceService, private cdRef: ChangeDetectorRef) {
    this.router.events.subscribe((evt: any) => {
      if (evt instanceof ActivationStart) {
        if (typeof $ !== 'undefined') {
          $('.shopping-cart-toggle').removeClass('active');
          $('.shopping-cart').removeClass('show');
        }
      }
    });
  }

  ngAfterViewInit() {
    if (typeof $ !== 'undefined') {
      var container: any = $('.shopping-cart');
      var cart: any = $('#cart');

      cart.on('click', function () {
        cart.toggleClass('active');
        container.toggleClass('show');
      });

      this.things.changes.subscribe(t => {
        this.productsInitialized();
      })

      if (typeof document !== 'undefined') {
        $(document).on('mouseup', function (e: any) {
          // if the target of the click isn't the container nor a descendant of the container
          if (!container.is(e.target) && container.has(e.target).length === 0 && !cart.is(e.target) && cart.has(e.target).length === 0) {
            cart.removeClass('active');
            container.removeClass('show');
          }
        });
      }
    }
  }

    @ViewChildren('productsInitialized') things: QueryList<any> = new QueryList<any>();

    public productsInitialized() {
        this.disabled = false;
        this.cdRef.detectChanges();
    }

    public plus(id: number) {
        this.disabled = true;
        this.cdRef.detectChanges();
        var val: number = parseInt($('.shopping-cart-items li[data-id="' + id + '"] .input-number input').val()) + 1;
        new OrderService(this.http, this.commerceService).addProductToOrder(id, this.res.websiteLanguageId, val, false)
        $('.shopping-cart-items li[data-id="' + id + '"] .input-number input').val(val);
    }

    public min(id: number) {
        var val: number = parseInt($('.shopping-cart-items li[data-id="' + id + '"] .input-number input').val()) - 1;

        if (val > 0) {
            this.disabled = true;
            this.cdRef.detectChanges();
          new OrderService(this.http, this.commerceService).addProductToOrder(id, this.res.websiteLanguageId, val, false);
            $('.shopping-cart-items li[data-id="' + id + '"] .input-number input').val(val);
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
}
