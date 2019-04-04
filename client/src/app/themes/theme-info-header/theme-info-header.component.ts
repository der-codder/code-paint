import { Component, OnInit, Input, ChangeDetectionStrategy } from '@angular/core';

import { ThemeInfo } from '@app/core';

@Component({
  selector: 'cp-theme-info-header',
  templateUrl: './theme-info-header.component.html',
  styleUrls: ['./theme-info-header.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ThemeInfoHeaderComponent implements OnInit {
  @Input()
  theme: ThemeInfo;

  constructor() { }

  ngOnInit() {
  }

}
