import { Component, ElementRef, Directive, Input, HostBinding, ViewEncapsulation } from '@angular/core';
import { Res } from "../../../models/res.model";

@Component({
    selector: 'footer',
    templateUrl: './footer.component.html',
    styleUrls: ['./footer.component.css'],
    encapsulation: ViewEncapsulation.None
})

export class FooterComponent {
    @Input() website:any = {};
}
