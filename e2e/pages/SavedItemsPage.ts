import { Page, expect } from '@playwright/test';

export class SavedItemsPage {
	constructor(readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/saved-items');
		await this.page.waitForSelector('[data-testid="saved-items-page"]', { state: 'visible' });
	}

	async createItem(name: string): Promise<void> {
		await this.page.locator('[data-testid="saved-item-form-name"]').fill(name);
		await this.page.locator('[data-testid="saved-item-form-add"]').click();
		await this.expectItemRow(name);
	}

	async editItem(currentName: string, newName: string): Promise<void> {
		await this.page.locator(`[data-testid="saved-item-edit-${currentName}"]`).click();
		await this.page.locator(`[data-testid="saved-item-edit-input-${currentName}"]`).fill(newName);
		await this.page.locator(`[data-testid="saved-item-save-${currentName}"]`).click();
		await this.expectItemRow(newName);
	}

	async expectItemRow(name: string): Promise<void> {
		await expect(this.page.locator(`[data-testid="saved-item-row-${name}"]`)).toBeVisible();
	}

	async expectNoItemRow(name: string): Promise<void> {
		await expect(this.page.locator(`[data-testid="saved-item-row-${name}"]`)).toHaveCount(0);
	}

	async isEmptyStateVisible(): Promise<boolean> {
		return this.page.locator('[data-testid="saved-items-empty-state"]').isVisible();
	}
}
