
export interface Theme {
  label: string;
  themeType: string;
  tokenColors: TokenColor[];
}

export interface TokenColor {
  name: string;
  scope: string;
  settings: Settings;
}

export interface Settings {
  foreground: string;
  fontStyle: string;
}
