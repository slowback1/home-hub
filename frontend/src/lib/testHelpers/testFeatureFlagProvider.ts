import type { FeatureFlag } from '$lib/services/FeatureFlag/IFeatureFlagProvider';
import type IFeatureFlagProvider from '$lib/services/FeatureFlag/IFeatureFlagProvider';

export default class TestFeatureFlagProvider implements IFeatureFlagProvider {
	constructor(private featureFlags: FeatureFlag[]) {}

	getFeatureFlagsMock = vi.fn(() => {
		return Promise.resolve(this.featureFlags);
	});

	getFeatureFlags = this.getFeatureFlagsMock;
}

export function createTestFeatureFlag(name: string, isEnabled: boolean = true): FeatureFlag {
	return {
		name,
		isEnabled: isEnabled
	};
}
