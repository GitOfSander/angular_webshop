import { Component, ElementRef, ChangeDetectorRef, Input, ViewEncapsulation } from '@angular/core';
import { Res } from "../../../models/res.model";
import { Http } from '@angular/http';
import { OrderService } from '../../../services/order.service'
import { CommerceService } from '../../../services/commerce.service';

declare let $: any;

@Component({
    selector: 'products',
    templateUrl: './products.component.html',
    styleUrls: ['./products.component.css'],
    encapsulation: ViewEncapsulation.None
})

export class ProductsComponent {
  @Input() res = new Res();

  public id: number = 0;
  public title: string = '';
  public image: any;
  public buyDisabled: boolean = false;

  get checkoutUrl() { return this.commerceService.getCheckoutUrl(); }

  constructor(private http: Http, private commerceService: CommerceService, private cdRef: ChangeDetectorRef) {
  }

  ngAfterViewInit() {
    if (typeof $ !== 'undefined') {
      $('#productModal').on('hidden.bs.modal', function (e) {
        $('html').removeClass('modal-open');
      });
    }
  }

  public async showModal(image: any, title: string, id: number) {
    if (!this.buyDisabled) {
      this.id = id;
      this.title = title;
      this.image = image;
      this.buyDisabled = true;
      this.cdRef.detectChanges();

      await this.addProduct(id);
      $('html').addClass('modal-open');
      $('#productModal').modal('show');
      this.buyDisabled = false;
      this.cdRef.detectChanges();
    }
  }

  public async addProduct(id: number) {
    await new OrderService(this.http, this.commerceService).addProductToOrder(id, this.res.websiteLanguageId);
  }
}
