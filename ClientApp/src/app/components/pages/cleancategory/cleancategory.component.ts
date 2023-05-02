import { Component, Renderer2, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { Res } from "../../../models/res.model";
import { LazyLoadService } from '../../../services/plugins/lazyload.service';
import { WebsiteService } from '../../../services/website.service';
import { NgsRevealConfig } from 'ngx-scrollreveal';

@Component({
  selector: 'clean-category',
  templateUrl: './cleancategory.component.html',
  styleUrls: ['./cleancategory.component.css'],
  encapsulation: ViewEncapsulation.None
})

export class CleanCategoryComponent {
  public res = new Res();

  get website() { return this.websiteService.getWebsiteData(); };

  constructor(private route: ActivatedRoute, private renderer: Renderer2, private websiteService: WebsiteService, private lazyLoadService: LazyLoadService, revealConfig: NgsRevealConfig) {
    revealConfig.duration = 450;
    revealConfig.easing = 'cubic-bezier(0.30, 0.045, 0.355, 1)';
    revealConfig.delay = 100;
    revealConfig.scale = 1;

    if (typeof document !== 'undefined') {
      this.renderer.removeAttribute(document.body, 'class');
      this.renderer.addClass(document.body, 'clean-category');
    }
  }

  ngOnInit() {
    this.res = this.route.snapshot.data['init'];
  }

  ngAfterViewInit() {
    this.lazyLoadService.update();

    if (typeof document !== 'undefined') this.renderer.setAttribute(document.body, 'data-loaded', 'true');
  }
}
