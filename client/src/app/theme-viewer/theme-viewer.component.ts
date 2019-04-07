import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { Observable, of } from 'rxjs';
import { tap, switchMap, map, delay } from 'rxjs/operators';

import { GalleryService, Extension, Theme } from '@app/core';

@Component({
  selector: 'cp-theme-viewer',
  templateUrl: './theme-viewer.component.html',
  styleUrls: ['./theme-viewer.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ThemeViewerComponent implements OnInit {
  extension$: Observable<Extension>;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private galleryService: GalleryService
  ) { }

  ngOnInit() {
    this.extension$ = this.route.paramMap
      .pipe(
        // delay(5000),
        switchMap((params: ParamMap) => {
          return this.galleryService.getExtension(params.get('id'));
        }),
        tap(theme => {
          this.loading = false;
        })
      );
  }
}
