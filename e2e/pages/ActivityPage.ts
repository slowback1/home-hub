import { Page } from '@playwright/test';

const BACKEND_URL = 'http://localhost:5273';

export class ActivityPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/activity');
		await this.page.waitForSelector('[data-testid="hero"]', { state: 'visible' });
	}

	async clearPickHistory(): Promise<void> {
		await this.page.request.delete(`${BACKEND_URL}/api/test/activity-picks`);
	}

	async seedCurrentPick(activityName: string): Promise<void> {
		const pickedAt = new Date();
		pickedAt.setMinutes(0, 0, 0);
		await this.page.request.post(`${BACKEND_URL}/api/test/activity-picks`, {
			data: { activityName, pickedAt: pickedAt.toISOString() }
		});
	}

	async isPlaceholderVisible(): Promise<boolean> {
		return this.page.locator('[data-testid="placeholder"]').isVisible();
	}

	async getCurrentPickName(): Promise<string> {
		const el = this.page.locator('[data-testid="current-activity"]');
		return (await el.textContent()) ?? '';
	}
}
