import { expect } from '@playwright/test';
import { Given, When, Then, Before } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

Before({ tags: '@feature-flags-toggle-off' }, async ({ request }) => {
	await request.patch(`${BACKEND_URL}/api/feature-flags/DEMO_FEATURE_FLAG`, {
		data: { isEnabled: true }
	});
});

Before({ tags: '@feature-flags-page-loads or @feature-flags-toggle-on' }, async ({ request }) => {
	await request.patch(`${BACKEND_URL}/api/feature-flags/DEMO_FEATURE_FLAG`, {
		data: { isEnabled: false }
	});
});

Given('I navigate to the admin feature flags page', async ({ featureFlagsPage }) => {
	await featureFlagsPage.goto();
});

Then('I should see the page heading {string}', async ({ featureFlagsPage }, heading: string) => {
	const visible = await featureFlagsPage.hasHeading(heading);
	expect(visible).toBe(true);
});

Then('I should see a flag row for {string}', async ({ featureFlagsPage }, name: string) => {
	const visible = await featureFlagsPage.hasFlagRow(name);
	expect(visible).toBe(true);
});

Then('the toggle for {string} should be off', async ({ featureFlagsPage }, name: string) => {
	const on = await featureFlagsPage.isToggleOn(name);
	expect(on).toBe(false);
});

Given('the toggle for {string} is off', async ({ featureFlagsPage }, name: string) => {
	const on = await featureFlagsPage.isToggleOn(name);
	expect(on).toBe(false);
});

When('I toggle the switch for {string} on', async ({ featureFlagsPage }, name: string) => {
	await featureFlagsPage.toggleFlag(name);
});

Then('the toggle for {string} should be on', async ({ featureFlagsPage }, name: string) => {
	const on = await featureFlagsPage.isToggleOn(name);
	expect(on).toBe(true);
});

Given('the toggle for {string} is on', async ({ featureFlagsPage }, name: string) => {
	const on = await featureFlagsPage.isToggleOn(name);
	expect(on).toBe(true);
});

When('I toggle the switch for {string} off', async ({ featureFlagsPage }, name: string) => {
	await featureFlagsPage.toggleFlag(name);
});

When('I click the {string} tab', async ({ featureFlagsPage }, label: string) => {
	await featureFlagsPage.clickTab(label);
});

Then('I should be on the admin feature flags page', async ({ featureFlagsPage }) => {
	const path = await featureFlagsPage.currentPath();
	expect(path).toBe('/admin/feature-flags');
});

Then('I should be on the admin system config page', async ({ featureFlagsPage }) => {
	const path = await featureFlagsPage.currentPath();
	expect(path).toBe('/admin/system-config');
});
