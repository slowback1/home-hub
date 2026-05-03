import { Page } from '@playwright/test';

export class FeatureFlagsPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		throw new Error('not implemented');
	}

	async hasHeading(text: string): Promise<boolean> {
		throw new Error('not implemented');
	}

	async hasFlagRow(name: string): Promise<boolean> {
		throw new Error('not implemented');
	}

	async isToggleOn(name: string): Promise<boolean> {
		throw new Error('not implemented');
	}

	async toggleFlag(name: string): Promise<void> {
		throw new Error('not implemented');
	}

	async hasSuccessToast(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async clickTab(label: string): Promise<void> {
		throw new Error('not implemented');
	}

	async currentPath(): Promise<string> {
		throw new Error('not implemented');
	}
}
