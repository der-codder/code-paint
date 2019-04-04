import { NgModule } from '@angular/core';

import { SharedModule } from '@app/shared';

import { ThemesComponent } from './themes.component';
import { ThemesRoutingModule } from './themes-routing.module';
import { ThemeInfoHeaderComponent } from './theme-info-header/theme-info-header.component';
import { ThemePreviewComponent } from './theme-preview/theme-preview.component';

@NgModule({
  declarations: [
    ThemesComponent,
    ThemeInfoHeaderComponent,
    ThemePreviewComponent
  ],
  imports: [
    SharedModule,
    ThemesRoutingModule
  ]
})
export class ThemesModule {
}
