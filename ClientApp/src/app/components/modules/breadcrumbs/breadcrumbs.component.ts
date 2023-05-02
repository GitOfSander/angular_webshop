import { Component, ElementRef, Directive, Input, HostBinding, ViewEncapsulation } from '@angular/core';

@Component({
    selector: 'breadcrumbs',
    templateUrl: './breadcrumbs.component.html',
    styleUrls: ['./breadcrumbs.component.css'],
    encapsulation: ViewEncapsulation.None
})

export class BreadcrumbsComponent {
    @Input() breadcrumbs: any = {};
    @Input() title: string = '';

    constructor(private el: ElementRef) { }

    ngAfterViewInit() {
    }
}