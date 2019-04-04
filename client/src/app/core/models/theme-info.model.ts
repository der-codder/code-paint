import { Statistics } from './statistics.model';

export class ThemeInfo {
  name: string;
  displayName: string;
  description: string;
  publisherName: string;
  publisherDisplayName: string;
  version: string;
  lastUpdated: Date;
  iconDefault: string;
  iconSmall: string;
  statistics: Statistics;

  get id(): string {
    return this.publisherName + '.' + this.name;
  }
}
