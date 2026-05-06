import { expect, Page } from '@playwright/test';
import * as path from 'path';

const WAV_FIXTURE = path.join(__dirname, '../fixtures/sample.wav');

export class AudiobookVoiceSamplesPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/audiobook/voice-samples');
		// Wait until either samples are loaded or the empty state message appears
		await this.page.waitForSelector(
			'[data-testid="voice-sample-list"], [data-testid="no-samples-message"]',
			{ state: 'visible', timeout: 10_000 }
		);
	}

	async uploadWavFile(wavPath: string = WAV_FIXTURE): Promise<void> {
		const beforeCount = await this.getSampleCount();
		await this.page.locator('[data-testid="wav-file-input"]').setInputFiles(wavPath);
		// Wait for the new sample to appear in the list
		await this.page
			.locator('[data-testid="voice-sample-item"]')
			.nth(beforeCount)
			.waitFor({ state: 'visible', timeout: 5000 });
	}

	async getVoiceSampleNames(): Promise<string[]> {
		const items = await this.page.locator('[data-testid="voice-sample-item"]').all();
		return Promise.all(
			items.map(async (item) => (await item.getAttribute('data-sample-name')) ?? '')
		);
	}

	async getSampleCount(): Promise<number> {
		return this.page.locator('[data-testid="voice-sample-item"]').count();
	}

	async deleteFirstSample(): Promise<void> {
		const beforeCount = await this.getSampleCount();
		await this.page.locator('[data-testid="delete-voice-sample-button"]').first().click();
		if (beforeCount > 0) {
			// Wait for the count to decrease
			await expect(this.page.locator('[data-testid="voice-sample-item"]')).toHaveCount(
				beforeCount - 1,
				{ timeout: 5000 }
			);
		}
	}

	async hasSample(name: string): Promise<boolean> {
		return this.page.locator(`[data-sample-name="${name}"]`).isVisible();
	}
}
