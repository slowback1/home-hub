import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

type RequestFixture = Parameters<Parameters<typeof Given>[1]>[0]['request'];

async function lookupUserId(request: RequestFixture, name: string): Promise<string> {
	const response = await request.get(`${BACKEND_URL}/api/users`);
	const users: { id: string; name: string }[] = await response.json();
	const user = users.find((u) => u.name === name);
	if (!user) {
		throw new Error(`Seeded user "${name}" not found`);
	}
	return user.id;
}

Before({ tags: '@user-data-isolation' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/bookmarks`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/wheels`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/activity-picks`).catch(() => {});
});

After({ tags: '@user-data-isolation' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/bookmarks`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/wheels`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/activity-picks`).catch(() => {});
});

When('I switch to the user {string}', async ({ userSwitchingPage }, name: string) => {
	await userSwitchingPage.clearUserSelection();
	await userSwitchingPage.selectUserByName(name);
});

When('I add a bookmark named {string}', async ({ bookmarksPage }, name: string) => {
	await bookmarksPage.goto();
	await bookmarksPage.openAddModal();
	await bookmarksPage.fillAndSubmitBookmark(
		`https://example.com/${name.toLowerCase().replace(/\s+/g, '-')}`,
		name,
		''
	);
});

Then('I should see the bookmark {string}', async ({ bookmarksPage }, name: string) => {
	await bookmarksPage.goto();
	expect(await bookmarksPage.isCardVisible(name)).toBe(true);
});

Then('I should not see the bookmark {string}', async ({ bookmarksPage }, name: string) => {
	await bookmarksPage.goto();
	expect(await bookmarksPage.isCardVisible(name)).toBe(false);
});

When('I add a wheel named {string} with items {string}', async ({ wheelsPage }, name: string, items: string) => {
	await wheelsPage.goto();
	await wheelsPage.createWheel(name, items);
});

Then('I should see the wheel {string}', async ({ wheelsPage }, name: string) => {
	await wheelsPage.goto();
	await wheelsPage.expectWheelRow(name);
});

Then('I should not see the wheel {string}', async ({ wheelsPage }, name: string) => {
	await wheelsPage.goto();
	await wheelsPage.expectNoWheelRow(name);
});

Given(
	'an activity pick named {string} is seeded for the user {string}',
	async ({ request }, pickName: string, userName: string) => {
		const userId = await lookupUserId(request, userName);
		const response = await request.post(`${BACKEND_URL}/api/test/activity-picks`, {
			data: { activityName: pickName, pickedAt: new Date().toISOString(), userId }
		});
		if (!response.ok()) {
			throw new Error(`Failed to seed activity pick for ${userName}: ${response.status()}`);
		}
	}
);
