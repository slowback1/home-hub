import { Page, expect } from '@playwright/test';

export class WheelsPage {
	constructor(readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/wheels');
		await this.page.waitForSelector('[data-testid="wheels-page"]', { state: 'visible' });
	}

	async createWheel(name: string, items: string): Promise<void> {
		await this.page.locator('[data-testid="wheel-form-name"]').fill(name);
		await this.page.locator('[data-testid="wheel-form-items"]').fill(items);
		await this.page.locator('[data-testid="wheel-form-save"]').click();
	}

	async editWheel(oldName: string, newName: string, items: string): Promise<void> {
		await this.page.locator(`[data-testid="edit-wheel-btn-${oldName}"]`).click();
		await this.page.locator('[data-testid="wheel-form-name"]').fill(newName);
		await this.page.locator('[data-testid="wheel-form-items"]').fill(items);
		await this.page.locator('[data-testid="wheel-form-save"]').click();
	}

	async deleteWheel(name: string): Promise<void> {
		await this.page.locator(`[data-testid="delete-wheel-btn-${name}"]`).click();
	}

	async selectWheelInSpin(name: string): Promise<void> {
		await this.page.locator('[data-testid="wheel-select"]').selectOption({ label: name });
	}

	async clickSpin(): Promise<void> {
		await this.page.locator('[data-testid="spin-btn"]').click();
	}

	async getSpinResult(): Promise<string> {
		const result = this.page.locator('[data-testid="wheel-spin-result"]');
		await result.waitFor({ state: 'visible' });
		return (await result.textContent())?.trim() ?? '';
	}

	async isSpinDisabled(): Promise<boolean> {
		return this.page.locator('[data-testid="spin-btn"]').isDisabled();
	}

	async expectWheelRow(name: string): Promise<void> {
		await expect(this.page.locator(`[data-testid="wheel-row-${name}"]`)).toBeVisible();
	}

	async expectNoWheelRow(name: string): Promise<void> {
		await expect(this.page.locator(`[data-testid="wheel-row-${name}"]`)).toHaveCount(0);
	}

	async getItemCountText(name: string): Promise<string> {
		return (
			(await this.page.locator(`[data-testid="wheel-item-count-${name}"]`).textContent())?.trim() ??
			''
		);
	}

	async isEmptyStateVisible(): Promise<boolean> {
		return this.page.locator('[data-testid="wheels-empty-state"]').isVisible();
	}
}
