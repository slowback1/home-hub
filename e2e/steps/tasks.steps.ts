import { Given, When, Then, Before } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

Before({ tags: '@tasks' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/tasks`);
});

Given('a task {string} with DoDate {int} days ago', async ({ tasksPage, request }, name: string, days: number) => {
	const doDate = new Date();
	doDate.setDate(doDate.getDate() - days);
	await request.post(`${BACKEND_URL}/api/tasks`, {
		data: { name, doDate: doDate.toISOString().split('T')[0], isRecurring: false, intervalDays: null }
	});
});

Given('a task {string} with DoDate {int} days from now', async ({ tasksPage, request }, name: string, days: number) => {
	const doDate = new Date();
	doDate.setDate(doDate.getDate() + days);
	await request.post(`${BACKEND_URL}/api/tasks`, {
		data: { name, doDate: doDate.toISOString().split('T')[0], isRecurring: false, intervalDays: null }
	});
});

Given('a task {string} with no DoDate', async ({ tasksPage, request }, name: string) => {
	await request.post(`${BACKEND_URL}/api/tasks`, {
		data: { name, doDate: null, isRecurring: false, intervalDays: null }
	});
});

Given('a task {string} with no DoDate and no recurrence', async ({ tasksPage, request }, name: string) => {
	await request.post(`${BACKEND_URL}/api/tasks`, {
		data: { name, doDate: null, isRecurring: false, intervalDays: null }
	});
});

Given(
	'a recurring task {string} with DoDate today and interval {int} days',
	async ({ tasksPage, request }, name: string, intervalDays: number) => {
		const today = new Date().toISOString().split('T')[0];
		await request.post(`${BACKEND_URL}/api/tasks`, {
			data: { name, doDate: today, isRecurring: true, intervalDays }
		});
	}
);

When('I visit the Tasks page', async ({ tasksPage }) => {
	await tasksPage.goto();
});

Then('I see {string} in the Due section', async ({ tasksPage }, name: string) => {
	throw new Error('not implemented');
});

Then('I do not see {string} in the Due section', async ({ tasksPage }, name: string) => {
	throw new Error('not implemented');
});

Then('I see {string} in the Upcoming section', async ({ tasksPage }, name: string) => {
	throw new Error('not implemented');
});

When('I click Done on {string}', async ({ tasksPage }, name: string) => {
	throw new Error('not implemented');
});

Then('{string} is not visible in the Due section', async ({ tasksPage }, name: string) => {
	throw new Error('not implemented');
});

Then('an undo toast is visible', async ({ tasksPage }) => {
	throw new Error('not implemented');
});

When('I click Undo on the toast', async ({ tasksPage }) => {
	throw new Error('not implemented');
});

When('I open the Add Task modal', async ({ tasksPage }) => {
	throw new Error('not implemented');
});

When('I fill in the task name {string} with DoDate {int} days from now', async ({ tasksPage }, name: string, days: number) => {
	throw new Error('not implemented');
});

When('I fill in the recurring task name {string} with interval {int} days', async ({ tasksPage }, name: string, days: number) => {
	throw new Error('not implemented');
});

When('I submit the task form', async ({ tasksPage }) => {
	throw new Error('not implemented');
});

When('I open the Edit modal for {string}', async ({ tasksPage }, name: string) => {
	throw new Error('not implemented');
});

When('I update the task name to {string} and save', async ({ tasksPage }, name: string) => {
	throw new Error('not implemented');
});

When('I delete the task', async ({ tasksPage }) => {
	throw new Error('not implemented');
});

Then('{string} is not visible on the Tasks page', async ({ tasksPage }, name: string) => {
	throw new Error('not implemented');
});
