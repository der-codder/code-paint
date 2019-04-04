import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PageEvent } from '@angular/material/paginator';
import { Observable } from 'rxjs';
import { tap, switchMap } from 'rxjs/operators';

import { GalleryService, QueryResult, ThemeInfo } from '@app/core';

export interface SortByOption {
  value: string;
  description: string;
}

const DEFAULT_SORT_OPTIONS: SortByOption[] = [
  { value: 'Downloads', description: 'Downloads' },
  { value: 'UpdatedDate', description: 'Updated Date' },
  { value: 'Publisher', description: 'Publisher' },
  { value: 'Name', description: 'Name' },
  { value: 'Rating', description: 'Rating' },
  { value: 'TrendingWeekly', description: 'Trending' },
];

const SEARCHING_SORT_OPTIONS: SortByOption[] = [
  { value: 'Relevance', description: 'Relevance' },
  { value: 'Downloads', description: 'Downloads' },
  { value: 'UpdatedDate', description: 'Updated Date' },
  { value: 'Publisher', description: 'Publisher' },
  { value: 'Name', description: 'Name' },
  { value: 'Rating', description: 'Rating' },
  { value: 'TrendingWeekly', description: 'Trending' },
];

@Component({
  selector: 'cp-gallery',
  templateUrl: './gallery.component.html',
  styleUrls: ['./gallery.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GalleryComponent implements OnInit {
  gallery$: Observable<QueryResult>;
  searchText = '';
  querySearchText = '';
  sortBy: string;
  sortByOptions = DEFAULT_SORT_OPTIONS;
  pageNumber: number;
  readonly pageSize = 50;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private galleryService: GalleryService
  ) { }

  ngOnInit() {
    this.gallery$ = this.route.queryParamMap
      .pipe(
        tap(queryParams => {
          this.pageNumber = this.parsePageIndex(queryParams.get('pageIndex'));
          this.sortBy = queryParams.get('sortBy') || '';
          this.searchText = this.querySearchText = queryParams.get('search') || '';
        }),
        switchMap(() => {
          if (this.searchText !== '') {
            this.sortByOptions = SEARCHING_SORT_OPTIONS;
          } else {
            this.sortByOptions = DEFAULT_SORT_OPTIONS;
          }

          return this.galleryService.getThemes({
            sortBy: this.sortBy,
            pageNumber: this.pageNumber,
            pageSize: this.pageSize,
            searchTerm: this.searchText
          });
        })
      );
  }

  onSearch(searchText: string) {
    console.log('search ' + searchText);

    if (this.querySearchText === searchText) {
      return;
    }

    if (searchText === '') {
      this.navigateToDefaults();
      return;
    }

    this.router.navigate(
      ['/gallery'],
      {
        queryParams: {
          search: searchText,
          sortBy: 'Relevance',
          pageIndex: 1
        }
      }
    );
  }

  onSortByChanged(value: string) {
    this.router.navigate(
      ['/gallery'],
      {
        queryParams: {
          search: this.searchText,
          sortBy: value,
          pageIndex: 1
        }
      }
    );
  }

  onPageChanged(event: PageEvent) {
    this.router.navigate(
      ['/gallery'],
      {
        queryParams: {
          search: this.searchText,
          sortBy: this.sortBy,
          pageIndex: event.pageIndex + 1
        }
      }
    );
  }

  private navigateToDefaults() {
    this.router.navigate(
      ['/gallery'],
      {
        queryParams: {
          search: '',
          sortBy: 'Downloads',
          pageIndex: 1
        }
      }
    );
  }

  private parsePageIndex(value: string): number {
    const result = parseInt(value, 10);

    if (isNaN(result)) {
      return 1;
    }

    if (result < 1) {
      return 1;
    }

    return result;
  }

  private parseSortByQueryParam(value: string): number {
    switch (value) {
      case 'Relevance':
        return 0;
      case 'UpdatedDate':
        return 1;
      case 'Publisher':
        return 3;
      case 'Name':
        return 2;
      case 'PublishedDate':
        return 10;
      case 'Rating':
        return 12;
      case 'Trending':
        return 8;
    }

    return 4;
  }

  private _valueToSortByQueryParam(value: number): string {
    switch (value) {
      case 0:
        return 'Relevance';
      case 1:
        return 'UpdatedDate';
      case 3:
        return 'Publisher';
      case 2:
        return 'Name';
      case 10:
        return 'PublishedDate';
      case 12:
        return 'Rating';
      case 8:
        return 'Trending';
    }

    return 'Downloads';
  }
}
