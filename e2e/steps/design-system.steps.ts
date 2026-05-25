import { expect } from '@playwright/test';
import { Given, When, Then } from '../fixtures';

Given('I am on the home page', async ({ designSystemPage }) => {
	await designSystemPage.goto();
});

Given('I navigate to the home page', async ({ designSystemPage }) => {
	await designSystemPage.goto();
});

Given('the Sidebar is expanded', async ({ designSystemPage }) => {
	const expanded = await designSystemPage.isSidebarExpanded();
	expect(expanded).toBe(true);
});

Given('I have collapsed the Sidebar', async ({ designSystemPage }) => {
	await designSystemPage.collapseSidebar();
	const collapsed = await designSystemPage.isSidebarCollapsed();
	expect(collapsed).toBe(true);
});

When(
	'I click the {string} nav item in the Sidebar',
	async ({ designSystemPage }, label: string) => {
		await designSystemPage.clickSidebarNavItem(label);
	}
);

When('I click the collapse toggle', async ({ designSystemPage }) => {
	await designSystemPage.clickCollapseToggle();
});

When('I reload the page', async ({ designSystemPage }) => {
	await designSystemPage.reload();
});

Then('I should be on the task tracker page', async ({ designSystemPage }) => {
	await designSystemPage.assertOnPath('/tasks');
});

Then(
	'the {string} nav item should be marked as active',
	async ({ designSystemPage }, label: string) => {
		const active = await designSystemPage.isNavItemActive(label);
		expect(active).toBe(true);
	}
);

Then('the Sidebar should collapse to icon-only mode', async ({ designSystemPage }) => {
	const collapsed = await designSystemPage.isSidebarCollapsed();
	expect(collapsed).toBe(true);
});

Then('nav item labels should not be visible', async ({ designSystemPage }) => {
	const visible = await designSystemPage.areNavLabelsVisible();
	expect(visible).toBe(false);
});

Then('the Sidebar should still be in icon-only mode', async ({ designSystemPage }) => {
	const collapsed = await designSystemPage.isSidebarCollapsed();
	expect(collapsed).toBe(true);
});

Then('the app should have the dark theme applied', async ({ designSystemPage }) => {
	const hasDark = await designSystemPage.hasDarkTheme();
	expect(hasDark).toBe(true);
});

Then('no light theme class should be present on the document', async ({ designSystemPage }) => {
	const hasLight = await designSystemPage.hasLightTheme();
	expect(hasLight).toBe(false);
});
