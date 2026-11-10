import { When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';
const SHOPPING_FLAG = 'SHOPPING_LISTS_ENABLED';
const SAVED_FLAG = 'SAVED_ITEMS_ENABLED';

async function resetState(request: {
	delete: (url: string) => Promise<{ ok: () => boolean }>;
	patch: (url: string, options: { data: unknown }) => Promise<{ ok: () => boolean }>;
}): Promise<void> {
	await request.delete(`${BACKEND_URL}/api/test/shopping-lists`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/saved-items`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${SHOPPING_FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${SAVED_FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
}

Before({ tags: '@saved-items-to-shopping-lists' }, async ({ request }) => {
	await resetState(request);
});

After({ tags: '@saved-items-to-shopping-lists' }, async ({ request }) => {
	await resetState(request);
});

When(
	'I type {string} in the add-item input of the {string} list',
	async ({ shoppingListsPage }, text, listName) => {
		await shoppingListsPage.typeItemDraft(listName, text);
	}
);

Then('I see a saved item suggestion named {string}', async ({ shoppingListsPage }, name) => {
	await shoppingListsPage.expectSuggestion(name);
});

Then('I do not see a saved item suggestion named {string}', async ({ shoppingListsPage }, name) => {
	await shoppingListsPage.expectNoSuggestion(name);
});

When(
	'I pick the saved item suggestion {string} in the {string} list',
	async ({ shoppingListsPage }, name, listName) => {
		await shoppingListsPage.pickSuggestion(name);
		await shoppingListsPage.expectItem(name);
	}
);
