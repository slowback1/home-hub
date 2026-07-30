import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';
const FLAG = 'WHEEL_PICKER_ENABLED';

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

// All steps below are stubs — implemented in task 09 (drive scenarios GREEN).
const notImplemented = () => {
	throw new Error('not implemented');
};

Given('the WHEEL_PICKER_ENABLED feature flag is enabled', notImplemented);
Given('the WHEEL_PICKER_ENABLED feature flag is disabled', notImplemented);
Given('a wheel named {string} exists with items {string}', notImplemented);
Given('a wheel named {string} exists with no items', notImplemented);
Given('the wheels widget is on the dashboard', notImplemented);

When('I visit the Wheels page', notImplemented);
When('I create a wheel named {string} with items {string}', notImplemented);
When('I edit the {string} wheel to be named {string} with items {string}', notImplemented);
When('I delete the {string} wheel', notImplemented);
When('I select the {string} wheel in the spin section', notImplemented);
When('I click Spin', notImplemented);
When('I select the {string} wheel in the wheels widget', notImplemented);
When('I click Spin in the wheels widget', notImplemented);

Then('I see a wheel named {string} in the manage list', notImplemented);
Then('the {string} wheel shows {int} items', notImplemented);
Then('I do not see a wheel named {string} in the manage list', notImplemented);
Then('I see the wheels empty state', notImplemented);
Then('I see a spin result that is one of {string}', notImplemented);
Then('the Spin button is disabled', notImplemented);
Then('I see a widget spin result that is one of {string}', notImplemented);
Then('the Wheels nav item is not visible in the sidebar', notImplemented);
Then('the wheels widget is not available in the widget picker', notImplemented);
