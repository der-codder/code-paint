import { Component, Input, ChangeDetectionStrategy } from '@angular/core';

const FullStar = 'mdi-star';
const HalfStar = 'mdi-star-half';
const EmptyStar = 'mdi-star-outline';

@Component({
  selector: 'cp-rating',
  template: `
    <span [matTooltip]="tooltip" [attr.aria-label]="tooltip">
      <mat-icon fontSet="mdi" [fontIcon]="stars[0]" color="accent"></mat-icon>
      <mat-icon fontSet="mdi" [fontIcon]="stars[1]" color="accent"></mat-icon>
      <mat-icon fontSet="mdi" [fontIcon]="stars[2]" color="accent"></mat-icon>
      <mat-icon fontSet="mdi" [fontIcon]="stars[3]" color="accent"></mat-icon>
      <mat-icon fontSet="mdi" [fontIcon]="stars[4]" color="accent"></mat-icon>
    </span>
  `,
  styles: [`
    .mat-icon {
      height: 16px;
      width: 16px;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RatingComponent {
  private _rating: number;
  public get rating(): number {
    return this._rating;
  }
  @Input()
  public set rating(v: number) {
    this._rating = v;

    this.updateTooltip()
    this.updateStars();
  }

  private _ratingCount: number;
  public get ratingCount(): number {
    return this._ratingCount;
  }
  @Input()
  public set ratingCount(value: number) {
    this._ratingCount = value;

    this.updateTooltip();
  }

  tooltip: string;

  stars = [EmptyStar, EmptyStar, EmptyStar, EmptyStar, EmptyStar];

  private updateStars() {
    for (let i = 1; i <= 5; i++) {
      if (this.rating >= i) {
        this.stars[i - 1] = FullStar;
      } else if (this.rating >= i - 0.5) {
        this.stars[i - 1] = HalfStar;
      } else {
        this.stars[i - 1] = EmptyStar;
      }
    }
  }

  private updateTooltip() {
    this.tooltip = `Average rating: ${this.rating.toFixed(1)} (${this.ratingCount} ratings).`;
  }

  private round(number: number) {
    return Math.round(number * 2) / 2;
  }
}
