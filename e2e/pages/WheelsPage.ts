import { Page } from '@playwright/test';

export class WheelsPage {
	constructor(readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/wheels');
		await this.page.waitForSelector('[data-testid="wheels-page"]', { state: 'visible' });
	}

	// Helpers are implemented in task 09 (drive scenarios GREEN).
}
