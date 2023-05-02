import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { WineComponent } from './wine.component'


const routes: Routes = [
  { path: "", component: WineComponent },
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forChild(routes)]
})
export class WineRoutingModule {

}
