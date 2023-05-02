import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { CategoryComponent } from './category.component'


const routes: Routes = [
  { path: "", component: CategoryComponent },
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forChild(routes)]
})
export class CategoryRoutingModule {

}
