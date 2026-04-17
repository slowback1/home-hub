import { getTestApplicationConfig } from '$lib/testHelpers/testData/testApplicationConfig';
import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';
import ConfigFeatureFlagProvider from '$lib/services/FeatureFlag/ConfigFeatureFlagProvider';
import { beforeEach } from 'vitest';
import { createTestFeatureFlag } from '$lib/testHelpers/testFeatureFlagProvider';
import type { FeatureFlag } from '$lib/services/FeatureFlag/IFeatureFlagProvider';

describe('ConfigFeatureFlagProvider', () => {
	let provider: ConfigFeatureFlagProvider;

	describe('when there are no feature flags in the config', () => {
		beforeEach(() => {
			const config = getTestApplicationConfig();
			config.featureFlags = undefined;

			MessageBus.sendMessage(Messages.ApplicationConfig, config);

			provider = new ConfigFeatureFlagProvider();
		});

		it("returns an empty array if there aren't any feature flags in the config", async () => {
			const result = await provider.getFeatureFlags();

			expect(result).toEqual([]);
		});
	});

	describe('when there are feature flags in the config', () => {
		let featureFlags: FeatureFlag[];

		beforeEach(() => {
			featureFlags = [createTestFeatureFlag('test1'), createTestFeatureFlag('test2', false)];

			const config = getTestApplicationConfig({ featureFlags });

			MessageBus.sendMessage(Messages.ApplicationConfig, config);
		});

		it('returns the feature flags from the config', async () => {
			provider = new ConfigFeatureFlagProvider();

			const result = await provider.getFeatureFlags();

			expect(result).toEqual(featureFlags);
		});
	});

	describe.each([
		{},
		'',
		null,
		undefined,
		1234,
		true,
		[{ a: 1, b: 2 }],
		[1, 2, 3, 4],
		[{ name: 'string', isEnabled: false }, { a: 1 }]
	])('when the config is not a feature flag array', (config: never) => {
		beforeEach(() => {
			MessageBus.sendMessage(
				Messages.ApplicationConfig,
				getTestApplicationConfig({ featureFlags: config })
			);
			provider = new ConfigFeatureFlagProvider();
		});

		it('returns an empty array if the config is not a feature flag array', async () => {
			const result = await provider.getFeatureFlags();

			expect(result).toEqual([]);
		});
	});
});
