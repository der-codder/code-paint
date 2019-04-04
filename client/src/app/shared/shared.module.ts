import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FlexLayoutModule } from '@angular/flex-layout';
import { RouterModule } from '@angular/router';
import {
  MatCardModule,
  MatToolbarModule,
  MatFormFieldModule,
  MatInputModule,
  MatButtonModule,
  MatIconRegistry,
  MatIconModule,
  MatPaginatorModule,
  MatSelectModule,
  MatProgressSpinnerModule,
  MatTooltipModule
} from '@angular/material';

import { DefaultIconPipe } from './pipes/default-icon.pipe';
import { InstallCountPipe } from './pipes/install-count.pipe';
import { RatingComponent } from './components/rating.component';
import { NotFoundComponent } from './components/not-found.component';

@NgModule({
  declarations: [
    DefaultIconPipe,
    InstallCountPipe,
    RatingComponent,
    NotFoundComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    FlexLayoutModule,
    MatCardModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatPaginatorModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  exports: [
    CommonModule,
    FormsModule,
    FlexLayoutModule,
    MatCardModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatPaginatorModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    RatingComponent,
    DefaultIconPipe,
    InstallCountPipe,
    MatTooltipModule,
    NotFoundComponent
  ]
})
export class SharedModule {
  constructor(matIconRegistry: MatIconRegistry) {
    matIconRegistry.registerFontClassAlias('materialdesignicons', 'mdi');
  }
}
