import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

Before({ tags: '@dashboard' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/dashboard`);
});

After({ tags: '@dashboard' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/dashboard`);
});

Given('I am on the dashboard', async () => {
	throw new Error('not implemented');
});

Given('slot 0 has a widget assigned', async () => {
	throw new Error('not implemented');
});

Given('I am in edit mode with slot 0 occupied', async () => {
	throw new Error('not implemented');
});

Given('I am in edit mode', async () => {
	throw new Error('not implemented');
});

When('I click the add widget button on slot 0', async () => {
	throw new Error('not implemented');
});

When('I select a widget from the picker', async () => {
	throw new Error('not implemented');
});

When('I reload the dashboard', async () => {
	throw new Error('not implemented');
});

When('I click the Edit Dashboard button', async () => {
	throw new Error('not implemented');
});

When('I click the remove button on slot 0', async () => {
	throw new Error('not implemented');
});

When('I click the Done button', async () => {
	throw new Error('not implemented');
});

Then('I should see 6 empty slot placeholders', async () => {
	throw new Error('not implemented');
});

Then('each placeholder should have an add widget button', async () => {
	throw new Error('not implemented');
});

Then('the widget picker modal should open', async () => {
	throw new Error('not implemented');
});

Then('the modal should close', async () => {
	throw new Error('not implemented');
});

Then('the widget should appear in slot 0', async () => {
	throw new Error('not implemented');
});

Then('slot 0 should still show the same widget', async () => {
	throw new Error('not implemented');
});

Then('each occupied slot should show a remove button', async () => {
	throw new Error('not implemented');
});

Then('I should see a Done button', async () => {
	throw new Error('not implemented');
});

Then('slot 0 should revert to an empty placeholder', async () => {
	throw new Error('not implemented');
});

Then('the change should be saved automatically', async () => {
	throw new Error('not implemented');
});

Then('the remove buttons should no longer be visible', async () => {
	throw new Error('not implemented');
});
