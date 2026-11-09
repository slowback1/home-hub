import { Page, expect } from '@playwright/test';

export class ShoppingListsPage {
	constructor(readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/shopping-lists');
		await this.page.waitForSelector('[data-testid="shopping-lists-page"]', { state: 'visible' });
	}

	async createList(name: string): Promise<void> {
		await this.page.locator('[data-testid="shopping-list-form-name"]').fill(name);
		await this.page.locator('[data-testid="shopping-list-form-add"]').click();
		await this.expectListRow(name);
	}

	async addItem(listName: string, text: string): Promise<void> {
		await this.page.locator(`[data-testid="shopping-item-input-${listName}"]`).fill(text);
		await this.page.locator(`[data-testid="shopping-item-add-${listName}"]`).click();
		await this.expectItem(text);
	}

	async toggleItem(_listName: string, text: string): Promise<void> {
		const item = this.page.locator(`[data-testid="shopping-item-${text}"]`);
		await item.click();
		await expect(item).toHaveClass(/shopping-item--checked/);
	}

	async completeList(name: string): Promise<void> {
		await this.page.locator(`[data-testid="shopping-list-complete-${name}"]`).click();
	}

	async expectListRow(name: string): Promise<void> {
		await expect(this.page.locator(`[data-testid="shopping-list-row-${name}"]`)).toBeVisible();
	}

	async expectNoListRow(name: string): Promise<void> {
		await expect(this.page.locator(`[data-testid="shopping-list-row-${name}"]`)).toHaveCount(0);
	}

	async expectArchivedRow(name: string): Promise<void> {
		await expect(this.page.locator(`[data-testid="shopping-archived-row-${name}"]`)).toBeVisible();
	}

	async expectItem(text: string): Promise<void> {
		await expect(this.page.locator(`[data-testid="shopping-item-${text}"]`)).toBeVisible();
	}

	async expectArchivedItem(text: string): Promise<void> {
		await expect(this.page.locator(`[data-testid="shopping-archived-item-${text}"]`)).toBeVisible();
	}

	async isItemChecked(text: string): Promise<boolean> {
		const cls = await this.page
			.locator(`[data-testid="shopping-item-${text}"]`)
			.getAttribute('class');
		return cls?.includes('shopping-item--checked') ?? false;
	}

	async isEmptyStateVisible(): Promise<boolean> {
		return this.page.locator('[data-testid="shopping-lists-empty-state"]').isVisible();
	}
}
