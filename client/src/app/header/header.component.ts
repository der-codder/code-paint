import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'cp-header',
  templateUrl: './header.component.html',
  styles: [`
    .fill-remaining-space {
      flex: 1 1 auto;
    }
    .button-icon {
      font-size: 26px;
      display: inline;
      margin-right: 7px;
    }
  `]
})
export class HeaderComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
