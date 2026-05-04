import { expect } from '@playwright/test';
import { Given, When, Then, Before } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

// Reset E2E seed data before each admin scenario so tests don't bleed state
// into each other via the in-memory DictionarySystemConfigProvider singleton.
Before({ tags: '@admin' }, async ({ request }) => {
	await request.put(`${BACKEND_URL}/api/system-config/weather/zip_code`, {
		data: { value: '10001' }
	});
	await request.put(`${BACKEND_URL}/api/system-config/weather/api_key`, {
		data: { value: 'test-api-key-1' }
	});
	await request.put(`${BACKEND_URL}/api/system-config/weather/provider`, {
		data: { value: 'mock' }
	});
});

Given('I navigate to the admin system config page', async ({ systemConfigPage }) => {
	await systemConfigPage.goto();
});

Then('I should see a {string} namespace section', async ({ systemConfigPage }, namespace: string) => {
	const visible = await systemConfigPage.hasNamespaceSection(namespace);
	expect(visible).toBe(true);
});

Then(
	'I should see a {string} entry with value {string}',
	async ({ systemConfigPage }, key: string, value: string) => {
		const actual = await systemConfigPage.getEntryValue(key);
		expect(actual).toBe(value);
	}
);

Then('I should see an {string} entry with a masked value', async ({ systemConfigPage }, key: string) => {
	const masked = await systemConfigPage.isEntryMasked(key);
	expect(masked).toBe(true);
});

When('I click on the {string} value', async ({ systemConfigPage }, key: string) => {
	await systemConfigPage.clickEntryValue(key);
});

Then('an inline text input with Save and Cancel buttons should appear', async ({ systemConfigPage }) => {
	const visible = await systemConfigPage.isEditModeVisible('zip_code');
	expect(visible).toBe(true);
});

When('I type {string} and click Save', async ({ systemConfigPage }, value: string) => {
	await systemConfigPage.typeValueAndSave(value);
});

Then(
	'the {string} entry should display {string}',
	async ({ systemConfigPage }, key: string, value: string) => {
		const actual = await systemConfigPage.getEntryValue(key);
		expect(actual).toBe(value);
	}
);

Then('I should see a success toast', async ({ systemConfigPage }) => {
	const visible = await systemConfigPage.hasSuccessToast();
	expect(visible).toBe(true);
});

When(
	'I click on the {string} value and type {string}',
	async ({ systemConfigPage }, key: string, value: string) => {
		await systemConfigPage.clickEntryValueAndType(key, value);
	}
);

When('I click Cancel', async ({ systemConfigPage }) => {
	await systemConfigPage.clickCancel();
});

Then('the {string} value should be masked', async ({ systemConfigPage }, key: string) => {
	const masked = await systemConfigPage.isEntryMasked(key);
	expect(masked).toBe(true);
});

When('I click the show toggle on the {string} entry', async ({ systemConfigPage }, key: string) => {
	await systemConfigPage.clickShowToggle(key);
});

Then(
	'the {string} value should display {string}',
	async ({ systemConfigPage }, key: string, value: string) => {
		const actual = await systemConfigPage.getEntryValue(key);
		expect(actual).toBe(value);
	}
);

When(
	'I click on the {string} value and the API returns an error on save',
	async ({ systemConfigPage }, key: string) => {
		await systemConfigPage.clickEntryValueWithErrorOnSave(key);
	}
);

Then('I should see an error toast', async ({ systemConfigPage }) => {
	const visible = await systemConfigPage.hasErrorToast();
	expect(visible).toBe(true);
});

Then('the input should remain open', async ({ systemConfigPage }) => {
	const open = await systemConfigPage.isInputOpen();
	expect(open).toBe(true);
});
