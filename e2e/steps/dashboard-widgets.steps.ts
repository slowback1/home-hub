import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

// Reset dashboard layout and any seeded data before/after each scenario
Before({ tags: '@dashboard-widgets' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/dashboard`);
	await request.delete(`${BACKEND_URL}/api/test/tasks`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/bookmarks`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/audiobook-jobs`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/activity-picks`).catch(() => {});
});

After({ tags: '@dashboard-widgets' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/dashboard`);
	await request.delete(`${BACKEND_URL}/api/test/tasks`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/bookmarks`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/audiobook-jobs`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/activity-picks`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/WEATHER_ENABLED`, { data: { isEnabled: true } })
		.catch(() => {});
});

// --- Slot setup helpers ---

async function placeWidget(request: Parameters<Parameters<typeof Given>[1]>[0]['request'], widgetType: string) {
	await request.put(`${BACKEND_URL}/api/dashboard/layout`, {
		data: {
			layoutFormat: '3x2',
			slots: [{ slotIndex: 0, widgetType }]
		}
	});
}

Given('the weather widget is placed in slot 0', async ({ request }) => {
	await placeWidget(request, 'weather');
});

Given('the tasks widget is placed in slot 0', async ({ request }) => {
	await placeWidget(request, 'tasks');
});

Given('the activity widget is placed in slot 0', async ({ request }) => {
	await placeWidget(request, 'activity');
});

Given('the audiobook widget is placed in slot 0', async ({ request }) => {
	await placeWidget(request, 'audiobook');
});

Given('the bookmarks widget is placed in slot 0', async ({ request }) => {
	await placeWidget(request, 'bookmarks');
});

// --- Data seeding ---

Given('there are due tasks in the system', async ({ request }) => {
	const today = new Date().toISOString().split('T')[0];
	await request.post(`${BACKEND_URL}/api/tasks`, {
		data: { name: 'Due task one', doDate: today, isRecurring: false, intervalDays: null }
	});
	await request.post(`${BACKEND_URL}/api/tasks`, {
		data: { name: 'Due task two', doDate: today, isRecurring: false, intervalDays: null }
	});
});

Given('there is a due task named {string}', async ({ request }, name: string) => {
	const today = new Date().toISOString().split('T')[0];
	await request.post(`${BACKEND_URL}/api/tasks`, {
		data: { name, doDate: today, isRecurring: false, intervalDays: null }
	});
});

Given('there are 7 due tasks in the system', async ({ request }) => {
	const today = new Date().toISOString().split('T')[0];
	for (let i = 1; i <= 7; i++) {
		await request.post(`${BACKEND_URL}/api/tasks`, {
			data: { name: `Task ${i}`, doDate: today, isRecurring: false, intervalDays: null }
		});
	}
});

Given('there is a current activity pick', async ({ request }) => {
	await request.post(`${BACKEND_URL}/api/test/activity-picks`, {
		data: { activityName: 'Go for a walk', pickedAt: new Date().toISOString() }
	});
});

Given('there is an in-progress audiobook job for {string}', async ({ request }, filename: string) => {
	await request.post(`${BACKEND_URL}/api/test/audiobook-jobs`, {
		data: { epubFilename: filename, status: 1 } // 1 = InProgress enum value
	});
});

Given('there are no audiobook jobs', async () => {
	// cleared in Before hook
});

Given('there is a starred bookmark named {string}', async ({ request }, name: string) => {
	const res = await request.post(`${BACKEND_URL}/api/bookmarks`, {
		data: { url: `https://example.com/${name.toLowerCase()}`, name, description: null }
	});
	const bookmark = await res.json();
	await request.patch(`${BACKEND_URL}/api/bookmarks/${bookmark.id}/star`, {});
});

Given('there are unstarred bookmarks in the system', async ({ request }) => {
	await request.post(`${BACKEND_URL}/api/bookmarks`, {
		data: { url: 'https://example.com', name: 'Example', description: null }
	});
	await request.post(`${BACKEND_URL}/api/bookmarks`, {
		data: { url: 'https://news.ycombinator.com', name: 'Hacker News', description: null }
	});
});

Given('the weather API is unavailable', async ({ dashboardPage }) => {
	await dashboardPage.setupWeatherApiError();
});

// --- Weather widget assertions ---

Then('I should see the temperature in the weather widget', async ({ dashboardPage }) => {
	await dashboardPage.page.waitForSelector('[data-testid="weather-widget-temperature"]', { timeout: 5000 });
	expect(await dashboardPage.isWeatherTemperatureVisible()).toBe(true);
});

Then('I should see the condition label in the weather widget', async ({ dashboardPage }) => {
	expect(await dashboardPage.isWeatherConditionVisible()).toBe(true);
});

Then('I should see the humidity in the weather widget', async ({ dashboardPage }) => {
	expect(await dashboardPage.isWeatherHumidityVisible()).toBe(true);
});

Then('I should see the wind speed in the weather widget', async ({ dashboardPage }) => {
	expect(await dashboardPage.isWeatherWindSpeedVisible()).toBe(true);
});

Then('I should see the error state in the weather widget', async ({ dashboardPage }) => {
	await dashboardPage.page.waitForSelector('[data-testid="weather-widget-error"]', { timeout: 5000 });
	expect(await dashboardPage.isWeatherErrorStateVisible()).toBe(true);
});

// --- Tasks widget ---

Then('I should see the due task names in the tasks widget', async ({ dashboardPage }) => {
	await dashboardPage.page.waitForSelector('[data-testid="tasks-widget-row"]', { timeout: 5000 });
	const count = await dashboardPage.getTaskWidgetRowCount();
	expect(count).toBeGreaterThan(0);
});

When('I mark {string} as done in the tasks widget', async ({ dashboardPage }, name: string) => {
	await dashboardPage.page.waitForSelector('[data-testid="tasks-widget-row"]', { timeout: 5000 });
	await dashboardPage.clickTaskWidgetDone(name);
});

Then('I should see an undo toast', async ({ dashboardPage }) => {
	await dashboardPage.page.waitForSelector('[data-testid="toast-action"]', { timeout: 5000 });
	expect(await dashboardPage.isUndoToastVisible()).toBe(true);
});

Then('I should see 5 task rows in the tasks widget', async ({ dashboardPage }) => {
	await dashboardPage.page.waitForSelector('[data-testid="tasks-widget-row"]', { timeout: 5000 });
	const count = await dashboardPage.getTaskWidgetRowCount();
	expect(count).toBe(5);
});

Then('I should see {string} in the tasks widget', async ({ dashboardPage }, text: string) => {
	await dashboardPage.page.waitForSelector('[data-testid="tasks-widget-overflow"]', { timeout: 5000 });
	expect(await dashboardPage.isTaskWidgetOverflowVisible(text)).toBe(true);
});

// --- Activity widget ---

Then('I should see the activity name in the activity widget', async ({ dashboardPage }) => {
	await dashboardPage.page.waitForSelector('[data-testid="activity-widget-name"]', { timeout: 5000 });
	expect(await dashboardPage.isActivityNameVisible()).toBe(true);
});

// --- Audiobook widget ---

Then('I should see {string} in the audiobook widget', async ({ dashboardPage }, text: string) => {
	const selector = text === 'No conversions yet'
		? '[data-testid="audiobook-widget-empty"]'
		: `[data-testid="audiobook-widget-filename"]:has-text("${text}")`;
	await dashboardPage.page.waitForSelector(selector, { timeout: 5000 });
	if (text === 'No conversions yet') {
		expect(await dashboardPage.isAudiobookWidgetEmptyVisible()).toBe(true);
	} else {
		expect(await dashboardPage.isAudiobookWidgetTextVisible(text)).toBe(true);
	}
});

Then(
	'I should see the {string} status badge in the audiobook widget',
	async ({ dashboardPage }, status: string) => {
		await dashboardPage.page.waitForSelector('[data-testid="audiobook-widget-status"]', { timeout: 5000 });
		expect(await dashboardPage.isAudiobookStatusBadgeVisible(status)).toBe(true);
	}
);

// --- Bookmarks widget ---

Then(
	'I should see {string} as a link in the bookmarks widget',
	async ({ dashboardPage }, name: string) => {
		await dashboardPage.page.waitForSelector('[data-testid="bookmarks-widget-link"]', { timeout: 5000 });
		expect(await dashboardPage.isBookmarkLinkVisible(name)).toBe(true);
	}
);

Then('I should see bookmark links in the bookmarks widget', async ({ dashboardPage }) => {
	await dashboardPage.page.waitForSelector('[data-testid="bookmarks-widget-link"]', { timeout: 5000 });
	const count = await dashboardPage.bookmarkLinkCount();
	expect(count).toBeGreaterThan(0);
});
