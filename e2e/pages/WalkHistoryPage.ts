import { Page } from '@playwright/test';

export class WalkHistoryPage {
	constructor(readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/walk-history');
		await this.page.waitForSelector('[data-testid="walk-history-page"]', { state: 'visible' });
	}

	async getSessionRowCount(): Promise<number> {
		return this.page.locator('[data-testid="walk-session-row"]').count();
	}

	async isEmptyStateVisible(): Promise<boolean> {
		return this.page.locator('[data-testid="walk-empty-state"]').isVisible();
	}

	async getFirstRowRelativeTime(): Promise<string> {
		const text = await this.page
			.locator('[data-testid="walk-session-row"]')
			.first()
			.locator('.when-primary')
			.textContent();
		return text?.trim() ?? '';
	}

	async isStepCountVisible(): Promise<boolean> {
		return this.page.locator('[data-testid="walk-session-steps"]').first().isVisible();
	}

	async isDurationVisible(): Promise<boolean> {
		return this.page.locator('[data-testid="walk-session-duration"]').first().isVisible();
	}
}
