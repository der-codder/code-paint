import { NgModule } from '@angular/core';
import { MonacoEditorLoaderDirective } from './directives/monaco-editor-loader.directive';
import { MonacoEditorComponent } from './components/monaco-editor/monaco-editor.component';
import { ResizedDirective } from './directives/resized-event.directive';

@NgModule({
  imports: [],
  declarations: [
    MonacoEditorLoaderDirective,
    MonacoEditorComponent,
    ResizedDirective
  ],
  exports: [
    MonacoEditorLoaderDirective,
    MonacoEditorComponent,
    ResizedDirective
  ],
  entryComponents: [MonacoEditorComponent]
})
export class MonacoEditorModule { }
