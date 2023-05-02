import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { CheckoutComponent } from './checkout.component'


const routes: Routes = [
  { path: "", component: CheckoutComponent },
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forChild(routes)]
})
export class CheckoutRoutingModule {

}
