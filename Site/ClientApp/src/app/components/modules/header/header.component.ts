import { Component, Directive, Input, HostBinding, ViewEncapsulation, ChangeDetectorRef, Renderer2, ElementRef } from '@angular/core';
import { DomSanitizer } from "@angular/platform-browser";
import { Res } from "../../../models/res.model";
import { CommerceService } from '../../../services/commerce.service';

declare let $: any;

@Component({
    selector: 'header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.css'],
    encapsulation: ViewEncapsulation.None
})

export class HeaderComponent {
  @Input() res = new Res();
  public vid: any;
  public vidHtml: any;

  get continueShoppingUrl() { return this.commerceService.getContinueShoppingUrl(); };

  constructor(private sanitizer: DomSanitizer, private commerceService: CommerceService, private cdRef: ChangeDetectorRef, private elementRef: ElementRef, private renderer: Renderer2) { }

  ngAfterViewInit() {
    this.initVideo();
  }

    public initVideo() {
        if (/9500|9800|9810|9860|BlackBerry9500|BlackBerry9800|BlackBerry9810|BlackBerry9860|iphone|ipod|iPad|webOS|android|bb10/i.test(navigator.userAgent.toLowerCase())) {
            //$("video#vid").remove();
        } else {
            this.setVideo();

            //this.vid = $('#vid');
            //this.vid[0].load();
            //var e = this;
            //setTimeout(function () { e.vid[0].play(); }, 3000);
            //this.vid.on('ended', function (data: any) { e.restartVideo(); });
        }
    }

  public setVideo() {
    if (typeof window !== 'undefined') {
      var video: string = '';
      var ua = window.navigator.userAgent;
      var msie = ua.indexOf("MSIE ");
      if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))  // If Internet Explorer, return version number
      {
        video = '<video data-ignore id="vid" class="embed-responsive-item" autoplay muted playsinline loop poster="' + (this.res.page.files.hPoster ? this.res.page.files.hPoster[0].compressedPath : "") + '" title="' + (this.res.page.files.hPoster ? this.res.page.files.hPoster[0].alt : "") + '">';

        for (let key in this.res.page.files.hVideo) {
          video += '<source src="' + this.res.page.files.hVideo[key].originalPath + '" type="' + this.getVideoType(this.res.page.files.hVideo[key].originalPath) + '">'
        }

        video += '</video>'
      } else {
        video = '<video data-ignore id="vid" class="embed-responsive-item fp-lazy" autoplay muted playsinline loop poster="' + (this.res.page.files.hPoster ? this.res.page.files.hPoster[0].compressedPath : "") + '" title="' + (this.res.page.files.hPoster ? this.res.page.files.hPoster[0].alt : "") + '">';

        for (let key in this.res.page.files.hVideo) {
          video += '<source data-src="' + this.res.page.files.hVideo[key].originalPath + '" type="' + this.getVideoType(this.res.page.files.hVideo[key].originalPath) + '">'
        }

        video += '</video>'
      }

      this.vidHtml = this.sanitizer.bypassSecurityTrustHtml(video);
      this.cdRef.detectChanges();
    }
  }

    //public restartVideo() {
    //    this.vid.currentTime = .1, this.vid[0].play();
    //}

    public getVideoType(file: any) {
        var extension: string = file.slice((file.lastIndexOf(".") - 1 >>> 0) + 2);

        switch (extension.toLowerCase()) {
            case 'ogv':
                return 'video/ogg';
            case 'mp4':
                return 'video/mp4';
            case 'webm':
                return 'video/webm';
            default:
                return '';
        }
    }
}
