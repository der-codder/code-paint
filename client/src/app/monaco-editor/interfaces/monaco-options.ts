import { EditorMinimapOptions } from './editor-minimap-options';

/**
 * Configuration options for the editor.
 */
export interface MonacoOptions {
  /**
   * The initial language of the auto created model in the editor.
   * To not create automatically a model, use `model: null`.
   */
  language: string;

  /**
   * Control the width of line numbers, by reserving horizontal space for rendering at least an amount of digits.
   * Defaults to 5.
   */
  lineNumbers?: boolean;
  /**
   * Initial theme to be used for rendering.
   * The current out-of-the-box available themes are: 'vs' (default), 'vs-dark', 'hc-black'.
   * You can create custom themes via `monaco.editor.defineTheme`.
   * To switch a theme, use `monaco.editor.setTheme`
   */
  theme?: string;
  /**
   * Should the editor be read only.
   * Defaults to false.
   */
  readOnly?: boolean;
  /**
   * Enable that scrolling can go one screen size after the last line.
   * Defaults to true.
   */
  scrollBeyondLastLine?: boolean;
  /**
   * Enable that the editor will install an interval to check if its container dom node size has changed.
   * Enabling this might have a severe performance impact.
   * Defaults to false.
   */
  automaticLayout?: boolean;
  /**
   * Control the behavior and rendering of the minimap.
   */
  minimap?: EditorMinimapOptions;
}
