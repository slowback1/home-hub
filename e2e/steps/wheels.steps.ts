import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';
const FLAG = 'WHEEL_PICKER_ENABLED';

/** Convert a comma-separated item list into the newline-delimited form the API stores. */
function toItemLines(csv: string): string {
	return csv
		.split(',')
		.map((item) => item.trim())
		.filter((item) => item.length > 0)
		.join('\n');
}

function toItemList(csv: string): string[] {
	return csv
		.split(',')
		.map((item) => item.trim())
		.filter((item) => item.length > 0);
}

Before({ tags: '@wheels' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/wheels`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/dashboard`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
});

After({ tags: '@wheels' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/wheels`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/dashboard`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
});

Given('the WHEEL_PICKER_ENABLED feature flag is enabled', async ({ request }) => {
	await request.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: true } });
});

Given('the WHEEL_PICKER_ENABLED feature flag is disabled', async ({ request }) => {
	await request.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } });
});

Given('a wheel named {string} exists with items {string}', async ({ request }, name, items) => {
	await request.post(`${BACKEND_URL}/api/wheels`, {
		data: { name, items: toItemLines(items) }
	});
});

Given('a wheel named {string} exists with no items', async ({ request }, name) => {
	await request.post(`${BACKEND_URL}/api/wheels`, { data: { name, items: '' } });
});

Given('the wheels widget is on the dashboard', async ({ request }) => {
	await request.put(`${BACKEND_URL}/api/dashboard/layout`, {
		data: { layoutFormat: '3x2', slots: [{ slotIndex: 0, widgetType: 'wheels' }] }
	});
});

When('I visit the Wheels page', async ({ wheelsPage }) => {
	await wheelsPage.goto();
});

When('I create a wheel named {string} with items {string}', async ({ wheelsPage }, name, items) => {
	await wheelsPage.createWheel(name, toItemLines(items));
});

When(
	'I edit the {string} wheel to be named {string} with items {string}',
	async ({ wheelsPage }, oldName, newName, items) => {
		await wheelsPage.editWheel(oldName, newName, toItemLines(items));
	}
);

When('I delete the {string} wheel', async ({ wheelsPage }, name) => {
	await wheelsPage.deleteWheel(name);
});

When('I select the {string} wheel in the spin section', async ({ wheelsPage }, name) => {
	await wheelsPage.selectWheelInSpin(name);
});

When('I click Spin', async ({ wheelsPage }) => {
	await wheelsPage.clickSpin();
});

When('I select the {string} wheel in the wheels widget', async ({ dashboardPage }, name) => {
	await dashboardPage.page
		.locator('[data-testid="wheel-widget-select"]')
		.selectOption({ label: name });
});

When('I click Spin in the wheels widget', async ({ dashboardPage }) => {
	await dashboardPage.page.locator('[data-testid="wheel-widget-spin-btn"]').click();
});

Then('I see a wheel named {string} in the manage list', async ({ wheelsPage }, name) => {
	await wheelsPage.expectWheelRow(name);
});

Then('the {string} wheel shows {int} items', async ({ wheelsPage }, name, count) => {
	const text = await wheelsPage.getItemCountText(name);
	expect(text).toContain(String(count));
});

Then('I do not see a wheel named {string} in the manage list', async ({ wheelsPage }, name) => {
	await wheelsPage.expectNoWheelRow(name);
});

Then('I see the wheels empty state', async ({ wheelsPage }) => {
	await wheelsPage.page.waitForSelector('[data-testid="wheels-empty-state"]', { timeout: 5000 });
	expect(await wheelsPage.isEmptyStateVisible()).toBe(true);
});

Then('I see a spin result that is one of {string}', async ({ wheelsPage }, items) => {
	const result = await wheelsPage.getSpinResult();
	expect(toItemList(items)).toContain(result);
});

Then('the Spin button is disabled', async ({ wheelsPage }) => {
	expect(await wheelsPage.isSpinDisabled()).toBe(true);
});

Then('I see a widget spin result that is one of {string}', async ({ dashboardPage }, items) => {
	const result = dashboardPage.page.locator('[data-testid="wheel-widget-result"]');
	await result.waitFor({ state: 'visible', timeout: 5000 });
	expect(toItemList(items)).toContain((await result.textContent())?.trim() ?? '');
});

Then('the Wheels nav item is not visible in the sidebar', async ({ dashboardPage }) => {
	await expect(dashboardPage.page.locator('[data-testid="nav-item-wheels"]')).toBeHidden();
});

Then('the wheels widget is not available in the widget picker', async ({ dashboardPage }) => {
	await dashboardPage.clickAddWidget(0);
	await dashboardPage.page.waitForSelector('[data-testid="widget-picker-modal"]', {
		state: 'visible'
	});
	const card = dashboardPage.page.locator(
		'[data-testid="widget-picker-card"][data-widget-id="wheels"]'
	);
	await expect(card).toBeHidden();
});
