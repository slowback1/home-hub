import { parseItems, pickRandom } from '$lib/utils/spinWheel';

describe('spinWheel', () => {
	describe('parseItems', () => {
		it('splits a newline-delimited string into items', () => {
			expect(parseItems('Pizza\nTacos\nSushi')).toEqual(['Pizza', 'Tacos', 'Sushi']);
		});

		it('trims whitespace and drops blank lines', () => {
			expect(parseItems('  Pizza  \n\n   \nTacos\n')).toEqual(['Pizza', 'Tacos']);
		});

		it('handles carriage returns (CRLF)', () => {
			expect(parseItems('Pizza\r\nTacos')).toEqual(['Pizza', 'Tacos']);
		});

		it('preserves duplicates', () => {
			expect(parseItems('Pizza\nPizza\nTacos')).toEqual(['Pizza', 'Pizza', 'Tacos']);
		});

		it('returns an empty array for an empty or whitespace-only string', () => {
			expect(parseItems('')).toEqual([]);
			expect(parseItems('   \n  \n')).toEqual([]);
		});
	});

	describe('pickRandom', () => {
		it('returns null for an empty list', () => {
			expect(pickRandom([])).toBeNull();
		});

		it('returns the single element for a one-item list', () => {
			expect(pickRandom(['Pizza'])).toBe('Pizza');
		});

		it('selects by the injected random function', () => {
			const list = ['a', 'b', 'c'];
			expect(pickRandom(list, () => 0)).toBe('a');
			expect(pickRandom(list, () => 0.5)).toBe('b');
			expect(pickRandom(list, () => 0.99)).toBe('c');
		});

		it('never returns out of range even when random returns ~1', () => {
			const list = ['a', 'b', 'c'];
			expect(pickRandom(list, () => 0.999999999)).toBe('c');
		});

		it('always returns a member of the list across many iterations', () => {
			const list = ['a', 'b', 'c', 'd'];
			for (let i = 0; i < 1000; i++) {
				expect(list).toContain(pickRandom(list));
			}
		});
	});
});
