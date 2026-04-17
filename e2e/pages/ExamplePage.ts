import { expect, Page } from '@playwright/test';

export class ExamplePage {
	constructor(private readonly page: Page) {}

	async visit(): Promise<void> {
		await this.page.goto('/');
	}

	async doSomething() {
		await this.page.click('html');
	}

	async assertSomething() {
		expect(this.page.getByText('Svelte')).toBeDefined();
	}
}
