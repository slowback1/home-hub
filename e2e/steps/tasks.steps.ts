import { expect } from '@playwright/test';
import { Given, When, Then, Before } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';

Before({ tags: '@tasks' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/tasks`);
});

Given(
	'a task {string} with DoDate {int} days ago',
	async ({ request }, name: string, days: number) => {
		const doDate = new Date();
		doDate.setDate(doDate.getDate() - days);
		await request.post(`${BACKEND_URL}/api/tasks`, {
			data: {
				name,
				doDate: doDate.toISOString().split('T')[0],
				isRecurring: false,
				intervalDays: null
			}
		});
	}
);

Given(
	'a task {string} with DoDate {int} days from now',
	async ({ request }, name: string, days: number) => {
		const doDate = new Date();
		doDate.setDate(doDate.getDate() + days);
		await request.post(`${BACKEND_URL}/api/tasks`, {
			data: {
				name,
				doDate: doDate.toISOString().split('T')[0],
				isRecurring: false,
				intervalDays: null
			}
		});
	}
);

Given('a task {string} with no DoDate', async ({ request }, name: string) => {
	await request.post(`${BACKEND_URL}/api/tasks`, {
		data: { name, doDate: null, isRecurring: false, intervalDays: null }
	});
});

Given('a task {string} with no DoDate and no recurrence', async ({ request }, name: string) => {
	await request.post(`${BACKEND_URL}/api/tasks`, {
		data: { name, doDate: null, isRecurring: false, intervalDays: null }
	});
});

Given(
	'a recurring task {string} with DoDate today and interval {int} days',
	async ({ request }, name: string, intervalDays: number) => {
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
	const dueSection = tasksPage.page.locator('[data-testid="due-section"]');
	const row = dueSection.locator('[data-testid="due-task-row"]').filter({ hasText: name });
	await expect(row).toBeVisible({ timeout: 5000 });
});

Then('I do not see {string} in the Due section', async ({ tasksPage }, name: string) => {
	const visible = await tasksPage.isTaskVisibleInDueSection(name);
	expect(visible).toBe(false);
});

Then('I see {string} in the Upcoming section', async ({ tasksPage }, name: string) => {
	const visible = await tasksPage.isTaskVisibleInUpcomingSection(name);
	expect(visible).toBe(true);
});

When('I click Done on {string}', async ({ tasksPage }, name: string) => {
	await tasksPage.clickDone(name);
});

Then('{string} is not visible in the Due section', async ({ tasksPage }, name: string) => {
	await tasksPage.page.waitForFunction(
		(taskName) => {
			const dueSection = document.querySelector('[data-testid="due-section"]');
			if (!dueSection) return true;
			const names = dueSection.querySelectorAll('[data-testid="task-name"]');
			return !Array.from(names).some((el) => el.textContent?.includes(taskName));
		},
		name,
		{ timeout: 5000 }
	);
	const visible = await tasksPage.isTaskVisibleInDueSection(name);
	expect(visible).toBe(false);
});

Then('an undo toast is visible', async ({ tasksPage }) => {
	await tasksPage.page.waitForSelector('[data-testid="undo-toast"]', { state: 'visible' });
	const visible = await tasksPage.isUndoToastVisible();
	expect(visible).toBe(true);
});

When('I click Undo on the toast', async ({ tasksPage }) => {
	await tasksPage.clickUndoOnToast();
});

When('I open the Add Task modal', async ({ tasksPage }) => {
	await tasksPage.openAddTaskModal();
});

When(
	'I fill in the task name {string} with DoDate {int} days from now',
	async ({ tasksPage }, name: string, days: number) => {
		await tasksPage.fillOneOffTask(name, days);
	}
);

When(
	'I fill in the recurring task name {string} with interval {int} days',
	async ({ tasksPage }, name: string, days: number) => {
		await tasksPage.fillRecurringTask(name, days);
	}
);

When('I submit the task form', async ({ tasksPage }) => {
	await tasksPage.submitTaskForm();
});

When('I open the Edit modal for {string}', async ({ tasksPage }, name: string) => {
	await tasksPage.openEditModal(name);
});

When('I update the task name to {string} and save', async ({ tasksPage }, name: string) => {
	await tasksPage.updateTaskName(name);
	await tasksPage.submitTaskForm();
});

When('I delete the task', async ({ tasksPage }) => {
	await tasksPage.deleteTask();
});

Then('{string} is not visible on the Tasks page', async ({ tasksPage }, name: string) => {
	await tasksPage.page.waitForFunction(
		(taskName) => {
			const page = document.querySelector('[data-testid="tasks-page"]');
			if (!page) return true;
			const names = page.querySelectorAll('[data-testid="task-name"]');
			return !Array.from(names).some((el) => el.textContent?.includes(taskName));
		},
		name,
		{ timeout: 5000 }
	);
	const visible = await tasksPage.isTaskVisibleAnywhere(name);
	expect(visible).toBe(false);
});
