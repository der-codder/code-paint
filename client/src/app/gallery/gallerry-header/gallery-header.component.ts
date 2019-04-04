import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { SortByOption } from '../gallery.component';

@Component({
  selector: 'cp-gallery-header',
  templateUrl: './gallery-header.component.html',
  styles: [`
    h3 {
      padding: .4375em 0;
    }

    mat-form-field {
      margin-top: 15px;
    }

    .search-form-field {
      width: 350px;
    }
  `]
})
export class GalleryHeaderComponent {
  @Input()
  searchText: string;
  @Output()
  search = new EventEmitter<string>();
  @Input()
  selectedSort: string;
  @Output()
  sortByChange = new EventEmitter<string>();
  @Input()
  sortByOptions: SortByOption[];

  constructor() { }

  onSearch() {
    this.search.emit(this.searchText);
  }

  onInput(event: string) {
    if (event === '') {
      this.onSearch();
    }
  }

  onClearSearchText() {
    this.searchText = '';
    this.onSearch();
  }

}
