import { Component, ElementRef, Input, ViewEncapsulation, NgModule } from '@angular/core';
import { Res } from "../../../models/res.model";
import { NavigationService } from '../../../services/navigation.service';

declare let $: any;

@Component({
  selector: 'navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.css'],
  encapsulation: ViewEncapsulation.None
})

export class NavigationComponent {
  @Input() res = new Res();
  @Input() website: any = {};

  get navigations() { return this.naviagtionService.getNavigationsData(); };

  constructor(private naviagtionService: NavigationService) { }

  ngAfterViewInit() {
    if (typeof window !== 'undefined' && typeof $ !== 'undefined') {
      $(window).on("scroll", function () {
        var scroll = $(window).scrollTop();

        if (scroll) {
          if (scroll > 30) {
            $("#navbar").addClass("nb-scroll");
          } else {
            $("#navbar").removeClass("nb-scroll");
          }
        }
      }).trigger("scroll");
    }
    //this.initNavigation();
  }

  public initNavigation() {
    $('.dropdown-menu a.dropdown-toggle').on('click', function (e) {
      if (!$(this).next().hasClass('show')) {
        $(this).parents('.dropdown-menu').first().find('.show').removeClass('show').parent('li').removeClass('show');
      }
      var $subMenu = $(this).next('.dropdown-menu');
      $subMenu.toggleClass('show');
      $subMenu.parent('li').toggleClass('show');


      $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function (e) {
        $('.dropdown-submenu .show').removeClass('show');
        $('.dropdown-submenu').parent('li').removeClass('show');
      });

      return false;
    });
  }
}
