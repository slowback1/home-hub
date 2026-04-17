import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';
import type { FeatureFlag } from '$lib/services/FeatureFlag/IFeatureFlagProvider';

export type ApplicationConfig = {
	baseUrl: string;
	featureFlags: FeatureFlag[];
};

export default class ConfigService {
	static initialize() {
		this.getConfigFromFile();
	}

	private static async getConfigFromFile() {
		const config: ApplicationConfig = await fetch('/config/config.json').then((res) => res.json());

		MessageBus.sendMessage(Messages.ApplicationConfig, config);
	}

	getConfig<T>(key: keyof ApplicationConfig): T {
		const currentConfig = MessageBus.getLastMessage<ApplicationConfig>(Messages.ApplicationConfig);

		if (!currentConfig) return undefined;

		return currentConfig[key] as T;
	}
}
