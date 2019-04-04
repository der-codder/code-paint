import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'defaultIcon'
})
export class DefaultIconPipe implements PipeTransform {

  transform(value: string, args?: any): string {
    return !value || value === ''
      ? 'assets/images/default_icon.png'
      : value;
  }

}
