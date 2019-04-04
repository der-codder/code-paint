import { Component, OnInit, ChangeDetectionStrategy, Input } from '@angular/core';

import { ThemeInfo } from '@app/core';

@Component({
  selector: 'cp-theme-preview',
  templateUrl: './theme-preview.component.html',
  styleUrls: ['./theme-preview.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ThemePreviewComponent implements OnInit {

  private _themeInfo: ThemeInfo;
  public get themeInfo(): ThemeInfo {
    return this._themeInfo;
  }
  @Input()
  public set themeInfo(value: ThemeInfo) {
    this._themeInfo = value;
    console.log('themeInfo changed:' + JSON.stringify(this._themeInfo));
  }


  constructor() { }

  ngOnInit() {
    console.log('ngOnInit');
  }

}
