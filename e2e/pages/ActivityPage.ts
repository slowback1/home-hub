import { Page } from '@playwright/test';

export class ActivityPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		throw new Error('not implemented');
	}

	async clearPickHistory(): Promise<void> {
		throw new Error('not implemented');
	}

	async seedCurrentPick(activityName: string): Promise<void> {
		throw new Error('not implemented');
	}

	async isPlaceholderVisible(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async getCurrentPickName(): Promise<string> {
		throw new Error('not implemented');
	}
}
