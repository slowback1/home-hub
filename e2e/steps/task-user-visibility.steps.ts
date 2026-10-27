import { expect } from '@playwright/test';
import { Given, Then, Before, After } from '../fixtures';

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

Before({ tags: '@task-visibility' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/tasks`).catch(() => {});
});

After({ tags: '@task-visibility' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/tasks`).catch(() => {});
});

Given(
	'a personal task {string} is seeded for the user {string}',
	async ({ request }, name: string, userName: string) => {
		const userId = await lookupUserId(request, userName);
		const response = await request.post(`${BACKEND_URL}/api/tasks`, {
			headers: { 'X-User-Id': userId },
			data: {
				name,
				doDate: null,
				isRecurring: false,
				intervalDays: null,
				isGlobal: false
			}
		});
		if (!response.ok()) {
			throw new Error(`Failed to seed personal task "${name}" for ${userName}: ${response.status()}`);
		}
	}
);

Given('a shared task {string} is seeded', async ({ request }, name: string) => {
	const response = await request.post(`${BACKEND_URL}/api/tasks`, {
		data: { name, doDate: null, isRecurring: false, intervalDays: null }
	});
	if (!response.ok()) {
		throw new Error(`Failed to seed shared task "${name}": ${response.status()}`);
	}
});

Then('the task {string} is completed in the history for the user {string}', async ({ request }, name: string, userName: string) => {
	const userId = await lookupUserId(request, userName);
	const response = await request.get(`${BACKEND_URL}/api/tasks/history`, {
		headers: { 'X-User-Id': userId }
	});
	if (!response.ok()) {
		throw new Error(`Failed to fetch task history for ${userName}: ${response.status()}`);
	}
	const tasks: { name: string }[] = await response.json();
	expect(tasks.some((t) => t.name === name)).toBe(true);
});

Then('I should see the task {string}', async ({ tasksPage }, name: string) => {
	await tasksPage.goto();
	expect(await tasksPage.isTaskVisibleAnywhere(name)).toBe(true);
});

Then('I should not see the task {string}', async ({ tasksPage }, name: string) => {
	await tasksPage.goto();
	expect(await tasksPage.isTaskVisibleAnywhere(name)).toBe(false);
});
