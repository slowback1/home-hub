import { Page } from '@playwright/test';

export class ComfyUiPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		throw new Error('not implemented');
	}

	async selectFirstWorkflow(): Promise<void> {
		throw new Error('not implemented');
	}

	async enterPrompt(prompt: string): Promise<void> {
		throw new Error('not implemented');
	}

	async clickGenerate(): Promise<void> {
		throw new Error('not implemented');
	}

	async isLoadingVisible(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async isImageVisible(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async isErrorVisible(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async isGenerateButtonEnabled(): Promise<boolean> {
		throw new Error('not implemented');
	}
}
