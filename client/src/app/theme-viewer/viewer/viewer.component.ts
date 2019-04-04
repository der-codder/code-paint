import { Component, OnInit, ChangeDetectionStrategy, Input } from '@angular/core';

import { Theme } from '@app/core';

@Component({
  selector: 'cp-viewer',
  templateUrl: './viewer.component.html',
  styles: [``],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ViewerComponent implements OnInit {

  private _theme: Theme;
  public get theme(): Theme {
    return this._theme;
  }
  @Input()
  public set theme(value: Theme) {
    this._theme = value;
  }


  constructor() { }

  ngOnInit() { }

}
