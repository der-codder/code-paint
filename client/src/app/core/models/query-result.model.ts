import { ThemeInfo } from './theme-info.model';

export interface QueryResult {
  totalCount: number;
  themes: ThemeInfo[];
}
