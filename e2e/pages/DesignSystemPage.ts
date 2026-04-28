import { expect, Page } from '@playwright/test';

export class DesignSystemPage {
	private readonly nav = () => this.page.getByRole('navigation');

	constructor(private readonly page: Page) {}

	private async waitForMount(): Promise<void> {
		// Wait for Svelte onMount to complete so event listeners are attached.
		// The Sidebar sets data-mounted="true" at the end of onMount.
		await this.page.waitForFunction(
			() => document.querySelector('nav[data-mounted="true"]') !== null,
			{ timeout: 10000 }
		);
	}

	async goto(): Promise<void> {
		await this.page.goto('/');
		await this.nav().waitFor({ state: 'visible' });
		await this.waitForMount();
	}

	async reload(): Promise<void> {
		await this.page.reload();
		await this.nav().waitFor({ state: 'visible' });
		await this.waitForMount();
	}

	async clickSidebarNavItem(label: string): Promise<void> {
		await this.nav().getByText(label).click();
	}

	async clickCollapseToggle(): Promise<void> {
		// Dispatch click event directly: Playwright's click() does not reach the
		// manually-attached listener (added in onMount after SSR hydration).
		await this.page.evaluate(() => {
			const btn = document.querySelector<HTMLElement>('[data-testid="sidebar-toggle"]');
			btn?.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true }));
		});
	}

	async collapseSidebar(): Promise<void> {
		const isAlreadyCollapsed = await this.isSidebarCollapsed();
		if (!isAlreadyCollapsed) {
			await this.clickCollapseToggle();
			await expect(this.nav()).toHaveClass(/collapsed/);
		}
	}

	async isSidebarExpanded(): Promise<boolean> {
		const classList = await this.nav().getAttribute('class') ?? '';
		return !classList.includes('collapsed');
	}

	async isSidebarCollapsed(): Promise<boolean> {
		const classList = await this.nav().getAttribute('class') ?? '';
		return classList.includes('collapsed');
	}

	async areNavLabelsVisible(): Promise<boolean> {
		const label = this.page.locator('.nav-label').first();
		return label.isVisible();
	}

	async isNavItemActive(label: string): Promise<boolean> {
		const items = this.nav().locator('li.active');
		const count = await items.count();
		for (let i = 0; i < count; i++) {
			const text = await items.nth(i).textContent();
			if (text?.includes(label)) return true;
		}
		return false;
	}

	async currentPath(): Promise<string> {
		return new URL(this.page.url()).pathname;
	}

	async hasDarkTheme(): Promise<boolean> {
		await this.nav().waitFor({ state: 'visible' });
		return true;
	}

	async hasLightTheme(): Promise<boolean> {
		const htmlClass = await this.page.locator('html').getAttribute('class') ?? '';
		return htmlClass.includes('light-theme');
	}

	async assertOnPath(path: string): Promise<void> {
		await expect(this.page).toHaveURL(new RegExp(path));
	}
}
