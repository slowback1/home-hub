import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

Before({ tags: '@walk-history' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/walk-sessions`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/dashboard`).catch(() => {});
});

After({ tags: '@walk-history' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/walk-sessions`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/dashboard`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/WALK_SESSION_HISTORY_ENABLED`, {
			data: { isEnabled: false }
		})
		.catch(() => {});
});

Given('the WALK_SESSION_HISTORY_ENABLED feature flag is enabled', async () => {
	throw new Error('not implemented');
});

Given('the WALK_SESSION_HISTORY_ENABLED feature flag is disabled', async () => {
	throw new Error('not implemented');
});

Given('the walk session history widget is on the dashboard', async () => {
	throw new Error('not implemented');
});

Given('there are walk sessions synced from the Android app', async () => {
	throw new Error('not implemented');
});

When('I visit the Walk History page', async () => {
	throw new Error('not implemented');
});

Then('I see up to 3 sessions in the walk history widget', async () => {
	throw new Error('not implemented');
});

Then('each widget row shows the session date, step count, and duration', async () => {
	throw new Error('not implemented');
});

Then('the walk history widget shows an empty state message', async () => {
	throw new Error('not implemented');
});

Then('I see all synced walk sessions in the list', async () => {
	throw new Error('not implemented');
});

Then('the sessions are sorted with the most recent session first', async () => {
	throw new Error('not implemented');
});

Then('each row shows the session date, step count, and duration', async () => {
	throw new Error('not implemented');
});

Then('I see the walk history empty state', async () => {
	throw new Error('not implemented');
});

Then('the Walk History nav item is not visible in the sidebar', async () => {
	throw new Error('not implemented');
});

Then('the walk history widget is not available in the widget picker', async () => {
	throw new Error('not implemented');
});
