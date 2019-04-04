import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ThemeViewerComponent } from './theme-viewer.component';

const routes: Routes = [
  { path: '', component: ThemeViewerComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ThemeViewerRoutingModule { }
