import { ExtensionInfo } from './extension-info.model';
import { Theme } from './theme.model';

export interface Extension extends ExtensionInfo {
  themes: Theme[];
}
