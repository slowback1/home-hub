import { Page, expect } from '@playwright/test';

export class PremadeListsPage {
	constructor(readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/premade-lists');
		await this.page.waitForSelector('[data-testid="premade-lists-page"]', { state: 'visible' });
	}

	async createList(name: string): Promise<void> {
		await this.page.locator('[data-testid="premade-list-form-name"]').fill(name);
		await this.page.locator('[data-testid="premade-list-form-add"]').click();
		await this.expectListRow(name);
	}

	async addSavedItem(listName: string, savedItemName: string): Promise<void> {
		await this.page
			.locator(`[data-testid="premade-list-pick-${listName}"]`)
			.selectOption({ label: savedItemName });
		await this.page.locator(`[data-testid="premade-list-add-item-${listName}"]`).click();
		await this.expectSavedItem(listName, savedItemName);
	}

	async expectListRow(name: string): Promise<void> {
		await expect(this.page.locator(`[data-testid="premade-list-row-${name}"]`)).toBeVisible();
	}

	async expectSavedItem(listName: string, savedItemName: string): Promise<void> {
		await expect(
			this.page.locator(`[data-testid="premade-list-item-${listName}-${savedItemName}"]`)
		).toBeVisible();
	}
}
