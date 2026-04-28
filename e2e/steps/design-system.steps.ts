import { Given, When, Then } from '../fixtures';

Given('I am on the home page', async ({ designSystemPage }) => {
	await designSystemPage.goto();
});

Given('I navigate to the home page', async ({ designSystemPage }) => {
	await designSystemPage.goto();
});

Given('the Sidebar is expanded', async ({ designSystemPage }) => {
	throw new Error('not implemented');
});

Given('I have collapsed the Sidebar', async ({ designSystemPage }) => {
	throw new Error('not implemented');
});

When('I click the {string} nav item in the Sidebar', async ({ designSystemPage }, label: string) => {
	await designSystemPage.clickSidebarNavItem(label);
});

When('I click the collapse toggle', async ({ designSystemPage }) => {
	await designSystemPage.clickCollapseToggle();
});

When('I reload the page', async ({ designSystemPage }) => {
	await designSystemPage.reload();
});

Then('I should be on the task tracker page', async ({ designSystemPage }) => {
	throw new Error('not implemented');
});

Then('the {string} nav item should be marked as active', async ({ designSystemPage }, label: string) => {
	throw new Error('not implemented');
});

Then('the Sidebar should collapse to icon-only mode', async ({ designSystemPage }) => {
	throw new Error('not implemented');
});

Then('nav item labels should not be visible', async ({ designSystemPage }) => {
	throw new Error('not implemented');
});

Then('the Sidebar should still be in icon-only mode', async ({ designSystemPage }) => {
	throw new Error('not implemented');
});

Then('the app should have the dark theme applied', async ({ designSystemPage }) => {
	throw new Error('not implemented');
});

Then('no light theme class should be present on the document', async ({ designSystemPage }) => {
	throw new Error('not implemented');
});
