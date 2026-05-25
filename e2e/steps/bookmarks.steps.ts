import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

Before({ tags: '@bookmarks' }, async ({ request }) => {
	await request.patch(`${BACKEND_URL}/api/feature-flags/BOOKMARKS_ENABLED`, {
		data: { isEnabled: true }
	});
	await request.delete(`${BACKEND_URL}/api/test/bookmarks`);
});

After({ tags: '@bookmarks' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/bookmarks`);
	await request.patch(`${BACKEND_URL}/api/feature-flags/BOOKMARKS_ENABLED`, {
		data: { isEnabled: false }
	});
});

Given('I have saved bookmarks', async ({ request, bookmarksPage }) => {
	await request.post(`${BACKEND_URL}/api/bookmarks`, {
		data: { url: 'https://github.com', name: 'GitHub', description: 'Code hosting.' }
	});
	await request.post(`${BACKEND_URL}/api/bookmarks`, {
		data: { url: 'https://example.com', name: 'Example', description: '' }
	});
	await bookmarksPage.goto();
});

Given('I am on the bookmarks page', async ({ bookmarksPage }) => {
	await bookmarksPage.goto();
});

Given('I have a saved bookmark', async ({ request, bookmarksPage }) => {
	await request.post(`${BACKEND_URL}/api/bookmarks`, {
		data: { url: 'https://github.com', name: 'GitHub', description: 'Code hosting.' }
	});
	await bookmarksPage.goto();
});

Given('I have a saved bookmark that is not starred', async ({ request, bookmarksPage }) => {
	await request.post(`${BACKEND_URL}/api/bookmarks`, {
		data: { url: 'https://github.com', name: 'GitHub', description: '' }
	});
	await bookmarksPage.goto();
});

Given('I have multiple saved bookmarks', async ({ request, bookmarksPage }) => {
	await request.post(`${BACKEND_URL}/api/bookmarks`, {
		data: { url: 'https://github.com', name: 'GitHub', description: 'Code hosting.' }
	});
	await request.post(`${BACKEND_URL}/api/bookmarks`, {
		data: { url: 'https://example.com', name: 'Example', description: 'Example site.' }
	});
	await bookmarksPage.goto();
});

Given('I have no saved bookmarks', async ({ bookmarksPage }) => {
	await bookmarksPage.goto();
});

When('I navigate to the bookmarks page', async ({ bookmarksPage }) => {
	await bookmarksPage.goto();
});

When('I open the add bookmark modal and submit a URL, name, and description', async ({ bookmarksPage }) => {
	await bookmarksPage.openAddModal();
	await bookmarksPage.fillAndSubmitBookmark('https://news.ycombinator.com', 'Hacker News', 'Tech news.');
});

When('I add a bookmark with the URL {string} and no protocol', async ({ bookmarksPage }, url: string) => {
	await bookmarksPage.openAddModal();
	await bookmarksPage.addBookmarkByUrl(url);
});

When('I open the edit modal and change the name and description', async ({ bookmarksPage }) => {
	await bookmarksPage.openEditModal('GitHub');
	await bookmarksPage.fillEditForm('GitHub (updated)', 'Updated description.');
	await bookmarksPage.submitEditForm();
});

When('I click delete and confirm the confirmation dialog', async ({ bookmarksPage }) => {
	await bookmarksPage.clickDelete('GitHub');
	await bookmarksPage.confirmDelete();
});

When('I click delete and cancel the confirmation dialog', async ({ bookmarksPage }) => {
	await bookmarksPage.clickDelete('GitHub');
	await bookmarksPage.cancelDelete();
});

When('I click the star button on the card', async ({ bookmarksPage }) => {
	await bookmarksPage.clickStar('GitHub');
});

When('I type a search term that matches only one bookmark', async ({ bookmarksPage }) => {
	await bookmarksPage.search('GitHub');
});

Then('I should see each bookmark displayed as a card with its title and favicon', async ({ bookmarksPage }) => {
	const visible = await bookmarksPage.isCardVisible('GitHub');
	if (!visible) throw new Error('Expected GitHub card to be visible');
});

Then('the new bookmark card should appear on the page', async ({ bookmarksPage }) => {
	const visible = await bookmarksPage.isCardVisible('Hacker News');
	if (!visible) throw new Error('Expected Hacker News card to be visible');
});

Then('the bookmark should be saved with the URL {string}', async ({ bookmarksPage }, expectedUrl: string) => {
	const actual = await bookmarksPage.getCardUrl('github.com');
	if (actual !== expectedUrl) throw new Error(`Expected URL "${expectedUrl}" but got "${actual}"`);
});

Then('the card should reflect the updated values', async ({ bookmarksPage }) => {
	const visible = await bookmarksPage.isCardVisible('GitHub (updated)');
	if (!visible) throw new Error('Expected updated card to be visible');
});

Then('the bookmark card should no longer appear on the page', async ({ bookmarksPage }) => {
	const visible = await bookmarksPage.isCardVisible('GitHub');
	if (visible) throw new Error('Expected GitHub card to be gone after deletion');
});

Then('the bookmark card should still appear on the page', async ({ bookmarksPage }) => {
	const visible = await bookmarksPage.isCardVisible('GitHub');
	if (!visible) throw new Error('Expected GitHub card to still be visible after cancel');
});

Then('the bookmark should be marked as starred', async ({ bookmarksPage }) => {
	const starred = await bookmarksPage.isCardStarred('GitHub');
	if (!starred) throw new Error('Expected GitHub bookmark to be starred');
});

Then('only the matching bookmark card should be visible', async ({ bookmarksPage }) => {
	const count = await bookmarksPage.visibleCardCount();
	if (count !== 1) throw new Error(`Expected 1 visible card but got ${count}`);
	const visible = await bookmarksPage.isCardVisible('GitHub');
	if (!visible) throw new Error('Expected GitHub to be the visible card');
});

Then('I should see an empty state message', async ({ bookmarksPage }) => {
	const visible = await bookmarksPage.isEmptyStateVisible();
	if (!visible) throw new Error('Expected empty state to be visible');
});
