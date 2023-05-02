import { Component, Renderer2, ViewEncapsulation } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ActivatedRoute } from "@angular/router";
import { Res } from "../../../models/res.model";
import { WebsiteService } from '../../../services/website.service';
import { NgsRevealConfig } from 'ngx-scrollreveal';

@Component({
  selector: 'confirmation',
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.css'],
  encapsulation: ViewEncapsulation.None
})

export class ConfirmationComponent {
  public res = new Res();
  public svg: SafeHtml = '';
  public title: string = '';
  public text: string = '';
  public bg: any = null;

  get website() { return this.websiteService.getWebsiteData(); };

  constructor(private route: ActivatedRoute, private renderer: Renderer2, private sanitizer: DomSanitizer, private websiteService: WebsiteService, revealConfig: NgsRevealConfig) {
    revealConfig.duration = 450;
    revealConfig.easing = 'cubic-bezier(0.30, 0.045, 0.355, 1)';
    revealConfig.delay = 100;
    revealConfig.scale = 1;

    if (typeof document !== 'undefined') {
      this.renderer.removeAttribute(document.body, 'class');
      this.renderer.addClass(document.body, 'confirmation');
    }
  }

  ngOnInit() {
    this.res = this.route.snapshot.data['init'];

    switch (this.res.data.order.status.toLowerCase()) {
      case 'completed':
        this.svg = this.sanitizer.bypassSecurityTrustHtml('<svg width="36px" height="26px" version="1.1" fill="none" stroke="#DAA520" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" id="Icons" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 50.6 36.8" style="enable-background:new 0 0 50.6 36.8;" xml:space="preserve"><g><g><path class="st0" d="M25.8,7.6c2.2,9,8.6,15.1,14.4,13.8c5.8-1.3,8.9-9.7,6.9-18.7L25.8,7.6z" /><line class="st0" x1="40.3" y1="21.9" x2="42.9" y2="33" /><line class="st0" x1="38.7" y1="34.2" x2="47.1" y2="32.2" /></g></g><path class="st0" d="M26.1,8.8c0,0,4.5,3.6,21.6,0" /><g><g><path class="st0" d="M24.8,7.6c-2.2,9-8.6,15.1-14.4,13.8C4.5,20,1.4,11.7,3.4,2.7L24.8,7.6z" /><line class="st0" x1="10.2" y1="21.9" x2="7.7" y2="33" /><line class="st0" x1="11.8" y1="34.2" x2="3.4" y2="32.2" /></g></g><path class="st0" d="M24.4,8.8c0,0-4.5,3.6-21.6,0" /></svg>')
        this.title = this.res.page.resources.title;
        this.text = this.res.page.resources.text;
        this.bg = this.res.page.files.bg;
        break;
      default:
        this.svg = this.sanitizer.bypassSecurityTrustHtml('<svg width="37px" height="64px" fill="none" stroke="#DAA520" stroke-width="2" stroke-miterlimit="10" version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 51.2 85.9" style="enable-background:new 0 0 51.2 85.9;" xml:space="preserve"><path class="st0" d="M17.3,29l-9.7,9.7l4.4,4.4l9.7-9.7c2.7,1.1,5.9,0.5,8.1-1.7l17.9-17.9L36.9,3L19,20.9C16.8,23.1,16.3,26.3,17.3,29z"/><g><path class="st0" d="M3.3,58C3.1,59,3,59.9,3,60.9c0,6.9,3.6,12.5,8,12.5c4.4,0,8-5.6,8-12.5c0-1-0.1-2-0.2-2.9H3.3z"/><line class="st0" x1="11" y1="73.5" x2="11" y2="83.3"/><line class="st0" x1="4.8" y1="83.3" x2="17.2" y2="83.3"/></g><path class="st0" d="M12.9,49.8c0,1-0.8,1.8-1.8,1.8s-1.8-0.8-1.8-1.8s0.8-3.7,1.8-3.7S12.9,48.8,12.9,49.8z"/></svg>')
        this.title = this.res.page.resources.cTitle;
        this.text = this.res.page.resources.cText;
        this.bg = this.res.page.files.cBg;
        break;
    }

    this.text = this.text.replace(':email:', this.res.data.order.billingEmail);
  }

  ngAfterViewInit() {
    if (typeof document !== 'undefined') this.renderer.setAttribute(document.body, 'data-loaded', 'true');
  }
}
