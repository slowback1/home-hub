import type { ApplicationConfig } from '$lib/services/Config/ConfigService';

export const getTestApplicationConfig = (
	overrides: Partial<ApplicationConfig> = {}
): ApplicationConfig => ({
	baseUrl: 'baseUrl',
	featureFlags: [],
	...overrides
});
