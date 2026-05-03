import BaseApi from './baseApi';
import type { FeatureFlag } from '$lib/services/FeatureFlag/IFeatureFlagProvider';

export default class FeatureFlagApi extends BaseApi {
	constructor() {
		super();
	}

	async getAll(): Promise<FeatureFlag[]> {
		return this.Get<FeatureFlag[]>('/api/feature-flags');
	}

	async toggle(name: string, isEnabled: boolean): Promise<FeatureFlag> {
		return this.Patch<FeatureFlag>(`/api/feature-flags/${name}`, { isEnabled });
	}
}
