import { Page, expect } from '@playwright/test';
import * as path from 'path';

const EPUB_FIXTURE = path.join(__dirname, '../fixtures/sample.epub');
const STATUS_POLL_INTERVAL = 500;

export class AudiobookConvertPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/audiobook');
		// Wait until the voice samples load (inputs become enabled) OR no-samples message appears
		await this.page.waitForFunction(
			() => {
				const input = document.querySelector(
					'[data-testid="epub-file-input"]'
				) as HTMLInputElement | null;
				const msg = document.querySelector('[data-testid="no-voice-samples-message"]');
				return (input && !input.disabled) || !!msg;
			},
			{ timeout: 10_000 }
		);
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
		const countBefore = await this.page.locator('[data-testid="job-row"]').count();
		await this.page.locator('[data-testid="submit-job-button"]').click();
		// Wait for the new job row to appear in the DOM
		await this.page
			.locator('[data-testid="job-row"]')
			.nth(countBefore)
			.waitFor({ state: 'visible', timeout: 10_000 });
	}

	async getLastJobStatus(): Promise<string> {
		const lastRow = this.page.locator('[data-testid="job-row"]').last();
		const badge = lastRow.locator('[data-testid="job-status"]');
		return (await badge.textContent())?.trim() ?? '';
	}

	// Returns true if currentStatus has reached (or passed) targetStatus in the state machine
	private hasReachedStatus(currentStatus: string, targetStatus: string): boolean {
		if (currentStatus === targetStatus) return true;
		// If expected is an intermediate state, accept any terminal state (job passed through it)
		const terminalStates = ['completed', 'failed', 'cancelled'];
		if (targetStatus === 'in_progress' && terminalStates.includes(currentStatus)) return true;
		return false;
	}

	async waitForLastJobStatus(status: string, timeoutMs = 15_000): Promise<void> {
		const expectedText = status.replace('_', ' ');
		const deadline = Date.now() + timeoutMs;

		// Poll backend API until the job has reached (or passed) the expected status
		await this.page.waitForFunction(
			async ([expectedStatus, backendUrl]: [string, string]) => {
				try {
					const res = await fetch(`${backendUrl}/api/audiobook/jobs`, { cache: 'no-store' });
					if (!res.ok) return false;
					const jobs: { status: string }[] = await res.json();
					if (jobs.length === 0) return false;
					const current = jobs[jobs.length - 1].status;
					if (current === expectedStatus) return true;
					// Accept terminal state as proof job passed through in_progress
					if (expectedStatus === 'in_progress' && ['completed', 'failed', 'cancelled'].includes(current))
						return true;
					return false;
				} catch {
					return false;
				}
			},
			[status, 'http://localhost:5273'] as [string, string],
			{ timeout: timeoutMs, polling: 500 }
		);

		// Sync the UI: check current DOM first, reload if it doesn't reflect a sufficient state yet
		while (Date.now() < deadline) {
			const count = await this.page.locator('[data-testid="job-row"]').count();
			if (count > 0) {
				const text = (
					await this.page
						.locator('[data-testid="job-row"]')
						.last()
						.locator('[data-testid="job-status"]')
						.textContent()
				)?.trim() ?? '';
				if (this.hasReachedStatus(text.replace(' ', '_'), status)) return;
			}
			// DOM doesn't reflect the expected status yet — reload to pull fresh data
			await this.goto();
			await this.page.waitForTimeout(200);
		}
		throw new Error(`Timed out waiting for UI to show job status "${status}" after ${timeoutMs}ms`);
	}

	async hasDownloadButtonForLastJob(): Promise<boolean> {
		const lastRow = this.page.locator('[data-testid="job-row"]').last();
		await lastRow.waitFor({ state: 'visible', timeout: 5000 });
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
		await lastRow.waitFor({ state: 'visible', timeout: 5000 });
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
