import { Component, OnInit, Input, ChangeDetectionStrategy } from '@angular/core';

import { ExtensionInfo } from '@app/core';

@Component({
  selector: 'cp-theme-viewer-header',
  templateUrl: './theme-viewer-header.component.html',
  styleUrls: ['./theme-viewer-header.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ThemeViewerHeaderComponent implements OnInit {
  @Input()
  extensionInfo: ExtensionInfo;

  constructor() { }

  ngOnInit() {
  }

}
