import { DefaultIconPipe } from './default-icon.pipe';

const fallbackUrl = 'assets/images/default_icon.png';

describe('DefaultIconPipe', () => {
  it('should return value as is if it take not empty string', () => {
    const pipe = new DefaultIconPipe();

    const result = pipe.transform('not_empty_string');

    expect(result).toBe('not_empty_string');
  });

  it('should return fallback url if it takes an empty string', () => {
    const pipe = new DefaultIconPipe();

    const result = pipe.transform('');

    expect(result).toBe(fallbackUrl);
  });

  it('should return fallback url if it takes null', () => {
    const pipe = new DefaultIconPipe();

    const result = pipe.transform(null);

    expect(result).toBe(fallbackUrl);
  });

  it('should return fallback url if it takes undefined', () => {
    const pipe = new DefaultIconPipe();

    const result = pipe.transform(undefined);

    expect(result).toBe(fallbackUrl);
  });
});
