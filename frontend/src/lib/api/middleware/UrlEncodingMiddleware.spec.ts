import UrlEncodingMiddleware from '$lib/api/middleware/UrlEncodingMiddleware';
import { getTestAPIRequest } from '$lib/testHelpers/testData/testAPIRequest';

describe('UrlEncodingMiddleware', () => {
	const middleware = new UrlEncodingMiddleware();

	it("Should not touch the url if it doesn't have a query string", async () => {
		const request = getTestAPIRequest({ url: 'http://example.com' });
		const result = await middleware.transformRequest(request);
		expect(result.url).toBe('http://example.com');
	});

	it('Should append query parameters to the url', async () => {
		const request = getTestAPIRequest({
			url: 'http://example.com',
			queryParameters: { key: 'value' }
		});
		const result = await middleware.transformRequest(request);
		expect(result.url).toBe('http://example.com?key=value');
	});

	it('Should append multiple query parameters to the url', async () => {
		const request = getTestAPIRequest({
			url: 'http://example.com',
			queryParameters: { key1: 'value1', key2: 'value2' }
		});
		const result = await middleware.transformRequest(request);
		expect(result.url).toBe('http://example.com?key1=value1&key2=value2');
	});

	it("should encode query parameters that aren't URL safe", async () => {
		const request = getTestAPIRequest({
			url: 'http://example.com',
			queryParameters: { key: 'value&stuff' }
		});
		const result = await middleware.transformRequest(request);
		expect(result.url).toBe('http://example.com?key=value%26stuff');
	});
});
