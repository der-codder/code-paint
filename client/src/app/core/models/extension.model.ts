import { ExtensionInfo } from './extension-info.model';
import { Theme } from '..';

export interface Extension extends ExtensionInfo {
  themes: Theme[];
}
