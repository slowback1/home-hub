import { Page, expect } from '@playwright/test';
import * as path from 'path';

const EPUB_FIXTURE = path.join(__dirname, '../fixtures/sample.epub');
const STATUS_POLL_INTERVAL = 500;

export class AudiobookConvertPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/audiobook');
		await this.page.waitForSelector('[data-testid="conversion-form"]', { state: 'attached' });
	}

	async hasVoiceSamples(): Promise<boolean> {
		const options = await this.page.locator('[data-testid="voice-sample-select"] option').count();
		return options > 0;
	}

	async isFormDisabled(): Promise<boolean> {
		return this.page.locator('[data-testid="epub-file-input"]').isDisabled();
	}

	async hasNoVoiceSamplesMessage(): Promise<boolean> {
		return this.page.locator('[data-testid="no-voice-samples-message"]').isVisible();
	}

	async uploadEpubFile(epubPath: string = EPUB_FIXTURE): Promise<void> {
		await this.page.locator('[data-testid="epub-file-input"]').setInputFiles(epubPath);
	}

	async selectFirstVoiceSample(): Promise<void> {
		const select = this.page.locator('[data-testid="voice-sample-select"]');
		const firstOption = await select.locator('option').first().getAttribute('value');
		if (firstOption) await select.selectOption(firstOption);
	}

	async submitForm(): Promise<void> {
		await this.page.locator('[data-testid="submit-job-button"]').click();
	}

	async getLastJobStatus(): Promise<string> {
		const lastRow = this.page.locator('[data-testid="job-row"]').last();
		const badge = lastRow.locator('[data-testid="job-status"]');
		return (await badge.textContent())?.trim() ?? '';
	}

	async waitForLastJobStatus(status: string, timeoutMs = 15_000): Promise<void> {
		const expectedText = status.replace('_', ' ');
		const lastRow = this.page.locator('[data-testid="job-row"]').last();
		await expect(lastRow.locator('[data-testid="job-status"]')).toHaveText(expectedText, {
			timeout: timeoutMs
		});
	}

	async hasDownloadButtonForLastJob(): Promise<boolean> {
		const lastRow = this.page.locator('[data-testid="job-row"]').last();
		return lastRow.locator('[data-testid="download-job-button"]').isVisible();
	}

	async clickCancelOnLastJob(): Promise<void> {
		const lastRow = this.page.locator('[data-testid="job-row"]').last();
		await lastRow.locator('[data-testid="cancel-job-button"]').click();
	}

	async clickDownloadOnLastJob(): Promise<void> {
		const lastRow = this.page.locator('[data-testid="job-row"]').last();
		await lastRow.locator('[data-testid="download-job-button"]').click();
	}

	async hasErrorMessageOnLastJob(): Promise<boolean> {
		const lastRow = this.page.locator('[data-testid="job-row"]').last();
		return lastRow.locator('[data-testid="job-error-message"]').isVisible();
	}

	async submitJobAndWaitForStatus(
		epubPath: string,
		targetStatus: string,
		timeoutMs = 15_000
	): Promise<void> {
		await this.uploadEpubFile(epubPath);
		await this.selectFirstVoiceSample();
		await this.submitForm();
		await this.waitForLastJobStatus(targetStatus, timeoutMs);
	}
}
