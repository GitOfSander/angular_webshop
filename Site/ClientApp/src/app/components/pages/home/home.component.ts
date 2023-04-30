import { Component, ElementRef, HostListener, Renderer2, ViewEncapsulation } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from "@angular/router";
import "owl.carousel";
import LazyLoad from 'vanilla-lazyload/dist/lazyload';
import { Res } from "../../../models/res.model";
import { CommerceService } from '../../../services/commerce.service';
import { WebsiteService } from '../../../services/website.service';
import { NgsRevealConfig } from 'ngx-scrollreveal';

declare let $: any;

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css',
    '../../../../../node_modules/fullpage.js/dist/jquery.fullpage.min.css'],
  encapsulation: ViewEncapsulation.None
})

export class HomeComponent {
  public res = new Res();
  public currentIndex: number = 0;
  public showFooterCheck: number = 0;
  public lockSectionScroll: boolean = false;
  public lockFooterScroll: boolean = true;
  public bVidHtml: any;
  public tVidHtml: any;
  public fullpage: any;
  public lazyLoad: any;
  public loaded: boolean = false;

  get website() { return this.websiteService.getWebsiteData(); };
  get continueShoppingUrl() { return this.commerceService.getContinueShoppingUrl(); };

  constructor(private elementRef: ElementRef, private route: ActivatedRoute, private renderer: Renderer2, private websiteService: WebsiteService, private sanitizer: DomSanitizer, private commerceService: CommerceService, revealConfig: NgsRevealConfig) {
    revealConfig.duration = 450;
    revealConfig.easing = 'cubic-bezier(0.30, 0.045, 0.355, 1)';
    revealConfig.delay = 100;
    revealConfig.scale = 1;

    if (typeof document !== 'undefined') {
      this.renderer.removeAttribute(document.body, 'class');
      this.renderer.addClass(document.body, 'home');
    }
  }

  @HostListener('window:scroll', [])
  onWindowScroll() {
    if (typeof window !== 'undefined') {
      const number = window.scrollY;
      if (number > 150 && !this.loaded) {
        this.loaded = true;
        var elements = this.elementRef.nativeElement.querySelectorAll('section')[1].querySelectorAll('div.fp-cc-lazy');
        if (elements) {
          for (var i = 0; i < elements.length; ++i) {
            this.renderer.setStyle(elements[i], 'background-image', 'url("' + elements[i].dataset.src + '")');
          }
        }

        var elements = this.elementRef.nativeElement.querySelectorAll('section')[1].querySelectorAll('img.fp-cc-lazy');
        if (elements) {
          for (var i = 0; i < elements.length; ++i) {
            this.renderer.setAttribute(elements[i], 'src', elements[i].dataset.src);
          }
        }

        var element = this.elementRef.nativeElement.querySelectorAll('section')[2].querySelectorAll('.fp-lazy')[1];
        if (element) this.renderer.setAttribute(element, 'poster', element.dataset.poster);

        element = this.elementRef.nativeElement.querySelectorAll('section')[3].querySelectorAll('.fp-lazy')[1];
        if (element) this.renderer.setAttribute(element, 'poster', element.dataset.poster);
      }
    }
  }

  ngOnInit() {
    this.res = this.route.snapshot.data['init'];

    //this.setJsonLdScheme();
    this.setBoxVideo();
    this.setTastingVideo();
  }

  ngAfterViewInit() {
    if (typeof $ !== 'undefined') {
      if (typeof document !== 'undefined') this.renderer.setAttribute(document.body, 'data-loaded', 'true');

      this.checkMediaQuery();

      if (typeof document !== 'undefined') document.createElement('video'); //Old browser fix
      this.initVideo('#vid2');
      this.initVideo('#vid3');
      this.initVideo('#vidMob2');
      this.initVideo('#vidMob3');

      var container: any = $('footer');
      var self = this;
      if (typeof document !== 'undefined') {
        $(document).on('mouseup', function (e: any) {
          // if the target of the click isn't the container nor a descendant of the container
          if (!container.is(e.target) && container.has(e.target).length === 0) {

            if (self.showFooterCheck !== 0) {
              $('body').removeClass('footer-to-emerge').addClass('footer-disappear');
              container.removeClass('to-emerge').addClass('disappear');

              self.showFooterCheck = 0;
              self.showFooterCheck = 0;

              setTimeout(function () { self.lockSectionScroll = false; }, 1000);
            }
          }
        });
      }

      this.checkBrowserType();
    }
  }

  public checkBrowserType() {
    if (typeof window !== 'undefined') {
      var ua = window.navigator.userAgent;
      var msie = ua.indexOf("MSIE ");
      if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))  // If Internet Explorer, return version number
      {
        // IE 11 bug fix, couldn't scroll because of lazyload
        var elements = this.elementRef.nativeElement.querySelectorAll('video');
        for (var i = 0; i < elements.length; ++i) {
          this.renderer.removeClass(elements[i], 'fp-lazy');
        }

        elements = this.elementRef.nativeElement.querySelectorAll('video source');
        for (var i = 0; i < elements.length; ++i) {
          var src = elements[i].getAttribute('data-src');
          if (src !== null) {
            this.renderer.setAttribute(elements[i], 'src', src);
            this.renderer.removeAttribute(elements[i], 'data-src');
          }
        }
      }
    }
  }

  public setBoxVideo() {
    var video: string = '<video data-ignore id="vid2" class="embed-responsive-item fp-lazy" muted playsinline data-poster="' + (this.res.page.files.bPoster ? this.res.page.files.bPoster[0].compressedPath : "") + '" title="' + (this.res.page.files.bPoster ? this.res.page.files.bPoster[0].alt : "") + '">';

    for (let key in this.res.page.files.bVideo) {
      video += '<source data-src="' + this.res.page.files.bVideo[key].originalPath + '" type="' + this.getVideoType(this.res.page.files.bVideo[key].originalPath) + '">'
    }

    video += '</video>'

    this.bVidHtml = this.sanitizer.bypassSecurityTrustHtml(video)
  }

  public setTastingVideo() {
    var video: string = '<video data-ignore id="vid3" class="embed-responsive-item fp-lazy" muted playsinline data-poster="' + (this.res.page.files.tPoster ? this.res.page.files.tPoster[0].compressedPath : "") + '" title="' + (this.res.page.files.tPoster ? this.res.page.files.tPoster[0].alt : "") + '">';

    for (let key in this.res.page.files.tVideo) {
      video += '<source data-src="' + this.res.page.files.tVideo[key].originalPath + '" type="' + this.getVideoType(this.res.page.files.tVideo[key].originalPath) + '">'
    }

    video += '</video>'

    this.tVidHtml = this.sanitizer.bypassSecurityTrustHtml(video)
  }

  ngOnDestroy() {
    if (typeof $ !== 'undefined') {
      if (typeof $.fn.fullpage.destroy == 'function') {
        $.fn.fullpage.destroy('all');
      }
    }
  }

  @HostListener('window:mousewheel', ['$event']) onMousewheel(event: MouseEvent) {
    if (typeof window !== 'undefined') {
      var event2: any = window.event || event; // old IE support
      var delta = Math.max(-1, Math.min(1, (event2.wheelDelta || -event2.detail)));

      if (delta < 0) {
        if (this.currentIndex === 6) {
          this.showFooterCheck = 1;

          if (this.lockFooterScroll === false) {
            this.lockSectionScroll = true;
            $('body').removeClass('footer-disappear').addClass('footer-to-emerge').removeClass('footer-disappear');
            $('footer').removeClass('disappear').addClass('to-emerge');
          }
        }
      } else {
        if (this.showFooterCheck !== 0) {
          $('body').removeClass('footer-to-emerge').addClass('footer-disappear');
          $('footer').removeClass('to-emerge').addClass('disappear');

          this.showFooterCheck = 0;

          var e = this;
          setTimeout(function () { e.lockSectionScroll = false; }, 1000);
        }
      }
    }
  }

  public initVideo(id: any) {
    var e = this;
    $(id).on('ended', function (data: any) { e.restartVideo(id); });
  }

  public restartVideo(id: any) {
    $(id)[0].currentTime = .1, $(id)[0].play();
  }

  public setCurrentIndex(index: any) {
    this.currentIndex = index;
  }

  public playBoxVideo(index: any) {
    if (!$('html').hasClass('fp-enabled')) {
      if ($('#vidMob2')[0].paused) {
        if ($('#vidMob2')[0].readyState !== 4) {
          $('#vidMob2')[0].load()
        }

        $('#vidMob2')[0].play();
        $('.the-box-video').addClass('play');
      } else {
        $('#vidMob2')[0].pause();
        $('#vidMob2')[0].currentTime = .1;
        $('.the-box-video').removeClass('play');
      }
    }
  }

  public playTasteVideo(index: any) {
    if (!$('html').hasClass('fp-enabled')) {
      if ($('#vidMob3')[0].paused) {
        if ($('#vidMob3')[0].readyState !== 4) {
          $('#vidMob3')[0].load()
        }
        
        $('#vidMob3')[0].play();
        $('.connoisseur-video').addClass('play');
      } else {
        $('#vidMob3')[0].pause();
        $('#vidMob3')[0].currentTime = .1;
        $('.connoisseur-video').removeClass('play');
      }
    }
  }

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

  public checkMediaQuery() {
    if (typeof window !== 'undefined') {
      var matchMedia = window.matchMedia("(max-width: 991.5px)");
      this.match(matchMedia);
      matchMedia.addListener(this.match);
    }
  }

  public match: any = (matchMedia: any) => {
    if (matchMedia.matches) { // If media query matches
      this.destroyFullpage();

      this.lazyLoad = new LazyLoad({
        elements_selector: '.fp-lazy',
        threshold: 0
      });

      $("a.arrow").click(function () {
        var offset = 70; //Offset of 20px

        $('html, body').animate({
          scrollTop: $("#skip").offset().top - offset
        }, 2000);
      });
    } else {
      $("a.arrow").off();

      if (this.lazyLoad) this.lazyLoad.destroy();
      this.initFullpage();
    }
  }

  public destroyFullpage() {
    if (typeof $.fn.fullpage.destroy == 'function') {
      $.fn.fullpage.destroy('all');
    }
  }

  public initFullpage() {
    var self = this;
    this.fullpage = $("#fullpage").fullpage({
      anchors: ["header", "intro", "in-de-doos", "fijnproever", "champaigne", "over-proef"],
      navigation: !0,
      navigationPosition: "right",
      sectionSelector: 'section',
      scrollingSpeed: 1e3,
      verticalCentered: !1,
      lazyLoading: true,
      offsetSections: false,
      paddingTop: '0px',
      onLeave: function (index: any, nextIndex: any, direction: any) {
        if (self.lockSectionScroll) { return false; }

        if ($("#vid").length) {
          $('#vid')[0].pause();
        }
        $('#vid2')[0].pause();
        $('#vid3')[0].pause();

        var elements = self.elementRef.nativeElement.querySelectorAll('section')[1].querySelectorAll('div.fp-cc-lazy');
        if (elements) {
          for (var i = 0; i < elements.length; ++i) {
            self.renderer.setStyle(elements[i], 'background-image', 'url("' + elements[i].dataset.src + '")');
          }
        }

        var element = self.elementRef.nativeElement.querySelectorAll('section')[2].querySelector('.fp-lazy');
        if (element) self.renderer.setAttribute(element, 'poster', element.dataset.poster);

        element = self.elementRef.nativeElement.querySelectorAll('section')[3].querySelector('.fp-lazy');
        if (element) self.renderer.setAttribute(element, 'poster', element.dataset.poster);

        element = self.elementRef.nativeElement.querySelectorAll('section')[4];
        if (element) self.renderer.setStyle(element, 'background-image', 'url("' + element.dataset.src + '")');

        element = self.elementRef.nativeElement.querySelectorAll('section')[5].querySelector('.fp-lazy');
        if (element) self.renderer.setStyle(element, 'background-image', 'url("' + element.dataset.src + '")');

        self.loaded = true;

        switch (nextIndex) {
          case 1:
            if ($("#vid").length) {
              $('#vid')[0].currentTime = .1;
              setTimeout(function () { if ($('#vid').find('source[src]').length > 0) $('#vid')[0].play(); }, 10);
            }
            break;
          case 3:
            $('#vid2')[0].currentTime = .1;
            setTimeout(function () { if ($('#vid2').find('source[src]').length > 0) $('#vid2')[0].play(); }, 10);
            break;
          case 4:
            $('#vid3')[0].currentTime = .1;
            setTimeout(function () { if ($('#vid3').find('source[src]').length > 0) $('#vid3')[0].play(); }, 10);
            break;
          case 6:
            setTimeout(function () { self.lockFooterScroll = false; }, 1000);
        }

        switch (index) {
          //case 1:
          //    setTimeout(function () {
          //        if ($("#vid").length) {
          //            $('#vid')[0].pause();
          //        }
          //    }, 1000);
          //    break;
          //case 3:
          //    setTimeout(function () { $('#vid2')[0].pause() }, 1000);
          //    break;
          //case 4:
          //    setTimeout(function () { $('#vid3')[0].pause() }, 1000);
          //    break;
          case 6:
            self.lockFooterScroll = true;
            break;
        }

        self.setCurrentIndex(nextIndex);
      }
    });
  }
}
