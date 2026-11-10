import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';
const FLAG = 'PREMADE_LISTS_ENABLED';
const SHOPPING_LISTS_FLAG = 'SHOPPING_LISTS_ENABLED';

function toItemList(csv: string): string[] {
	return csv
		.split(',')
		.map((item) => item.trim())
		.filter((item) => item.length > 0);
}

Before({ tags: '@premade-lists' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/premade-lists`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/shopping-lists`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${SHOPPING_LISTS_FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
});

After({ tags: '@premade-lists' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/premade-lists`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/shopping-lists`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${SHOPPING_LISTS_FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
});

Given('the PREMADE_LISTS_ENABLED feature flag is enabled', async ({ request }) => {
	const response = await request.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, {
		data: { isEnabled: true }
	});
	if (!response.ok()) throw new Error(`Failed to enable ${FLAG}: ${response.status()}`);
});

Given('the PREMADE_LISTS_ENABLED feature flag is disabled', async ({ request }) => {
	const response = await request.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, {
		data: { isEnabled: false }
	});
	if (!response.ok()) throw new Error(`Failed to disable ${FLAG}: ${response.status()}`);
});

Given(
	'a premade list named {string} with saved items {string} exists',
	async ({ request, testUser }, name, savedItems) => {
		const listResponse = await request.post(`${BACKEND_URL}/api/premade-lists`, {
			data: { name },
			headers: { 'X-User-Id': testUser.id }
		});
		if (!listResponse.ok()) throw new Error(`Failed to seed premade list: ${listResponse.status()}`);
		const created = await listResponse.json();
		for (const itemName of toItemList(savedItems)) {
			const savedResponse = await request.post(`${BACKEND_URL}/api/saved-items`, {
				data: { name: itemName },
				headers: { 'X-User-Id': testUser.id }
			});
			if (!savedResponse.ok())
				throw new Error(`Failed to seed saved item: ${savedResponse.status()}`);
			const saved = await savedResponse.json();
			const membershipResponse = await request.post(
				`${BACKEND_URL}/api/premade-lists/${created.id}/items`,
				{
					data: { savedItemId: saved.id },
					headers: { 'X-User-Id': testUser.id }
				}
			);
			if (!membershipResponse.ok())
				throw new Error(`Failed to seed membership: ${membershipResponse.status()}`);
		}
	}
);

When('I visit the Premade Lists page', async ({ premadeListsPage }) => {
	await premadeListsPage.goto();
});

When('I create a premade list named {string}', async ({ premadeListsPage }, name) => {
	await premadeListsPage.createList(name);
});

When(
	'I add the saved item {string} to the premade list {string}',
	async ({ premadeListsPage }, savedItemName, listName) => {
		await premadeListsPage.addSavedItem(listName, savedItemName);
	}
);

When(
	'I create a shopping list named {string} from the premade list {string}',
	async ({ shoppingListsPage }, name, premadeName) => {
		await shoppingListsPage.createListFromPremade(name, premadeName);
	}
);

Then('I see a premade list named {string}', async ({ premadeListsPage }, name) => {
	await premadeListsPage.expectListRow(name);
});

Then(
	'the premade list {string} shows saved item {string}',
	async ({ premadeListsPage }, listName, savedItemName) => {
		await premadeListsPage.expectSavedItem(listName, savedItemName);
	}
);

Then('the Premade Lists nav item is not visible in the sidebar', async ({ dashboardPage }) => {
	await expect(dashboardPage.page.locator('[data-testid="nav-item-premade-lists"]')).toBeHidden();
});
