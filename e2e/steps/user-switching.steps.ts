import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

Before({ tags: '@user-switching' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/users`);
	await request.post(`${BACKEND_URL}/api/test/users`, { data: { name: 'Alex' } });
	await request.post(`${BACKEND_URL}/api/test/users`, { data: { name: 'Sam' } });
});

After({ tags: '@user-switching' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/users`);
});

Given('no user has been selected on this device', async () => {
	// Playwright browser contexts start with clean localStorage
});

Given('I have selected the user {string}', async ({ userSwitchingPage }, name: string) => {
	await userSwitchingPage.gotoHome();
	await userSwitchingPage.selectUserByName(name);
});

When('I visit the home page', async ({ userSwitchingPage }) => {
	await userSwitchingPage.gotoHome();
});

When('I select the user {string}', async ({ userSwitchingPage }, name: string) => {
	await userSwitchingPage.selectUserByName(name);
});

When('I reload the home page', async ({ page }) => {
	await page.reload();
	await page.waitForSelector('[data-testid="user-switcher"], [data-testid="sidebar-logo"]');
});

Then('I should see the user switcher with the seeded users', async ({ userSwitchingPage }) => {
	if (!(await userSwitchingPage.isSwitcherVisible())) {
		throw new Error('User switcher was not visible');
	}
	if (!(await userSwitchingPage.isSwitcherShowingUser('Alex'))) {
		throw new Error('User switcher did not list seeded user Alex');
	}
	if (!(await userSwitchingPage.isSwitcherShowingUser('Sam'))) {
		throw new Error('User switcher did not list seeded user Sam');
	}
});

Then('I should not see the user switcher', async ({ userSwitchingPage }) => {
	if (await userSwitchingPage.isSwitcherVisible()) {
		throw new Error('User switcher was unexpectedly visible');
	}
});

Then('I should see the app sidebar', async ({ userSwitchingPage }) => {
	if (!(await userSwitchingPage.isSidebarVisible())) {
		throw new Error('App sidebar was not visible');
	}
});

Then('I should not see the app sidebar', async ({ userSwitchingPage }) => {
	if (await userSwitchingPage.isSidebarVisible()) {
		throw new Error('App sidebar was unexpectedly visible');
	}
});
