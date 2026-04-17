import type IFeatureFlagProvider from '$lib/services/FeatureFlag/IFeatureFlagProvider';
import type { FeatureFlag } from '$lib/services/FeatureFlag/IFeatureFlagProvider';
import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';

export default class FeatureFlagService {
	public static featureFlags: FeatureFlag[] = [];

	public static async initialize(provider: IFeatureFlagProvider) {
		this.featureFlags = await provider.getFeatureFlags().catch(() => []);

		MessageBus.sendMessage(Messages.FeatureFlagsChanged, true);
	}

	private static isFeatureEnabled(featureFlagName: string): boolean {
		const featureFlag = this.featureFlags.find((flag) => flag.name === featureFlagName);

		return featureFlag?.isEnabled ?? false;
	}

	public static subscribeToFeature(
		featureFlagName: string,
		callback: (isEnabled: boolean) => void
	) {
		return MessageBus.subscribe(Messages.FeatureFlagsChanged, () => {
			const isEnabled = this.isFeatureEnabled(featureFlagName);

			callback(isEnabled);
		});
	}
}
