import { Statistics } from './statistics.model';

export interface ExtensionInfo {
  id: string;
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
}
