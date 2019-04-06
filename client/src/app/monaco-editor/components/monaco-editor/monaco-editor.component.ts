import {
  Component,
  ViewChild,
  ElementRef,
  OnInit,
  OnDestroy,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy
} from '@angular/core';

import { MonacoEditorLoaderService } from '../../services/monaco-editor-loader.service';
import { MonacoOptions } from '../../interfaces/monaco-options';
import { ResizedEvent } from '../../directives/resized-event.directive';
import { StandaloneThemeData } from '../../interfaces/standalone-theme-data';

declare const monaco: any;

@Component({
  selector: 'cp-monaco-editor',
  templateUrl: './monaco-editor.component.html',
  styleUrls: ['./monaco-editor.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MonacoEditorComponent implements OnInit, OnDestroy {
  container: HTMLDivElement;
  editor: any;

  private _code: string;
  @Input() set code(value: string) {
    this._code = value;
    this.updateCode();
  }
  get code(): string {
    return this._code;
  }

  private _options: MonacoOptions;
  @Input() set options(value: MonacoOptions) {
    this._options = value;
    this.updateOptions();
  }
  get options(): MonacoOptions {
    return this._options;
  }

  private _theme: StandaloneThemeData;
  @Input() set theme(value: StandaloneThemeData) {
    this._theme = value;
    this.updateTheme();
  }
  get theme(): StandaloneThemeData {
    return this._theme;
  }

  @Output() codeChange = new EventEmitter<string>();

  @ViewChild('editor') editorContent: ElementRef;

  constructor(private monacoLoader: MonacoEditorLoaderService) { }

  ngOnInit() {
    this.container = this.editorContent.nativeElement;
    this.codeChange.next(this.code);
    this.monacoLoader.isMonacoLoaded.subscribe(value => {
      if (value) {
        this.initMonaco();
      }
    });
  }

  onResized(event: ResizedEvent) {
    if (this.editor) {
      this.editor.layout({
        width: event.newWidth,
        height: event.newHeight
      });
    }
  }

  ngOnDestroy() {
    if (this.editor) {
      this.editor.dispose();
    }
  }

  private initMonaco() {
    const opts: any = {
      value: [this.code].join('\n'),
      language: 'json',
      automaticLayout: true,
      scrollBeyondLastLine: false
    };
    if (this.options.minimap) {
      opts.minimap = this.options.minimap;
    }
    if (this.options.readOnly) {
      opts['readOnly'] = true;
    }
    this.editor = monaco.editor.create(this.container, opts);
    this.editor.layout();

    this.editor.onDidChangeModelContent(changes => {
      this.codeChange.next(this.editor.getValue());
    });

    this.updateOptions();
  }

  private updateCode() {
    if (this.editor && this.code) {
      if (this.code !== this.editor.getValue()) {
        this.code
          ? this.editor.setValue(this.code)
          : this.editor.setValue('');
      }
    }
  }

  private updateOptions() {
    if (this.editor && this.options) {
      if (!this.options.language) {
        this.options.language = 'text';
      }
      monaco.editor.setModelLanguage(
        this.editor.getModel(),
        this.options.language
      );

      if (this.options.theme) {
        monaco.editor.setTheme(this.options.theme);
      }
    }
  }

  private updateTheme() {
    if (this.theme) {
      monaco.editor.defineTheme(
        this.theme.name,
        this.theme
      );
      monaco.editor.setTheme(this.theme.name);
    }
  }
}
