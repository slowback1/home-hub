import { Page } from '@playwright/test';
import * as path from 'path';

const WAV_FIXTURE = path.join(__dirname, '../fixtures/sample.wav');

export class AudiobookVoiceSamplesPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/audiobook/voice-samples');
		await this.page.waitForSelector('section', { state: 'visible' });
	}

	async uploadWavFile(wavPath: string = WAV_FIXTURE): Promise<void> {
		await this.page.locator('[data-testid="wav-file-input"]').setInputFiles(wavPath);
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
		await this.page.locator('[data-testid="delete-voice-sample-button"]').first().click();
	}

	async hasSample(name: string): Promise<boolean> {
		return this.page.locator(`[data-sample-name="${name}"]`).isVisible();
	}
}
