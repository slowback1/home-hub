import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

Before({ tags: '@dashboard' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/dashboard`);
});

After({ tags: '@dashboard' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/dashboard`);
});

Given('I am on the dashboard', async ({ dashboardPage }) => {
	await dashboardPage.goto();
});

Given('slot 0 has a widget assigned', async ({ request, dashboardPage }) => {
	await request.put(`${BACKEND_URL}/api/dashboard/layout`, {
		data: {
			layoutFormat: '3x2',
			slots: [{ slotIndex: 0, widgetType: 'tasks' }]
		}
	});
	await dashboardPage.goto();
});

Given('I am in edit mode with slot 0 occupied', async ({ request, dashboardPage }) => {
	await request.put(`${BACKEND_URL}/api/dashboard/layout`, {
		data: {
			layoutFormat: '3x2',
			slots: [
				{ slotIndex: 0, widgetType: 'tasks' },
				{ slotIndex: 1, widgetType: 'weather' }
			]
		}
	});
	await dashboardPage.goto();
	await dashboardPage.clickEditDashboard();
});

Given('I am in edit mode', async ({ request, dashboardPage }) => {
	await request.put(`${BACKEND_URL}/api/dashboard/layout`, {
		data: {
			layoutFormat: '3x2',
			slots: [{ slotIndex: 0, widgetType: 'tasks' }]
		}
	});
	await dashboardPage.goto();
	await dashboardPage.clickEditDashboard();
});

When('I click the add widget button on slot 0', async ({ dashboardPage }) => {
	await dashboardPage.clickAddWidget(0);
});

When('I select a widget from the picker', async ({ dashboardPage }) => {
	await dashboardPage.selectFirstAvailableWidget();
});

When('I reload the dashboard', async ({ dashboardPage }) => {
	await dashboardPage.reload();
});

When('I click the Edit Dashboard button', async ({ dashboardPage }) => {
	await dashboardPage.clickEditDashboard();
});

When('I click the remove button on slot 0', async ({ dashboardPage }) => {
	await dashboardPage.clickRemoveWidget(0);
});

When('I click the Done button', async ({ dashboardPage }) => {
	await dashboardPage.clickDone();
});

Then('I should see the first-run hero panel', async ({ dashboardPage }) => {
	expect(await dashboardPage.isHeroVisible()).toBe(true);
});

Then('the hero should have an add widget button', async ({ dashboardPage }) => {
	expect(await dashboardPage.addWidgetButtonCount()).toBeGreaterThan(0);
});

Then('the widget picker modal should open', async ({ dashboardPage }) => {
	expect(await dashboardPage.isPickerModalOpen()).toBe(true);
});

Then('the modal should close', async ({ dashboardPage }) => {
	expect(await dashboardPage.isPickerModalClosed()).toBe(true);
});

Then('the widget should appear in slot 0', async ({ dashboardPage }) => {
	expect(await dashboardPage.isSlotFilled(0)).toBe(true);
});

Then('slot 0 should still show the same widget', async ({ dashboardPage }) => {
	expect(await dashboardPage.isSlotFilled(0)).toBe(true);
});

Then('each occupied slot should show a remove button', async ({ dashboardPage }) => {
	const occupied = await dashboardPage.occupiedSlotCount();
	const removeBtns = await dashboardPage.removeButtonCount();
	expect(removeBtns).toBe(occupied);
});

Then('I should see a Done button', async ({ dashboardPage }) => {
	expect(await dashboardPage.isDoneButtonVisible()).toBe(true);
});

Then('slot 0 should revert to an empty placeholder', async ({ dashboardPage }) => {
	expect(await dashboardPage.isSlotEmpty(0)).toBe(true);
});

Then('the change should be saved automatically', async ({ dashboardPage }) => {
	await dashboardPage.reload();
	expect(await dashboardPage.isSlotEmpty(0)).toBe(true);
});

Then('the remove buttons should no longer be visible', async ({ dashboardPage }) => {
	expect(await dashboardPage.removeButtonCount()).toBe(0);
});
