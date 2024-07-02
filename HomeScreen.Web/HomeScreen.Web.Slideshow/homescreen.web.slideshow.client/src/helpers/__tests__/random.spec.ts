import { describe, expect, it } from 'vitest';
import { _choice, _range } from '../random';

describe('random', () => {
  const random = () => 0.5;

  it('should return a valid number in a range', () => {
    const result = _range(0, 2, random);

    expect(result).toBe(1);
  });

  it('should return a valid item in a choice', () => {
    const items = [0, 1, 2];
    const result = _choice(items, random);

    expect(result).toBe(1);
  });
});
