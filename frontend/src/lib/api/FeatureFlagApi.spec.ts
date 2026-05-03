import { mockApi } from '$lib/testHelpers/getFetchMock';
import FeatureFlagApi from './FeatureFlagApi';

const mockFlag = { name: 'DEMO_FEATURE_FLAG', isEnabled: false };

describe('FeatureFlagApi', () => {
	it('getAll calls GET /api/feature-flags and returns FeatureFlag[]', async () => {
		const mockFetch = mockApi({ '/api/feature-flags': [mockFlag] });
		const api = new FeatureFlagApi();

		const result = await api.getAll();

		expect(mockFetch).toHaveBeenCalled();
		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/feature-flags');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([mockFlag]);
	});

	it('toggle calls PATCH /api/feature-flags/{name} with isEnabled body', async () => {
		const updated = { ...mockFlag, isEnabled: true };
		const mockFetch = mockApi({ '/api/feature-flags/DEMO_FEATURE_FLAG': updated });
		const api = new FeatureFlagApi();

		const result = await api.toggle('DEMO_FEATURE_FLAG', true);

		expect(mockFetch).toHaveBeenCalled();
		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/feature-flags/DEMO_FEATURE_FLAG');
		expect(options.method).toEqual('PATCH');
		expect(JSON.parse(options.body as string)).toEqual({ isEnabled: true });
		expect(result).toEqual(updated);
	});
});
