import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

Before({ tags: '@weather-feature-flag-hidden' }, async ({ request }) => {
	await request.patch(`${BACKEND_URL}/api/feature-flags/WEATHER_ENABLED`, {
		data: { isEnabled: false }
	});
});

After({ tags: '@weather-feature-flag-hidden' }, async ({ request }) => {
	await request.patch(`${BACKEND_URL}/api/feature-flags/WEATHER_ENABLED`, {
		data: { isEnabled: true }
	});
});

Given('the weather provider is {string}', async ({ request }, provider: string) => {
	throw new Error('not implemented');
});

When('I navigate to the weather page', async ({ weatherPage }) => {
	throw new Error('not implemented');
});

When(
	'I navigate to the weather page and the weather API returns an error',
	async ({ weatherPage }) => {
		throw new Error('not implemented');
	}
);

Then('I should see a temperature value', async ({ weatherPage }) => {
	throw new Error('not implemented');
});

Then('I should see a condition description', async ({ weatherPage }) => {
	throw new Error('not implemented');
});

Then('I should see a humidity value', async ({ weatherPage }) => {
	throw new Error('not implemented');
});

Then('I should see a wind speed value', async ({ weatherPage }) => {
	throw new Error('not implemented');
});

Then('I should see a {string} message', async ({ weatherPage }, message: string) => {
	throw new Error('not implemented');
});

Given(
	'the {string} feature flag is disabled',
	async ({ request }, flagName: string) => {
		// handled by Before hook for @weather-feature-flag-hidden
	}
);

Then('I should be redirected or see a not-found state', async ({ weatherPage }) => {
	throw new Error('not implemented');
});
