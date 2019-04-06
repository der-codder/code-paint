export type BuiltinTheme = 'vs' | 'vs-dark' | 'hc-black';

export interface TokenThemeRule {
  token: string;
  foreground?: string;
  background?: string;
  fontStyle?: string;
}

export interface Colors {
  [colorId: string]: string;
}

export interface StandaloneThemeData {
  name: string;
  base: BuiltinTheme;
  inherit: boolean;
  rules: TokenThemeRule[];
  colors: Colors;
}
