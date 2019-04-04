import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'cp-not-found',
  template: `
    <div class="centered-content">
      <h1>Error 404</h1>
      <p>Page not found. <a mat-button routerLink="/gallery" color="primary">Go back home</a></p>
    </div>
  `,
  styles: []
})
export class NotFoundComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
