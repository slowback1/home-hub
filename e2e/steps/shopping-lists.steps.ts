import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';
const FLAG = 'SHOPPING_LISTS_ENABLED';

function toItemList(csv: string): string[] {
	return csv
		.split(',')
		.map((item) => item.trim())
		.filter((item) => item.length > 0);
}

Before({ tags: '@shopping-lists' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/shopping-lists`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
});

After({ tags: '@shopping-lists' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/shopping-lists`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
});

Given('the SHOPPING_LISTS_ENABLED feature flag is enabled', async ({ request }) => {
	const response = await request.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, {
		data: { isEnabled: true }
	});
	if (!response.ok()) throw new Error(`Failed to enable ${FLAG}: ${response.status()}`);
});

Given('the SHOPPING_LISTS_ENABLED feature flag is disabled', async ({ request }) => {
	const response = await request.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, {
		data: { isEnabled: false }
	});
	if (!response.ok()) throw new Error(`Failed to disable ${FLAG}: ${response.status()}`);
});

Given(
	'a shopping list named {string} exists with items {string}',
	async ({ request, testUser }, name, items) => {
		const response = await request.post(`${BACKEND_URL}/api/shopping-lists`, {
			data: { name },
			headers: { 'X-User-Id': testUser.id }
		});
		if (!response.ok()) throw new Error(`Failed to seed list: ${response.status()}`);
		const created = await response.json();
		for (const text of toItemList(items)) {
			await request.post(`${BACKEND_URL}/api/shopping-lists/${created.id}/items`, {
				data: { text },
				headers: { 'X-User-Id': testUser.id }
			});
		}
	}
);

When('I visit the Shopping Lists page', async ({ shoppingListsPage }) => {
	await shoppingListsPage.goto();
});

When('I create a shopping list named {string}', async ({ shoppingListsPage }, name) => {
	await shoppingListsPage.createList(name);
});

When('I add item {string} to the {string} list', async ({ shoppingListsPage }, text, listName) => {
	await shoppingListsPage.addItem(listName, text);
});

When('I toggle item {string} in the {string} list', async ({ shoppingListsPage }, text, listName) => {
	await shoppingListsPage.toggleItem(listName, text);
});

When('I complete the {string} list', async ({ shoppingListsPage }, name) => {
	await shoppingListsPage.completeList(name);
});

Then('I see a shopping list named {string}', async ({ shoppingListsPage }, name) => {
	await shoppingListsPage.expectListRow(name);
});

Then('the {string} list shows item {string}', async ({ shoppingListsPage }, _listName, text) => {
	await shoppingListsPage.expectItem(text);
});

Then(
	'item {string} in the {string} list is crossed out',
	async ({ shoppingListsPage }, text, _listName) => {
		expect(await shoppingListsPage.isItemChecked(text)).toBe(true);
	}
);

Then(
	'item {string} in the {string} list is not crossed out',
	async ({ shoppingListsPage }, text, _listName) => {
		expect(await shoppingListsPage.isItemChecked(text)).toBe(false);
	}
);

Then('I do not see an active shopping list named {string}', async ({ shoppingListsPage }, name) => {
	await shoppingListsPage.expectNoListRow(name);
});

Then('I see an archived shopping list named {string}', async ({ shoppingListsPage }, name) => {
	await shoppingListsPage.expectArchivedRow(name);
});

Then(
	'the archived {string} list shows item {string}',
	async ({ shoppingListsPage }, _listName, text) => {
		await shoppingListsPage.expectArchivedItem(text);
	}
);

Then('I see the shopping lists empty state', async ({ shoppingListsPage }) => {
	await shoppingListsPage.page.waitForSelector('[data-testid="shopping-lists-empty-state"]', {
		timeout: 5000
	});
	expect(await shoppingListsPage.isEmptyStateVisible()).toBe(true);
});

Then('the Shopping Lists nav item is not visible in the sidebar', async ({ dashboardPage }) => {
	await expect(dashboardPage.page.locator('[data-testid="nav-item-shopping-lists"]')).toBeHidden();
});
