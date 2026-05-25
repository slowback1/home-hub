import { Page } from '@playwright/test';

export class TasksPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		throw new Error('not implemented');
	}

	async isTaskVisibleInDueSection(name: string): Promise<boolean> {
		throw new Error('not implemented');
	}

	async isTaskVisibleInUpcomingSection(name: string): Promise<boolean> {
		throw new Error('not implemented');
	}

	async isTaskVisibleAnywhere(name: string): Promise<boolean> {
		throw new Error('not implemented');
	}

	async clickDone(taskName: string): Promise<void> {
		throw new Error('not implemented');
	}

	async isUndoToastVisible(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async clickUndoOnToast(): Promise<void> {
		throw new Error('not implemented');
	}

	async openAddTaskModal(): Promise<void> {
		throw new Error('not implemented');
	}

	async openEditModal(taskName: string): Promise<void> {
		throw new Error('not implemented');
	}

	async fillOneOffTask(name: string, daysFromNow: number): Promise<void> {
		throw new Error('not implemented');
	}

	async fillRecurringTask(name: string, intervalDays: number): Promise<void> {
		throw new Error('not implemented');
	}

	async updateTaskName(newName: string): Promise<void> {
		throw new Error('not implemented');
	}

	async submitTaskForm(): Promise<void> {
		throw new Error('not implemented');
	}

	async deleteTask(): Promise<void> {
		throw new Error('not implemented');
	}
}
