import { NgModule } from '@angular/core';

import { SharedModule } from '@app/shared';

import { GalleryRoutingModule } from './gallery-routing.module';
import { GalleryHeaderComponent } from './gallerry-header/gallery-header.component';
import { GalleryComponent } from './gallery.component';
import { ThemeCardComponent } from './theme-card/theme-card.component';

@NgModule({
  declarations: [
    GalleryHeaderComponent,
    GalleryComponent,
    ThemeCardComponent
  ],
  imports: [
    SharedModule,
    GalleryRoutingModule
  ]
})
export class GalleryModule { }
