import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';
const FLAG = 'WALK_SESSION_HISTORY_ENABLED';

Before({ tags: '@walk-history' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/walk-sessions`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/dashboard`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
});

After({ tags: '@walk-history' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/walk-sessions`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/dashboard`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
});

Given('the WALK_SESSION_HISTORY_ENABLED feature flag is enabled', async ({ request }) => {
	await request.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: true } });
});

Given('the WALK_SESSION_HISTORY_ENABLED feature flag is disabled', async ({ request }) => {
	await request.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } });
});

Given('the walk session history widget is on the dashboard', async ({ request }) => {
	await request.put(`${BACKEND_URL}/api/dashboard/layout`, {
		data: { layoutFormat: '3x2', slots: [{ slotIndex: 0, widgetType: 'walk-history' }] }
	});
});

Given('there are walk sessions synced from the Android app', async ({ request }) => {
	const now = new Date();
	const todayStart = new Date(now.getFullYear(), now.getMonth(), now.getDate());
	const sessions = [
		{
			clientId: crypto.randomUUID(),
			startedAt: new Date(todayStart.getTime() + 7 * 3600 * 1000).toISOString(),
			durationSeconds: 2280,
			stepCount: 4820
		},
		{
			clientId: crypto.randomUUID(),
			startedAt: new Date(todayStart.getTime() - 6 * 3600 * 1000).toISOString(),
			durationSeconds: 3720,
			stepCount: 7140
		},
		{
			clientId: crypto.randomUUID(),
			startedAt: new Date(todayStart.getTime() - 48 * 3600 * 1000).toISOString(),
			durationSeconds: 5040,
			stepCount: 9360
		}
	];
	for (const session of sessions) {
		await request.post(`${BACKEND_URL}/api/walk-sessions`, { data: session });
	}
});

When('I visit the Walk History page', async ({ walkHistoryPage }) => {
	await walkHistoryPage.goto();
});

Then('I see up to 3 sessions in the walk history widget', async ({ dashboardPage }) => {
	await dashboardPage.page.waitForSelector('[data-testid="walk-widget-row"]', { timeout: 5000 });
	const count = await dashboardPage.page.locator('[data-testid="walk-widget-row"]').count();
	expect(count).toBeGreaterThan(0);
	expect(count).toBeLessThanOrEqual(3);
});

Then('each widget row shows the session date, step count, and duration', async ({ dashboardPage }) => {
	const row = dashboardPage.page.locator('[data-testid="walk-widget-row"]').first();
	await expect(row.locator('[data-testid="walk-widget-steps"]')).toBeVisible();
	await expect(row.locator('[data-testid="walk-widget-duration"]')).toBeVisible();
});

Then('the walk history widget shows an empty state message', async ({ dashboardPage }) => {
	await dashboardPage.page.waitForSelector('[data-testid="walk-widget-empty"]', { timeout: 5000 });
	await expect(dashboardPage.page.locator('[data-testid="walk-widget-empty"]')).toBeVisible();
});

Then('I see all synced walk sessions in the list', async ({ walkHistoryPage }) => {
	await walkHistoryPage.page.waitForSelector('[data-testid="walk-session-row"]', { timeout: 5000 });
	const count = await walkHistoryPage.getSessionRowCount();
	expect(count).toBeGreaterThan(0);
});

Then('the sessions are sorted with the most recent session first', async ({ walkHistoryPage }) => {
	const firstRowDate = await walkHistoryPage.getFirstRowRelativeTime();
	expect(firstRowDate).toBe('Today');
});

Then('each row shows the session date, step count, and duration', async ({ walkHistoryPage }) => {
	await expect(walkHistoryPage.page.locator('[data-testid="walk-session-steps"]').first()).toBeVisible();
	await expect(
		walkHistoryPage.page.locator('[data-testid="walk-session-duration"]').first()
	).toBeVisible();
});

Then('I see the walk history empty state', async ({ walkHistoryPage }) => {
	await walkHistoryPage.page.waitForSelector('[data-testid="walk-empty-state"]', { timeout: 5000 });
	expect(await walkHistoryPage.isEmptyStateVisible()).toBe(true);
});

Then('the Walk History nav item is not visible in the sidebar', async ({ dashboardPage }) => {
	const navItem = dashboardPage.page.locator('[data-testid="nav-item-walk-history"]');
	await expect(navItem).toBeHidden();
});

Then('the walk history widget is not available in the widget picker', async ({ dashboardPage }) => {
	await dashboardPage.clickAddWidget(0);
	await dashboardPage.page.waitForSelector('[data-testid="widget-picker-modal"]', {
		state: 'visible'
	});
	const walkCard = dashboardPage.page.locator('[data-testid="widget-picker-card"][data-widget-id="walk-history"]');
	await expect(walkCard).toBeHidden();
});
