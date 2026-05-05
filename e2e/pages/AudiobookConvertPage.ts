import { Page } from '@playwright/test';

export class AudiobookConvertPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		throw new Error('not implemented');
	}

	async hasVoiceSamples(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async uploadEpubAndSelectVoiceSample(epubPath: string): Promise<void> {
		throw new Error('not implemented');
	}

	async submitForm(): Promise<void> {
		throw new Error('not implemented');
	}

	async getLastJobStatus(): Promise<string> {
		throw new Error('not implemented');
	}

	async waitForJobStatus(status: string, timeoutMs?: number): Promise<void> {
		throw new Error('not implemented');
	}

	async hasDownloadButtonForLastJob(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async submitJobThatWillFail(): Promise<void> {
		throw new Error('not implemented');
	}

	async hasErrorMessageOnLastJob(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async isFormDisabled(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async hasNoVoiceSamplesMessage(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async clickCancelOnLastJob(): Promise<void> {
		throw new Error('not implemented');
	}

	async clickDownloadOnLastJob(): Promise<void> {
		throw new Error('not implemented');
	}

	async clearVoiceSamples(): Promise<void> {
		throw new Error('not implemented');
	}

	async submitJobAndWaitForQueued(epubPath: string): Promise<void> {
		throw new Error('not implemented');
	}

	async submitJobAndWaitForCompleted(epubPath: string): Promise<void> {
		throw new Error('not implemented');
	}
}
