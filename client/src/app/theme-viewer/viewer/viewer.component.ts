import { Component, OnInit, ChangeDetectionStrategy, Input } from '@angular/core';

import { Theme } from '@app/core';

interface EditorOptions {
  automaticLayout: boolean;
  theme: string;
  language: string;
}

@Component({
  selector: 'cp-viewer',
  templateUrl: './viewer.component.html',
  styles: [``],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ViewerComponent implements OnInit {
  editorTheme: Theme;
  editorOptions: EditorOptions = {
    automaticLayout: true,
    theme: 'vs-dark',
    language: 'javascript'
  };

  code = 'function x() {\n  console.log("Hello world!");\n}';

  @Input()
  themes: Theme[];
  selectedThemeLabel: string;

  constructor() { }

  ngOnInit() {
    this.selectedThemeLabel = this.themes[0].label;
  }

  onMonacoInitialized() {
    this.updateEditorTheme();
  }

  selectedThemeChanged(selectedThemeLabel: string) {
    this.selectedThemeLabel = selectedThemeLabel;
    this.updateEditorTheme();
  }

  private updateEditorTheme() {
    this.editorTheme = this.themes.find(t => t.label === this.selectedThemeLabel);
    console.log(JSON.stringify(this.editorTheme));
  }
}
