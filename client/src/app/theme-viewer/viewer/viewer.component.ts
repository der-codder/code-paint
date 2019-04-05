import { Component, OnInit, ChangeDetectionStrategy, Input } from '@angular/core';

import { Theme } from '@app/core';

@Component({
  selector: 'cp-viewer',
  templateUrl: './viewer.component.html',
  styles: [``],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ViewerComponent implements OnInit {
  editorOptions = {
    automaticLayout: true,
    theme: 'vs-dark',
    language: 'javascript'
  };
  code = 'function x() {\n  console.log("Hello world!");\n}';

  private _theme: Theme;
  get theme(): Theme {
    return this._theme;
  }
  @Input()
  set theme(value: Theme) {
    this._theme = value;
  }

  constructor() { }

  ngOnInit() { }

}
