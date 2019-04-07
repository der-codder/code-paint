import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { Theme } from '@app/core';

@Component({
  selector: 'cp-viewer-toolbar',
  templateUrl: './viewer-toolbar.component.html',
  styles: []
})
export class ViewerToolbarComponent implements OnInit {
  @Input()
  themes: Theme[];
  @Input()
  selectedThemeLabel: string;
  @Output()
  selectedThemeChange = new EventEmitter<string>();

  constructor() { }

  ngOnInit() {
  }
}
