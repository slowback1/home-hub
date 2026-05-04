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
	await request.put(`${BACKEND_URL}/api/system-config/weather/provider`, {
		data: { value: provider }
	});
});

When('I navigate to the weather page', async ({ weatherPage }) => {
	await weatherPage.goto();
});

When(
	'I navigate to the weather page and the weather API returns an error',
	async ({ weatherPage }) => {
		await weatherPage.gotoWithApiError();
	}
);

Then('I should see a temperature value', async ({ weatherPage }) => {
	expect(await weatherPage.hasTemperatureValue()).toBe(true);
});

Then('I should see a condition description', async ({ weatherPage }) => {
	expect(await weatherPage.hasConditionDescription()).toBe(true);
});

Then('I should see a humidity value', async ({ weatherPage }) => {
	expect(await weatherPage.hasHumidityValue()).toBe(true);
});

Then('I should see a wind speed value', async ({ weatherPage }) => {
	expect(await weatherPage.hasWindSpeedValue()).toBe(true);
});

Then('I should see a {string} message', async ({ weatherPage }, message: string) => {
	expect(await weatherPage.hasMessage(message)).toBe(true);
});

Given(
	'the {string} feature flag is disabled',
	async () => {
		// handled by @weather-feature-flag-hidden Before hook
	}
);

Then('I should be redirected or see a not-found state', async ({ weatherPage }) => {
	expect(await weatherPage.isNotFoundState()).toBe(true);
});
