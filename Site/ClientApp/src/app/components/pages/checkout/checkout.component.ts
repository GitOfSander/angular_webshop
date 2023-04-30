import { ChangeDetectorRef, Component, Inject, QueryList, Renderer2, ViewChildren, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { Headers, Http } from '@angular/http';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { NgsRevealConfig } from 'ngx-scrollreveal';
import '../../../../assets/js/sticky-sidebar.js';
import { Res } from '../../../models/res.model';
import { CookieModule } from '../../../modules/cookie.module';
import { CommerceService } from '../../../services/commerce.service';
import { OrderService } from '../../../services/order.service';
import { WebsiteService } from '../../../services/website.service';

declare let $: any;
declare var StickySidebar: any;

@Component({
  selector: 'checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css'],
  encapsulation: ViewEncapsulation.None
})

export class CheckoutComponent {
  public res = new Res();
  public count: number = 140;
  public countColor: SafeStyle = this.sanitizer.bypassSecurityTrustStyle('');
  public client: string = 'private';
  public deliveryClient: string = 'private';
  public greetingCardText: string = '';
  public reserveMinutsExpired: boolean = false;
  public errorMessage: boolean = false;
  public request: boolean = true;
  public submitted: boolean = false;
  public orderInfo: any;
  public text: string = "";
  public disabled: boolean = false;
  private _baseUrl: string = '';


  get website() { return this.websiteService.getWebsiteData(); };
  get order() {
    this.orderInfo = this.commerceService.getOrder();

    return this.commerceService.getOrder();
  }
  get cartUrl() { return this.commerceService.getCartUrl(); };

  public checkout = {
    websiteLanguageId: 0,
    reserveGuid: CookieModule.read('cart'),
    newspaper: false,
    client: 'private',
    deliveryClient: 'private',
    differentAddress: false,
    email: '',
    //phoneNumber: '',
    company: '',
    city: '',
    firstName: '',
    lastName: '',
    zipCode: '',
    houseNr: '',
    addition: '',
    addressLine1: '',
    deliveryCompany: '',
    deliveryCity: '',
    deliveryFirstName: '',
    deliveryLastName: '',
    deliveryZipCode: '',
    deliveryHouseNr: '',
    deliveryAddition: '',
    deliveryAddressLine1: '',
    greetingCard: '',
    issuer: '',
    agreement: false
  };
  public checkoutForm: FormGroup;

  get email() { return this.checkoutForm.get('email'); }
  //get phoneNumber() { return this.checkoutForm.get('phoneNumber'); }
  get company() { return this.checkoutForm.get('company'); }
  get city() { return this.checkoutForm.get('city'); }
  get firstName() { return this.checkoutForm.get('firstName'); }
  get lastName() { return this.checkoutForm.get('lastName'); }
  get zipCode() { return this.checkoutForm.get('zipCode'); }
  get houseNr() { return this.checkoutForm.get('houseNr'); }
  get addition() { return this.checkoutForm.get('addition'); }
  get addressLine1() { return this.checkoutForm.get('addressLine1'); }
  get deliveryCompany() { return this.checkoutForm.get('deliveryCompany'); }
  get deliveryCity() { return this.checkoutForm.get('deliveryCity'); }
  get deliveryFirstName() { return this.checkoutForm.get('deliveryFirstName'); }
  get deliveryLastName() { return this.checkoutForm.get('deliveryLastName'); }
  get deliveryZipCode() { return this.checkoutForm.get('deliveryZipCode'); }
  get deliveryHouseNr() { return this.checkoutForm.get('deliveryHouseNr'); }
  get deliveryAddition() { return this.checkoutForm.get('deliveryAddition'); }
  get deliveryAddressLine1() { return this.checkoutForm.get('deliveryAddressLine1'); }
  get greetingCard() { return this.checkoutForm.get('greetingCard'); }
  get issuer() { return this.checkoutForm.get('issuer'); }
  get agreement() { return this.checkoutForm.get('agreement'); }

  constructor(private http: Http, private route: ActivatedRoute, private renderer: Renderer2, private sanitizer: DomSanitizer, private commerceService: CommerceService, private websiteService: WebsiteService, private cdRef: ChangeDetectorRef, revealConfig: NgsRevealConfig, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;

    revealConfig.duration = 450;
    revealConfig.easing = 'cubic-bezier(0.30, 0.045, 0.355, 1)';
    revealConfig.delay = 100;
    revealConfig.scale = 1;

    if (typeof document !== 'undefined') {
      this.renderer.removeAttribute(document.body, 'class');
      this.renderer.addClass(document.body, 'checkout');
    }

    this.checkoutForm = new FormGroup({
      'newspaper': new FormControl(this.checkout.newspaper),
      'client': new FormControl(this.checkout.client),
      'deliveryClient': new FormControl(this.checkout.deliveryClient),
      'differentAddress': new FormControl(this.checkout.differentAddress),
      'email': new FormControl(this.checkout.email, [
        Validators.required,
        Validators.maxLength(250),
        Validators.pattern("[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,3}$")
      ]),
      //'phoneNumber': new FormControl(this.checkout.phoneNumber, [
      //    Validators.maxLength(250),
      //    Validators.pattern("[0-9]*")
      //]),
      'company': new FormControl(this.checkout.company, [
        Validators.required,
        Validators.maxLength(250)
      ]),
      'city': new FormControl(this.checkout.city, [
        Validators.required,
        Validators.maxLength(250)
      ]),
      'firstName': new FormControl(this.checkout.firstName, [
        Validators.required,
        Validators.maxLength(250),
        Validators.pattern("[a-zA-Z\sàáâäãåèéêëìíîïòóôöõøùúûüÿýñçčšžÀÁÂÄÃÅÈÉÊËÌÍÎÏÒÓÔÖÕØÙÚÛÜŸÝÑßÇŒÆČŠŽ∂ð ,.'-]+$")
      ]),
      'lastName': new FormControl(this.checkout.lastName, [
        Validators.required,
        Validators.maxLength(250),
        Validators.pattern("[a-zA-Z\sàáâäãåèéêëìíîïòóôöõøùúûüÿýñçčšžÀÁÂÄÃÅÈÉÊËÌÍÎÏÒÓÔÖÕØÙÚÛÜŸÝÑßÇŒÆČŠŽ∂ð ,.'-]+$")
      ]),
      'zipCode': new FormControl(this.checkout.zipCode, [
        Validators.required,
        Validators.maxLength(32),
        Validators.pattern("[1-9][0-9]{3}[ ]?([A-RT-Za-rt-z][A-Za-z]|[sS][BCbcE-Re-rT-Zt-z])") /* Only validates zip codes from the netherlands */
      ]),
      'houseNr': new FormControl(this.checkout.houseNr, [
        Validators.required,
        Validators.maxLength(15),
        Validators.pattern("[0-9]*")
      ]),
      'addition': new FormControl(this.checkout.addition, [
        Validators.maxLength(10)
      ]),
      'addressLine1': new FormControl(this.checkout.addressLine1, [
        Validators.required,
        Validators.maxLength(220)
      ]),
      'deliveryCompany': new FormControl(this.checkout.deliveryCompany, [
        Validators.required,
        Validators.maxLength(250)
      ]),
      'deliveryCity': new FormControl(this.checkout.deliveryCity, [
        Validators.required,
        Validators.maxLength(250)
      ]),
      'deliveryFirstName': new FormControl(this.checkout.deliveryFirstName, [
        Validators.required,
        Validators.maxLength(250),
        Validators.pattern("[a-zA-Z\sàáâäãåèéêëìíîïòóôöõøùúûüÿýñçčšžÀÁÂÄÃÅÈÉÊËÌÍÎÏÒÓÔÖÕØÙÚÛÜŸÝÑßÇŒÆČŠŽ∂ð ,.'-]+$")
      ]),
      'deliveryLastName': new FormControl(this.checkout.deliveryLastName, [
        Validators.required,
        Validators.maxLength(250),
        Validators.pattern("[a-zA-Z\sàáâäãåèéêëìíîïòóôöõøùúûüÿýñçčšžÀÁÂÄÃÅÈÉÊËÌÍÎÏÒÓÔÖÕØÙÚÛÜŸÝÑßÇŒÆČŠŽ∂ð ,.'-]+$")
      ]),
      'deliveryZipCode': new FormControl(this.checkout.deliveryZipCode, [
        Validators.required,
        Validators.maxLength(32),
        Validators.pattern("[1-9][0-9]{3}[ ]?([A-RT-Za-rt-z][A-Za-z]|[sS][BCbcE-Re-rT-Zt-z])") /* Only validates zip codes from the netherlands */
      ]),
      'deliveryHouseNr': new FormControl(this.checkout.deliveryHouseNr, [
        Validators.required,
        Validators.maxLength(15),
        Validators.pattern("[0-9]*")
      ]),
      'deliveryAddition': new FormControl(this.checkout.deliveryAddition, [
        Validators.maxLength(10)
      ]),
      'deliveryAddressLine1': new FormControl(this.checkout.deliveryAddressLine1, [
        Validators.required,
        Validators.maxLength(220)
      ]),
      'greetingCard': new FormControl(this.checkout.greetingCard, [
        Validators.maxLength(140)
      ]),
      'issuer': new FormControl(this.checkout.issuer, [
        Validators.required
      ]),
      'agreement': new FormControl(false, {
        validators: Validators.pattern("^(true)$"),
        updateOn: 'change'
      })
    }, { updateOn: 'change' });

    //this.checkoutForm.valueChanges.subscribe(val => {
    //    console.log(`Checkbox: ${val.agreement}`);
    //});
  }

  ngOnInit() {
    this.res = this.route.snapshot.data['init'];
    this.checkout.websiteLanguageId = this.res.websiteLanguageId;

    new OrderService(this.http, this.commerceService, this._baseUrl).setOrder(this.res.websiteLanguageId, true);

    this.updateDeliveryValidation(this.checkout.differentAddress);
    this.updateCompanyValidation('private');
  }

  ngAfterViewInit() {
    if (typeof this.orderInfo.products !== 'undefined') {
      if (this.orderInfo.products.length > 0) {
        var sidebar = new StickySidebar('.checkout-sidebar', {
          topSpacing: '50px',
          bottomSpacing: 0,
          containerSelector: '.checkout-info',
          innerWrapperSelector: '.checkout-sidebar-inner',
          resizeSensor: true,
          stickyClass: 'is-affixed'
        });
      }

      this.orderInfo = this.commerceService.getOrder();
      this.cdRef.detectChanges();
    }

    this.things.changes.subscribe(t => {
      this.productsInitialized();
    })

    if (typeof document !== 'undefined') this.renderer.setAttribute(document.body, 'data-loaded', 'true');
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
    new OrderService(this.http, this.commerceService, this._baseUrl).addProductToOrder(id, this.res.websiteLanguageId, val, false)
    $('.shopping-cart-items li[data-id="' + id + '"] .input-number input').val(val);
  }

  public min(id: number) {
    var val: number = parseInt($('.shopping-cart-items li[data-id="' + id + '"] .input-number input').val()) - 1;

    if (val > 0) {
      this.disabled = true;
      this.cdRef.detectChanges();
      new OrderService(this.http, this.commerceService, this._baseUrl).addProductToOrder(id, this.res.websiteLanguageId, val, false);
      $('.shopping-cart-items li[data-id="' + id + '"] .input-number input').val(val);
    }
  }

  public updateQuantity(id: number, val: string) {
    if (parseInt(val) > 0) {
      new OrderService(this.http, this.commerceService, this._baseUrl).addProductToOrder(id, this.res.websiteLanguageId, parseInt(val), false);
    } else {
      new OrderService(this.http, this.commerceService, this._baseUrl).deleteProductFromOrder(id, this.res.websiteLanguageId);
    }
  }

  public deleteProduct(id: number) {
    new OrderService(this.http, this.commerceService, this._baseUrl).deleteProductFromOrder(id, this.res.websiteLanguageId);
  }

  public toggleGreetingCard(value: any) {
    if (value) {
      $('#wishCard').slideDown(450);
    } else {
      $('#wishCard').slideUp(400);
      $('textarea#greetingCard').val('');
      this.greetingCardText = '';
    }
  }

  public updateCounter(value: string) {
    //var lines = value.split('\n');
    //var last_line = lines[lines.length - 1];
    //if (last_line.length >= 38) {
    //
    //    // Resetting the textarea val() in this way has the 
    //    // effect of adding a line break at the end of the 
    //    // textarea and putting the caret position at the 
    //    // end of the textarea
    //    $(this).val($(this).val() + "\n");
    //
    //}

    //if (value.length > 0) {
    //    $('#wishCardSample').slideDown(450);
    //} else {
    //    $('#wishCardSample').slideUp(400);
    //}

    var line_height = Math.floor($('textarea#greetingCard').height() / parseInt($('textarea#greetingCard').attr("rows")));
    var dirty_number_of_lines = (Math.ceil($('textarea#greetingCard')[0].scrollHeight / line_height));
    if (dirty_number_of_lines > 8) {
      $('textarea#greetingCard').val(this.text);
    } else {
      this.text = value;

      this.count = 140 - value.length;
      this.count <= 0 ? this.countColor = this.sanitizer.bypassSecurityTrustStyle('#B20000') : this.countColor = this.sanitizer.bypassSecurityTrustStyle('');

      this.greetingCardText = value.replace(/\r|\r\n|\n/g, '<br/>');
    }
  }

  public deliveryAddress(eve: any) {
    this.checkout.differentAddress = !this.checkout.differentAddress;
    this.checkout.differentAddress == true ? $('#deliveryAddress').slideDown(450) : $('#deliveryAddress').slideUp(400);

    this.updateDeliveryValidation(this.checkout.differentAddress);
  }

  public deliveryClientType(value: any) {
    value != 'private' ? $('#deliveryCompany').slideDown(450) : $('#deliveryCompany').slideUp(400);

    this.updateDeliveryCompanyValidation(value);
  }

  public clientType(value: any) {
    value != 'private' ? $('#company').slideDown(450) : $('#company').slideUp(400);

    this.updateCompanyValidation(value);
  }

  public onSubmit(form: NgForm) {
    const searchParams = Object.keys(form.value).map((key) => {
      return encodeURIComponent(key) + '=' + encodeURIComponent(form.value[key]);
    }).join('&') + '&reserveGuid=' + this.checkout.reserveGuid + '&websiteLanguageId=' + this.checkout.websiteLanguageId;

    this.submitted = true;

    if (this.request) {
      this.request = false;

      this.http.post('/spine-api/payment',
        searchParams,
        {
          headers: new Headers({
            'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8',
            'Accept': 'application/json'
          })
        }).subscribe(
          entry => this.handleSubmitSuccess(entry),
          error => this.handleSubmitError(error)
        )
    }
  }

  protected handleSubmitSuccess(entry: any) {
    if (typeof window !== 'undefined') window.location.href = entry.json().value;

    //this.contactFormMessage = response.result;
    //this.contactFormMessageStatus = "success"

    //form.resetForm();

    //this.contactFormDisabled = false;
  }

  protected handleSubmitError(error: any) {
    this.request = true;

    if (error.status === 422) {
      const data = error.json().value;
      const fields = Object.keys(data.fields || {});
      fields.forEach((i: any) => {
        var field: any = data.fields[i];
        this.checkoutForm.controls[field.property].setErrors({ remote: field.message });
        this.checkoutForm.controls[field.property].markAsDirty({ onlySelf: true });
      });

      if (data.errorType === 'reserveMinutsExpired') {
        this.reserveMinutsExpired = true;
      }

      if (data.errorType === 'errorMessage') {
        this.errorMessage = true;
      }
    }
  }

  public updateDeliveryCompanyValidation(type: string) {
    if (type === 'private') {
      this.checkoutForm.controls['deliveryCompany'].disable();
    } else {
      this.checkoutForm.controls['deliveryCompany'].enable();
    }
  }

  public updateCompanyValidation(type: string) {
    if (type === 'private') {
      this.checkoutForm.controls['company'].disable();
    } else {
      this.checkoutForm.controls['company'].enable();
    }
  }

  public updateDeliveryValidation(enable: boolean) {
    if (enable) {
      if (this.checkout.deliveryClient == 'company') {
        this.checkoutForm.controls['deliveryCompany'].enable();
      }

      this.checkoutForm.controls['deliveryFirstName'].enable();
      this.checkoutForm.controls['deliveryLastName'].enable();
      this.checkoutForm.controls['deliveryZipCode'].enable();
      this.checkoutForm.controls['deliveryHouseNr'].enable();
      this.checkoutForm.controls['deliveryAddition'].enable();
      this.checkoutForm.controls['deliveryAddressLine1'].enable();
      this.checkoutForm.controls['deliveryCity'].enable();
    } else {
      this.checkoutForm.controls['deliveryCompany'].disable();
      this.checkoutForm.controls['deliveryFirstName'].disable();
      this.checkoutForm.controls['deliveryLastName'].disable();
      this.checkoutForm.controls['deliveryZipCode'].disable();
      this.checkoutForm.controls['deliveryHouseNr'].disable();
      this.checkoutForm.controls['deliveryAddition'].disable();
      this.checkoutForm.controls['deliveryAddressLine1'].disable();
      this.checkoutForm.controls['deliveryCity'].disable();
    }
  }
}
