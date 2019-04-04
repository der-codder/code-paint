import { NgModule } from '@angular/core';

import { SharedModule } from '@app/shared';

import { ThemeViewerComponent } from './theme-viewer.component';
import { ThemeViewerRoutingModule } from './theme-viewer-routing.module';
import { ViewerComponent } from './viewer/viewer.component';
import { ThemeViewerHeaderComponent } from './theme-viewer-header/theme-viewer-header.component';

@NgModule({
  declarations: [
    ThemeViewerComponent,
    ThemeViewerHeaderComponent,
    ViewerComponent
  ],
  imports: [
    SharedModule,
    ThemeViewerRoutingModule
  ]
})
export class ThemeViewerModule {
}
