import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'installCount'
})
export class InstallCountPipe implements PipeTransform {

  transform(value: number, short: boolean = false): string {
    let installLabel: string;

    if (short) {
      if (value > 1000000) {
        installLabel = `${Math.floor(value / 100000) / 10}M`;
      } else if (value > 1000) {
        installLabel = `${Math.floor(value / 1000)}K`;
      } else {
        installLabel = String(value);
      }
    } else {
      installLabel = value.toLocaleString();
    }

    return installLabel;
  }

}
