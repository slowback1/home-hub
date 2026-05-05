import { Page } from '@playwright/test';

export class AudiobookVoiceSamplesPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		throw new Error('not implemented');
	}

	async uploadWavFile(wavPath: string): Promise<void> {
		throw new Error('not implemented');
	}

	async getVoiceSampleNames(): Promise<string[]> {
		throw new Error('not implemented');
	}

	async hasSample(name: string): Promise<boolean> {
		throw new Error('not implemented');
	}

	async deleteSample(name: string): Promise<void> {
		throw new Error('not implemented');
	}

	async deleteFirstSample(): Promise<void> {
		throw new Error('not implemented');
	}

	async getSampleCount(): Promise<number> {
		throw new Error('not implemented');
	}

	async ensureSampleExists(name: string): Promise<void> {
		throw new Error('not implemented');
	}
}
