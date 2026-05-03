import { expect, Page } from '@playwright/test';

export class FeatureFlagsPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/admin/feature-flags');
		await this.page.waitForSelector('h1', { state: 'visible' });
	}

	async hasHeading(text: string): Promise<boolean> {
		return this.page.locator('h1').filter({ hasText: text }).isVisible();
	}

	async hasFlagRow(displayName: string): Promise<boolean> {
		const name = this.toRawName(displayName);
		return this.page.locator(`[data-testid="flag-row-${name}"]`).isVisible();
	}

	async isToggleOn(displayName: string): Promise<boolean> {
		const name = this.toRawName(displayName);
		const switchBtn = this.page.locator(`[data-testid="flag-toggle-${name}"] [role="switch"]`);
		await switchBtn.waitFor({ state: 'visible' });
		const ariaChecked = await switchBtn.getAttribute('aria-checked');
		return ariaChecked === 'true';
	}

	async toggleFlag(displayName: string): Promise<void> {
		const name = this.toRawName(displayName);
		await this.page.locator(`[data-testid="flag-toggle-${name}"] [role="switch"]`).click();
	}

	async hasSuccessToast(): Promise<boolean> {
		const toast = this.page.locator('[data-testid="toast-item"].toast-item__success');
		await toast.waitFor({ state: 'visible', timeout: 5000 });
		return toast.isVisible();
	}

	async clickTab(label: string): Promise<void> {
		const link = this.page.locator('a.admin-tab', { hasText: label });
		const href = await link.getAttribute('href');
		await link.click();
		if (href) await this.page.waitForURL(`**${href}`);
	}

	async currentPath(): Promise<string> {
		return new URL(this.page.url()).pathname;
	}

	private toRawName(displayName: string): string {
		return displayName.toUpperCase().replace(/ /g, '_');
	}
}
