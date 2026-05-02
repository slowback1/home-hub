import { Page } from '@playwright/test';

export class ActivityConfigPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		throw new Error('not implemented');
	}

	async addActivity(name: string, weight: number): Promise<void> {
		throw new Error('not implemented');
	}

	async seedActivity(name: string, weight: number): Promise<void> {
		throw new Error('not implemented');
	}

	async changeWeight(name: string, weight: number): Promise<void> {
		throw new Error('not implemented');
	}

	async deleteActivity(name: string): Promise<void> {
		throw new Error('not implemented');
	}

	async isActivityVisible(name: string): Promise<boolean> {
		throw new Error('not implemented');
	}

	async getActivityWeight(name: string): Promise<number> {
		throw new Error('not implemented');
	}
}
