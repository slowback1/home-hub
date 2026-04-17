import { beforeEach } from 'vitest';
import UrlPathProvider, { RealUrlProvider, TestUrlProvider } from '$lib/providers/urlPathProvider';

describe('UrlPathProvider', () => {
	describe('Test Url Path Provider', () => {
		beforeEach(() => {
			UrlPathProvider.initialize(new TestUrlProvider('/test'));
		});

		it.each([
			['/test', true],
			['/not-it', false],
			['/test/1', true],
			['/', false]
		])('correctly matches the url (%s, %s)', (path, expected) => {
			const result = UrlPathProvider.matchesPath(path);

			expect(result).toEqual(expected);
		});
	});

	describe('Real Url Path Provider', () => {
		beforeEach(() => {
			global.location.pathname = '/';

			UrlPathProvider.initialize(new RealUrlProvider());
		});

		it.each([
			['/test', false],
			['/not-it', false],
			['/test/1', false],
			['/', true]
		])('correctly matches the url (%s, %s)', (path, expected) => {
			const result = UrlPathProvider.matchesPath(path);

			expect(result).toEqual(expected);
		});
	});
});
