import { Page } from '@playwright/test';

export class DashboardPage {
	constructor(readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/');
		await this.page.waitForSelector('[data-testid="dashboard-page"]');
	}

	async isHeroVisible(): Promise<boolean> {
		return await this.page.locator('[data-testid="first-run-hero"]').isVisible();
	}

	async emptySlotCount(): Promise<number> {
		return await this.page.locator('[data-testid="empty-slot"]').count();
	}

	async addWidgetButtonCount(): Promise<number> {
		return await this.page.locator('[data-testid="add-widget-button"]').count();
	}

	async clickAddWidget(slotIndex: number): Promise<void> {
		await this.page.locator('[data-testid="add-widget-button"]').nth(slotIndex).click();
	}

	async isPickerModalOpen(): Promise<boolean> {
		return await this.page.locator('[data-testid="widget-picker-modal"]').isVisible();
	}

	async selectFirstAvailableWidget(): Promise<string> {
		const card = this.page.locator('[data-testid="widget-picker-card"]').first();
		const widgetId = (await card.getAttribute('data-widget-id')) ?? '';
		await card.click();
		return widgetId;
	}

	async isPickerModalClosed(): Promise<boolean> {
		return await this.page.locator('[data-testid="widget-picker-modal"]').isHidden();
	}

	async filledSlotWidgetId(slotIndex: number): Promise<string | null> {
		return await this.page
			.locator(`[data-testid="filled-slot"][data-slot-index="${slotIndex}"]`)
			.getAttribute('data-widget-id');
	}

	async isSlotFilled(slotIndex: number): Promise<boolean> {
		return await this.page
			.locator(`[data-testid="filled-slot"][data-slot-index="${slotIndex}"]`)
			.isVisible();
	}

	async isSlotEmpty(slotIndex: number): Promise<boolean> {
		return await this.page
			.locator(`[data-testid="empty-slot"][data-slot-index="${slotIndex}"]`)
			.isVisible();
	}

	async occupiedSlotCount(): Promise<number> {
		return await this.page.locator('[data-testid="filled-slot"]').count();
	}

	async reload(): Promise<void> {
		await this.page.reload();
		await this.page.waitForSelector('[data-testid="dashboard-page"]');
	}

	async clickEditDashboard(): Promise<void> {
		await this.page.click('[data-testid="edit-dashboard-button"]');
	}

	async removeButtonCount(): Promise<number> {
		return await this.page.locator('[data-testid="remove-widget-button"]').count();
	}

	async isDoneButtonVisible(): Promise<boolean> {
		return await this.page.locator('[data-testid="done-edit-button"]').isVisible();
	}

	async clickRemoveWidget(slotIndex: number): Promise<void> {
		await this.page
			.locator(
				`[data-testid="filled-slot"][data-slot-index="${slotIndex}"] [data-testid="remove-widget-button"]`
			)
			.click();
	}

	async clickDone(): Promise<void> {
		await this.page.click('[data-testid="done-edit-button"]');
	}
}
