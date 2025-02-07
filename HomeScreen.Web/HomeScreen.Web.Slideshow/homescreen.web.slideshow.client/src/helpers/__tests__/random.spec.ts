import { describe, expect, it } from 'vitest';
import { choiceRNG, rangeRNG } from '../random';

describe('random', () => {
    const random = () => 0.5;

    it('should return a valid number in a range', () => {
        const result = rangeRNG(0, 2, random);

        expect(result).toBe(1);
    });

    it('should return a valid item in a choice', () => {
        const items = [0, 1, 2];
        const result = choiceRNG(items, random);

        expect(result).toBe(1);
    });
});
