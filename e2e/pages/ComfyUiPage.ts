import { Page } from '@playwright/test';

export class ComfyUiPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/comfyui');
		await this.page.waitForSelector('[data-testid="generate-form"]', { state: 'visible' });
	}

	async selectFirstWorkflow(): Promise<void> {
		// The first option is auto-selected on load; no action needed unless testing explicitly
	}

	async enterPrompt(prompt: string): Promise<void> {
		await this.page.fill('[data-testid="prompt-input"]', prompt);
	}

	async clickGenerate(): Promise<void> {
		await this.page.click('[data-testid="generate-btn"]');
	}

	async isLoadingVisible(): Promise<boolean> {
		return this.page.locator('[data-testid="generating-indicator"]').isVisible();
	}

	async isImageVisible(): Promise<boolean> {
		return this.page.locator('[data-testid="result-image"]').isVisible();
	}

	async isErrorVisible(): Promise<boolean> {
		return this.page.locator('[data-testid="generate-error"]').isVisible();
	}

	async isGenerateButtonEnabled(): Promise<boolean> {
		return this.page.locator('[data-testid="generate-btn"]').isEnabled();
	}

	async waitForImage(timeout = 30000): Promise<void> {
		await this.page.locator('[data-testid="result-image"]').waitFor({ state: 'visible', timeout });
	}

	async waitForError(timeout = 30000): Promise<void> {
		await this.page.locator('[data-testid="generate-error"]').waitFor({ state: 'visible', timeout });
	}

	async getResultImageSrc(): Promise<string | null> {
		return this.page.locator('[data-testid="result-image"]').getAttribute('src');
	}
}
