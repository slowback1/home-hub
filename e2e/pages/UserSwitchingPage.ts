import { Page } from '@playwright/test';

export class UserSwitchingPage {
	constructor(readonly page: Page) {}

	async gotoHome(): Promise<void> {
		await this.page.goto('/');
		await this.waitForGateResolved();
	}

	private async waitForGateResolved(): Promise<void> {
		await this.page.waitForSelector(
			'[data-testid="user-switcher"], [data-testid="sidebar-logo"]'
		);
	}

	async isSwitcherVisible(): Promise<boolean> {
		return await this.page.locator('[data-testid="user-switcher"]').isVisible();
	}

	async isSidebarVisible(): Promise<boolean> {
		return await this.page.locator('[data-testid="sidebar-logo"]').isVisible();
	}

	async isSwitcherShowingUser(name: string): Promise<boolean> {
		return await this.page
			.locator('[data-testid="user-switcher"]')
			.getByRole('button', { name })
			.isVisible();
	}

	async selectUserByName(name: string): Promise<void> {
		await this.page.getByRole('button', { name }).click();
		await this.page.waitForSelector('[data-testid="sidebar-logo"]');
	}
}
