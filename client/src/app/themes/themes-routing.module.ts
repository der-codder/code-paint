import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ThemesComponent } from './themes.component';


const routes: Routes = [
  { path: '', component: ThemesComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ThemesRoutingModule {}
