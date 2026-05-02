import { expect } from '@playwright/test';
import { Given, When, Then, Before } from '../fixtures';

const BACKEND_URL = 'http://localhost:5272';

Before({ tags: '@activity' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/activities`);
	await request.delete(`${BACKEND_URL}/api/test/activity-picks`);
});

// --- Config page steps ---

Given('I am on the activity config page', async ({ activityConfigPage }) => {
	await activityConfigPage.goto();
});

When(
	'I add a new activity with name {string} and weight {int}',
	async ({ activityConfigPage }, name: string, weight: number) => {
		await activityConfigPage.addActivity(name, weight);
	}
);

Then(
	'I should see {string} in the activity list with weight {int}',
	async ({ activityConfigPage }, name: string, weight: number) => {
		const visible = await activityConfigPage.isActivityVisible(name);
		expect(visible).toBe(true);
		const actualWeight = await activityConfigPage.getActivityWeight(name);
		expect(actualWeight).toBe(weight);
	}
);

Given(
	'the activity list contains {string} with weight {int}',
	async ({ activityConfigPage, request }, name: string, weight: number) => {
		await request.post(`${BACKEND_URL}/api/activities`, { data: { name, weight } });
		await activityConfigPage.goto();
	}
);

When(
	'I change the weight of {string} to {int}',
	async ({ activityConfigPage }, name: string, weight: number) => {
		await activityConfigPage.changeWeight(name, weight);
	}
);

Given('the activity list contains {string}', async ({ activityConfigPage, request }, name: string) => {
	await request.post(`${BACKEND_URL}/api/activities`, { data: { name, weight: 1 } });
	await activityConfigPage.goto();
});

When('I delete {string}', async ({ activityConfigPage }, name: string) => {
	await activityConfigPage.deleteActivity(name);
});

Then(
	'I should not see {string} in the activity list',
	async ({ activityConfigPage }, name: string) => {
		const visible = await activityConfigPage.isActivityVisible(name);
		expect(visible).toBe(false);
	}
);

// --- Display page steps ---

Given('there are no activity picks recorded', async ({ activityPage }) => {
	await activityPage.clearPickHistory();
});

When('I navigate to the activity display page', async ({ activityPage }) => {
	await activityPage.goto();
});

Then(
	'I should see a placeholder message prompting me to configure activities',
	async ({ activityPage }) => {
		const visible = await activityPage.isPlaceholderVisible();
		expect(visible).toBe(true);
	}
);

Given(
	'an activity pick exists for the current hour with name {string}',
	async ({ activityPage }, name: string) => {
		await activityPage.clearPickHistory();
		await activityPage.seedCurrentPick(name);
	}
);

Then(
	'I should see {string} displayed as the current pick',
	async ({ activityPage }, name: string) => {
		const current = await activityPage.getCurrentPickName();
		expect(current).toBe(name);
	}
);
