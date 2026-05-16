import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';
import * as path from 'path';

const BACKEND_URL = 'http://localhost:5273';
const WAV_FIXTURE = path.join(__dirname, '../fixtures/sample.wav');
const EPUB_FIXTURE = path.join(__dirname, '../fixtures/sample.epub');
const FAIL_EPUB_FIXTURE = path.join(__dirname, '../fixtures/fail.epub');

Before({ tags: '@audiobook' }, async ({ request }) => {
	await request.patch(`${BACKEND_URL}/api/feature-flags/AUDIOBOOK_ENABLED`, {
		data: { isEnabled: true }
	});
	await request.delete(`${BACKEND_URL}/api/test/audiobook`);
});

After({ tags: '@audiobook' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/audiobook`);
	await request.patch(`${BACKEND_URL}/api/feature-flags/AUDIOBOOK_ENABLED`, {
		data: { isEnabled: false }
	});
});

// --- Convert page steps ---

Given('I am on the audiobook convert page', async ({ audiobookConvertPage }) => {
	await audiobookConvertPage.goto();
});

Given('at least one voice sample exists', async () => {
	// MockAudiobookService always pre-seeds "default" — no action needed
});

Given('a voice sample exists', async () => {
	// MockAudiobookService always pre-seeds "default" — no action needed
});

Given('a queued job exists', async ({ audiobookConvertPage }) => {
	await audiobookConvertPage.uploadEpubFile(EPUB_FIXTURE);
	await audiobookConvertPage.selectFirstVoiceSample();
	await audiobookConvertPage.submitForm();
	await audiobookConvertPage.waitForLastJobStatus('queued', 5_000);
});

Given('a completed job exists', async ({ audiobookConvertPage }) => {
	await audiobookConvertPage.uploadEpubFile(EPUB_FIXTURE);
	await audiobookConvertPage.selectFirstVoiceSample();
	await audiobookConvertPage.submitForm();
	await audiobookConvertPage.waitForLastJobStatus('completed', 15_000);
});

Given('no voice samples exist', async ({ request, audiobookConvertPage }) => {
	// Reset clears all samples including "default"
	await request.delete(`${BACKEND_URL}/api/test/audiobook`);
	// Now delete the pre-seeded default via the API (reset re-seeds it, so we need to delete it)
	await request.delete(`${BACKEND_URL}/api/audiobook/voice-samples/default`);
	await audiobookConvertPage.goto();
});

When('I upload an EPUB file and select a voice sample', async ({ audiobookConvertPage }) => {
	await audiobookConvertPage.uploadEpubFile(EPUB_FIXTURE);
	await audiobookConvertPage.selectFirstVoiceSample();
});

When('I submit the conversion form', async ({ audiobookConvertPage }) => {
	await audiobookConvertPage.submitForm();
});

When('I submit a conversion job that will fail', async ({ audiobookConvertPage }) => {
	// "fail" in the filename triggers the failed state in MockAudiobookService
	await audiobookConvertPage.uploadEpubFile(FAIL_EPUB_FIXTURE);
	await audiobookConvertPage.selectFirstVoiceSample();
	await audiobookConvertPage.submitForm();
});

When('I click cancel on the queued job', async ({ audiobookConvertPage }) => {
	await audiobookConvertPage.clickCancelOnLastJob();
});

When('I click download on the completed job', async () => {
	// Download click and event capture are handled together in the Then step
});

Then('a new job appears in the queue with status {string}', async ({ audiobookConvertPage }, status: string) => {
	await audiobookConvertPage.waitForLastJobStatus(status, 5_000);
});

Then('the job progresses to {string}', async ({ audiobookConvertPage }, status: string) => {
	await audiobookConvertPage.waitForLastJobStatus(status, 15_000);
});

Then('a download button is visible for the completed job', async ({ audiobookConvertPage }) => {
	expect(await audiobookConvertPage.hasDownloadButtonForLastJob()).toBe(true);
});

Then('the job status changes to {string}', async ({ audiobookConvertPage }, status: string) => {
	await audiobookConvertPage.waitForLastJobStatus(status, 10_000);
});

Then('the file download is initiated', async ({ page, audiobookConvertPage }) => {
	// Verify clicking the download button causes a successful GET request to the file endpoint
	const [response] = await Promise.all([
		page.waitForResponse((r) => r.url().includes('/file') && r.request().method() === 'GET', {
			timeout: 10_000
		}),
		audiobookConvertPage.clickDownloadOnLastJob()
	]);
	expect(response.status()).toBe(200);
});

Then('an error message is visible on the failed job row', async ({ audiobookConvertPage }) => {
	expect(await audiobookConvertPage.hasErrorMessageOnLastJob()).toBe(true);
});

Then('the conversion form is disabled', async ({ audiobookConvertPage }) => {
	expect(await audiobookConvertPage.isFormDisabled()).toBe(true);
});

Then('a message directing me to the Voice Samples tab is visible', async ({ audiobookConvertPage }) => {
	expect(await audiobookConvertPage.hasNoVoiceSamplesMessage()).toBe(true);
});

Given('a failed job exists', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

Given('a cancelled job exists', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

When('I click delete on the completed job', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

When('I click delete on the failed job', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

When('I click delete on the cancelled job', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

Then('the job is removed from the queue list', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

// --- Voice samples page steps ---

Given('I am on the audiobook voice samples page', async ({ audiobookVoiceSamplesPage }) => {
	await audiobookVoiceSamplesPage.goto();
});

When('I upload a WAV file as a voice sample', async ({ audiobookVoiceSamplesPage }) => {
	await audiobookVoiceSamplesPage.uploadWavFile(WAV_FIXTURE);
});

When('I delete a voice sample', async ({ audiobookVoiceSamplesPage }) => {
	await audiobookVoiceSamplesPage.deleteFirstSample();
});

Then('the new voice sample appears in the list', async ({ audiobookVoiceSamplesPage }) => {
	const names = await audiobookVoiceSamplesPage.getVoiceSampleNames();
	// After upload there should be at least 2 samples (default + uploaded)
	expect(names.length).toBeGreaterThan(1);
});

Then('the voice sample is removed from the list', async ({ audiobookVoiceSamplesPage }) => {
	const count = await audiobookVoiceSamplesPage.getSampleCount();
	expect(count).toBe(0);
});
