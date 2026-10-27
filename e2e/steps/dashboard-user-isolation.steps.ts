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

Before({ tags: '@dashboard-user-isolation' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/dashboard`).catch(() => {});
});

After({ tags: '@dashboard-user-isolation' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/dashboard`).catch(() => {});
});

Given(
	'the dashboard layout for the user {string} is seeded with widgets {string}',
	async ({ request }, userName: string, widgets: string) => {
		const userId = await lookupUserId(request, userName);
		const slots = widgets
			.split(',')
			.map((widgetType, slotIndex) => ({ slotIndex, widgetType: widgetType.trim() }));
		const response = await request.put(`${BACKEND_URL}/api/dashboard/layout`, {
			data: { layoutFormat: '3x2', slots },
			headers: { 'X-User-Id': userId }
		});
		if (!response.ok()) {
			throw new Error(`Failed to seed dashboard layout for ${userName}: ${response.status()}`);
		}
	}
);

When('I visit the dashboard', async ({ dashboardPage }) => {
	await dashboardPage.goto();
});

Then('slot {int} should show the widget {string}', async ({ dashboardPage }, slotIndex: number, widgetId: string) => {
	const actual = await dashboardPage.filledSlotWidgetId(slotIndex);
	expect(actual).toBe(widgetId);
});
