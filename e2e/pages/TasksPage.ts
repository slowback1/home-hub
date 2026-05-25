import { Page, expect } from '@playwright/test';

export class TasksPage {
	constructor(readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/tasks');
		await this.page.waitForSelector('[data-testid="tasks-page"]', { state: 'visible' });
		await this.page.waitForSelector('[data-testid="loading"]', { state: 'detached' });
	}

	async isTaskVisibleInDueSection(name: string): Promise<boolean> {
		const dueSection = this.page.locator('[data-testid="due-section"]');
		const row = dueSection.locator('[data-testid="due-task-row"]').filter({ hasText: name });
		return row.isVisible();
	}

	async isTaskVisibleInUpcomingSection(name: string): Promise<boolean> {
		const upcomingSection = this.page.locator('[data-testid="upcoming-section"]');
		const row = upcomingSection
			.locator('[data-testid="upcoming-task-row"]')
			.filter({ hasText: name });
		return row.isVisible();
	}

	async isTaskVisibleAnywhere(name: string): Promise<boolean> {
		const tasksPage = this.page.locator('[data-testid="tasks-page"]');
		const nameEl = tasksPage.locator('[data-testid="task-name"]').filter({ hasText: name });
		return nameEl.isVisible();
	}

	async clickDone(taskName: string): Promise<void> {
		await this.page.click(`[aria-label="Done ${taskName}"]`);
	}

	async isUndoToastVisible(): Promise<boolean> {
		return this.page.locator('[data-testid="undo-toast"]').isVisible();
	}

	async clickUndoOnToast(): Promise<void> {
		await this.page.click('[data-testid="undo-button"]');
	}

	async openAddTaskModal(): Promise<void> {
		await this.page.click('[data-testid="add-task-button"]');
		await this.page.waitForSelector('[data-testid="task-modal"]', { state: 'visible' });
	}

	async openEditModal(taskName: string): Promise<void> {
		await this.page.click(`[aria-label="Edit ${taskName}"]`);
		await this.page.waitForSelector('[data-testid="task-modal"]', { state: 'visible' });
	}

	async fillOneOffTask(name: string, daysFromNow: number): Promise<void> {
		await this.page.fill('[data-testid="task-name-input"]', name);
		const date = new Date();
		date.setDate(date.getDate() + daysFromNow);
		const dateStr = date.toISOString().split('T')[0];
		await this.page.fill('[data-testid="task-do-date-input"]', dateStr);
	}

	async fillRecurringTask(name: string, intervalDays: number): Promise<void> {
		await this.page.fill('[data-testid="task-name-input"]', name);
		await this.page.check('[data-testid="task-recurring-toggle"]');
		await this.page.fill('[data-testid="task-interval-input"]', String(intervalDays));
	}

	async updateTaskName(newName: string): Promise<void> {
		await this.page.fill('[data-testid="task-name-input"]', newName);
	}

	async submitTaskForm(): Promise<void> {
		await this.page.click('[data-testid="submit-task-button"]');
		await this.page.waitForSelector('[data-testid="task-modal"]', { state: 'detached' });
	}

	async deleteTask(): Promise<void> {
		await this.page.click('[data-testid="delete-task-button"]');
		await this.page.waitForSelector('[data-testid="task-modal"]', { state: 'detached' });
	}
}
