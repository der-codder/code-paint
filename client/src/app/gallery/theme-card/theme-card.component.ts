import { Component, OnInit, Input } from '@angular/core';

import { ThemeInfo } from '@app/core';

@Component({
  selector: 'cp-theme-card',
  templateUrl: './theme-card.component.html',
  styleUrls: ['./theme-card.component.scss']
})
export class ThemeCardComponent implements OnInit {
  @Input()
  themeInfo: ThemeInfo;

  constructor() { }

  ngOnInit() {
  }

}
