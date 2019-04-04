import { ExtensionInfo } from './extension-info.model';

export interface QueryResult {
  totalCount: number;
  items: ExtensionInfo[];
}
