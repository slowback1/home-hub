import { Page } from '@playwright/test';

export class DesignSystemPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/');
	}

	async reload(): Promise<void> {
		await this.page.reload();
	}

	async clickSidebarNavItem(label: string): Promise<void> {
		throw new Error('not implemented');
	}

	async clickCollapseToggle(): Promise<void> {
		throw new Error('not implemented');
	}

	async collapseSidebar(): Promise<void> {
		throw new Error('not implemented');
	}

	async isSidebarExpanded(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async isSidebarCollapsed(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async areNavLabelsVisible(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async isNavItemActive(label: string): Promise<boolean> {
		throw new Error('not implemented');
	}

	async currentPath(): Promise<string> {
		return new URL(this.page.url()).pathname;
	}

	async hasDarkTheme(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async hasLightTheme(): Promise<boolean> {
		throw new Error('not implemented');
	}
}
