import { Component, OnInit, Input } from '@angular/core';

import { ExtensionInfo } from '@app/core';

@Component({
  selector: 'cp-theme-card',
  templateUrl: './theme-card.component.html',
  styleUrls: ['./theme-card.component.scss']
})
export class ThemeCardComponent implements OnInit {
  @Input()
  extensionInfo: ExtensionInfo;

  constructor() { }

  ngOnInit() {
  }

}
