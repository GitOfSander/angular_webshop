import { Component, ElementRef, Directive, Input, HostBinding, ViewEncapsulation } from '@angular/core';
import { Res } from "../../../models/res.model";
import { LazyLoadService } from '../../../services/plugins/lazyload.service';

@Component({
  selector: 'categories-white',
  templateUrl: './categorieswhite.component.html',
  styleUrls: ['./categorieswhite.component.css'],
  encapsulation: ViewEncapsulation.None
})

export class CategoriesWhiteComponent {
  @Input() res = new Res();

  constructor(private el: ElementRef, private lazyLoadService: LazyLoadService) { }

  ngAfterViewInit() {
    this.lazyLoadService.update();
  }
}
