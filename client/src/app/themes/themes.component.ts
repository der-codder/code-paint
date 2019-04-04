import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { tap, switchMap, map, delay } from 'rxjs/operators';

import { GalleryService, ThemeInfo } from '@app/core';

@Component({
  selector: 'cp-themes',
  templateUrl: './themes.component.html',
  styleUrls: ['./themes.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ThemesComponent implements OnInit {
  theme$: Observable<ThemeInfo>;
  loading = true;
  notFound = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private galleryService: GalleryService
  ) { }

  ngOnInit() {
    // this.theme$ = this.route.queryParamMap
    //   .pipe(
    //     map(queryParams => queryParams.get('themeName')),
    //     // delay(5000),
    //     switchMap(themeId => {
    //       return themeId
    //         ? this.galleryService.getTheme(themeId)
    //         : of(null);
    //     }),
    //     tap(theme => {
    //       this.loading = false;
    //       if (theme === null) {
    //         this.notFound = true;
    //       }
    //     })
    //   );
  }

}
