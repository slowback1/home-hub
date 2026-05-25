import { expect, Page } from '@playwright/test';

export class ActivityConfigPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/activity/config');
		await this.page.waitForSelector('[data-testid="activity-table"]', { state: 'visible' });
	}

	async addActivity(name: string, weight: number): Promise<void> {
		await this.page.fill('[data-testid="new-activity-name"]', name);
		await this.page.click(`[data-testid="new-weight-btn-${weight}"]`);
		await this.page.click('[data-testid="add-activity-btn"]');
		await this.page.waitForSelector(`[data-testid="activity-row-${name}"]`, { state: 'visible' });
	}

	async changeWeight(name: string, weight: number): Promise<void> {
		await this.page.click(`[data-testid="weight-btn-${name}-${weight}"]`);
		await expect(this.page.locator(`[data-testid="weight-btn-${name}-${weight}"]`)).toHaveClass(
			/weight-btn--active/
		);
	}

	async deleteActivity(name: string): Promise<void> {
		await this.page.click(`[data-testid="delete-btn-${name}"]`);
		await this.page.waitForSelector(`[data-testid="activity-row-${name}"]`, { state: 'detached' });
	}

	async isActivityVisible(name: string): Promise<boolean> {
		return this.page.locator(`[data-testid="activity-row-${name}"]`).isVisible();
	}

	async getActivityWeight(name: string): Promise<number> {
		for (let w = 1; w <= 5; w++) {
			const btn = this.page.locator(`[data-testid="weight-btn-${name}-${w}"]`);
			const classes = await btn.getAttribute('class');
			if (classes?.includes('weight-btn--active')) return w;
		}
		return 0;
	}

	// eslint-disable-next-line @typescript-eslint/no-unused-vars
	async seedActivity(_name: string, _weight: number): Promise<void> {
		// Seeding is done via API in step definitions using the request fixture
	}
}
