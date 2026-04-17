import TestFeatureFlagProvider, {
	createTestFeatureFlag
} from '$lib/testHelpers/testFeatureFlagProvider';

describe('testFeatureFlagProvider', () => {
	it('returns the given feature flags', async () => {
		const featureFlags = [createTestFeatureFlag('test1'), createTestFeatureFlag('test2', false)];

		const provider = new TestFeatureFlagProvider(featureFlags);

		const result = await provider.getFeatureFlags();

		expect(result).toEqual(featureFlags);
	});

	it('the getFeatureFlags method is a mock that can be asserted against', async () => {
		const featureFlags = [createTestFeatureFlag('test1'), createTestFeatureFlag('test2', false)];

		const provider = new TestFeatureFlagProvider(featureFlags);

		const result = await provider.getFeatureFlags();

		expect(result).toEqual(featureFlags);
		expect(provider.getFeatureFlagsMock).toHaveBeenCalled();
	});
});
