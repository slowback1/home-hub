import { slugify } from '$lib/utils/stringUtils';

describe('String Utilities', () => {
	describe('slugify', () => {
		it.each([
			['test', 'test'],
			['abc 123', 'abc-123'],
			['My Cool Sentence', 'my-cool-sentence'],
			['    abc   ', 'abc'],
			['söme stüff with áccènts', 'some-stuff-with-accents']
		])('with input %s gets output %s', (input, expectedOutput) => {
			const result = slugify(input);

			expect(result).toEqual(expectedOutput);
		});
	});
});
