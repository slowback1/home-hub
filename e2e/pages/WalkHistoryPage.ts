import { Page } from '@playwright/test';

export class WalkHistoryPage {
	constructor(readonly page: Page) {}

	async goto(): Promise<void> {
		throw new Error('not implemented');
	}

	async getSessionRowCount(): Promise<number> {
		throw new Error('not implemented');
	}

	async isEmptyStateVisible(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async getSessionDates(): Promise<string[]> {
		throw new Error('not implemented');
	}

	async isStepCountVisible(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async isDurationVisible(): Promise<boolean> {
		throw new Error('not implemented');
	}
}
