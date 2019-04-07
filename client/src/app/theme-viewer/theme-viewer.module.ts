import { NgModule } from '@angular/core';

import { SharedModule } from '@app/shared';

import { ThemeViewerComponent } from './theme-viewer.component';
import { ThemeViewerRoutingModule } from './theme-viewer-routing.module';
import { ViewerComponent } from './viewer/viewer.component';
import { ThemeViewerHeaderComponent } from './theme-viewer-header/theme-viewer-header.component';
import { ViewerToolbarComponent } from './viewer/viewer-toolbar.component';
import { MonacoEditorModule } from '@app/monaco-editor/monaco-editor.module';

@NgModule({
  declarations: [
    ThemeViewerComponent,
    ThemeViewerHeaderComponent,
    ViewerComponent,
    ViewerToolbarComponent
  ],
  imports: [
    SharedModule,
    ThemeViewerRoutingModule,
    MonacoEditorModule
  ]
})
export class ThemeViewerModule {
}
