import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';
const FLAG = 'SAVED_ITEMS_ENABLED';

Before({ tags: '@saved-items' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/saved-items`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
});

After({ tags: '@saved-items' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/saved-items`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
});

Given('the SAVED_ITEMS_ENABLED feature flag is enabled', async ({ request }) => {
	const response = await request.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, {
		data: { isEnabled: true }
	});
	if (!response.ok()) throw new Error(`Failed to enable ${FLAG}: ${response.status()}`);
});

Given('the SAVED_ITEMS_ENABLED feature flag is disabled', async ({ request }) => {
	const response = await request.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, {
		data: { isEnabled: false }
	});
	if (!response.ok()) throw new Error(`Failed to disable ${FLAG}: ${response.status()}`);
});

Given('a saved item named {string} exists', async ({ request, testUser }, name) => {
	const response = await request.post(`${BACKEND_URL}/api/saved-items`, {
		data: { name },
		headers: { 'X-User-Id': testUser.id }
	});
	if (!response.ok()) throw new Error(`Failed to seed saved item: ${response.status()}`);
});

When('I visit the Saved Items page', async ({ savedItemsPage }) => {
	await savedItemsPage.goto();
});

When('I create a saved item named {string}', async ({ savedItemsPage }, name) => {
	await savedItemsPage.createItem(name);
});

When(
	'I edit the saved item {string} to {string}',
	async ({ savedItemsPage }, currentName, newName) => {
		await savedItemsPage.editItem(currentName, newName);
	}
);

Then('I see a saved item named {string}', async ({ savedItemsPage }, name) => {
	await savedItemsPage.expectItemRow(name);
});

Then('I do not see a saved item named {string}', async ({ savedItemsPage }, name) => {
	await savedItemsPage.expectNoItemRow(name);
});

Then('I see the saved items empty state', async ({ savedItemsPage }) => {
	await savedItemsPage.page.waitForSelector('[data-testid="saved-items-empty-state"]', {
		timeout: 5000
	});
	expect(await savedItemsPage.isEmptyStateVisible()).toBe(true);
});

Then('the Saved Items nav item is not visible in the sidebar', async ({ dashboardPage }) => {
	await expect(dashboardPage.page.locator('[data-testid="nav-item-saved-items"]')).toBeHidden();
});
