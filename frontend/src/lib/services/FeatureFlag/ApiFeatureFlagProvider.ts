import BaseApi from '$lib/api/baseApi';
import type IFeatureFlagProvider from '$lib/services/FeatureFlag/IFeatureFlagProvider';
import type { FeatureFlag } from '$lib/services/FeatureFlag/IFeatureFlagProvider';

export default class ApiFeatureFlagProvider extends BaseApi implements IFeatureFlagProvider {
	constructor() {
		super();
	}

	async getFeatureFlags(): Promise<FeatureFlag[]> {
		const result = await this.Get<FeatureFlag[]>('/api/feature-flags').catch(() => []);
		return Array.isArray(result) ? result : [];
	}
}
