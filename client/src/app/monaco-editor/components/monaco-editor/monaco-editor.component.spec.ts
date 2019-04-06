import { TestBed, async } from '@angular/core/testing';

import { MonacoEditorComponent } from './monaco-editor.component';
import { ResizedDirective } from '../../directives/resized-event.directive';
import { MonacoEditorLoaderDirective } from '../../directives/monaco-editor-loader.directive';
import { MonacoEditorLoaderService } from '../../services/monaco-editor-loader.service';
import { MonacoEditorModule } from '../../monaco-editor.module';

describe('MonacoEditorComponent', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
      ],
      declarations: [
        MonacoEditorComponent
      ],
      providers: [
        { provide: MonacoEditorLoaderService, useClass: MonacoEditorLoaderService }
      ]
    }).compileComponents();
  }));

  it('should create the component', async(() => {
    const fixture = TestBed.createComponent(MonacoEditorComponent);
    const component = fixture.debugElement.componentInstance;
    expect(component).toBeTruthy();
  }));
});
