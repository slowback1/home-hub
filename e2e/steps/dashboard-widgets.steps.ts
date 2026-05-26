import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

// Reset dashboard layout and any seeded data before/after each scenario
Before({ tags: '@dashboard-widgets' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/dashboard`);
	await request.delete(`${BACKEND_URL}/api/test/tasks`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/bookmarks`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/audiobook-jobs`).catch(() => {});
});

After({ tags: '@dashboard-widgets' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/dashboard`);
	await request.delete(`${BACKEND_URL}/api/test/tasks`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/bookmarks`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/audiobook-jobs`).catch(() => {});
	// Restore weather feature flag if it was disabled
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/WEATHER_ENABLED`, { data: { isEnabled: true } })
		.catch(() => {});
});

// --- Slot setup helpers ---

Given('the weather widget is placed in slot 0', async ({ request }) => {
	throw new Error('not implemented');
});

Given('the tasks widget is placed in slot 0', async ({ request }) => {
	throw new Error('not implemented');
});

Given('the activity widget is placed in slot 0', async ({ request }) => {
	throw new Error('not implemented');
});

Given('the audiobook widget is placed in slot 0', async ({ request }) => {
	throw new Error('not implemented');
});

Given('the bookmarks widget is placed in slot 0', async ({ request }) => {
	throw new Error('not implemented');
});

// --- Data seeding ---

Given('there are due tasks in the system', async ({ request }) => {
	throw new Error('not implemented');
});

Given('there is a due task named {string}', async ({ request }, name: string) => {
	throw new Error('not implemented');
});

Given('there are 7 due tasks in the system', async ({ request }) => {
	throw new Error('not implemented');
});

Given('there is a current activity pick', async ({ request }) => {
	throw new Error('not implemented');
});

Given('there is an in-progress audiobook job for {string}', async ({ request }, filename: string) => {
	throw new Error('not implemented');
});

Given('there are no audiobook jobs', async ({ request }) => {
	// nothing to seed — default state is empty
});

Given('there is a starred bookmark named {string}', async ({ request }, name: string) => {
	throw new Error('not implemented');
});

Given('there are unstarred bookmarks in the system', async ({ request }) => {
	throw new Error('not implemented');
});

Given('the weather API is unavailable', async ({ request }) => {
	throw new Error('not implemented');
});

// --- Weather widget assertions ---

Then('I should see the temperature in the weather widget', async ({ dashboardPage }) => {
	throw new Error('not implemented');
});

Then('I should see the condition label in the weather widget', async ({ dashboardPage }) => {
	throw new Error('not implemented');
});

Then('I should see the humidity in the weather widget', async ({ dashboardPage }) => {
	throw new Error('not implemented');
});

Then('I should see the wind speed in the weather widget', async ({ dashboardPage }) => {
	throw new Error('not implemented');
});

Then('I should see the error state in the weather widget', async ({ dashboardPage }) => {
	throw new Error('not implemented');
});

// --- Tasks widget ---

Then('I should see the due task names in the tasks widget', async ({ dashboardPage }) => {
	throw new Error('not implemented');
});

When('I mark {string} as done in the tasks widget', async ({ dashboardPage }, name: string) => {
	throw new Error('not implemented');
});

Then('I should see an undo toast', async ({ dashboardPage }) => {
	throw new Error('not implemented');
});

Then('I should see 5 task rows in the tasks widget', async ({ dashboardPage }) => {
	throw new Error('not implemented');
});

Then('I should see {string} in the tasks widget', async ({ dashboardPage }, text: string) => {
	throw new Error('not implemented');
});

// --- Activity widget ---

Then('I should see the activity name in the activity widget', async ({ dashboardPage }) => {
	throw new Error('not implemented');
});

// --- Audiobook widget ---

Then('I should see {string} in the audiobook widget', async ({ dashboardPage }, text: string) => {
	throw new Error('not implemented');
});

Then(
	'I should see the {string} status badge in the audiobook widget',
	async ({ dashboardPage }, status: string) => {
		throw new Error('not implemented');
	}
);

// --- Bookmarks widget ---

Then(
	'I should see {string} as a link in the bookmarks widget',
	async ({ dashboardPage }, name: string) => {
		throw new Error('not implemented');
	}
);

Then('I should see bookmark links in the bookmarks widget', async ({ dashboardPage }) => {
	throw new Error('not implemented');
});
