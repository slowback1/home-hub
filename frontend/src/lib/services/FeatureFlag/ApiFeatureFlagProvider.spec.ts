import { mockApi, getFetchMock } from '$lib/testHelpers/getFetchMock';
import ApiFeatureFlagProvider from '$lib/services/FeatureFlag/ApiFeatureFlagProvider';

const mockFlags = [
	{ name: 'DEMO_FEATURE_FLAG', isEnabled: false },
	{ name: 'WEATHER_ENABLED', isEnabled: true }
];

describe('ApiFeatureFlagProvider', () => {
	it('returns mapped flags from GET /api/feature-flags on success', async () => {
		mockApi({ '/api/feature-flags': mockFlags });
		const provider = new ApiFeatureFlagProvider();

		const result = await provider.getFeatureFlags();

		expect(result).toEqual(mockFlags);
	});

	it('returns an empty array when the request fails', async () => {
		getFetchMock(null, 500);
		const provider = new ApiFeatureFlagProvider();

		const result = await provider.getFeatureFlags();

		expect(result).toEqual([]);
	});
});
