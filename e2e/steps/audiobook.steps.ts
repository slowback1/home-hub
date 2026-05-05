import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';
import * as path from 'path';

const BACKEND_URL = 'http://localhost:5273';
const WAV_FIXTURE = path.join(__dirname, '../fixtures/sample.wav');

Before({ tags: '@audiobook' }, async ({ request }) => {
	await request.patch(`${BACKEND_URL}/api/feature-flags/AUDIOBOOK_ENABLED`, {
		data: { isEnabled: true }
	});
});

After({ tags: '@audiobook' }, async ({ request }) => {
	await request.patch(`${BACKEND_URL}/api/feature-flags/AUDIOBOOK_ENABLED`, {
		data: { isEnabled: false }
	});
});

// --- Convert page steps ---

Given('I am on the audiobook convert page', async ({ audiobookConvertPage }) => {
	await audiobookConvertPage.goto();
});

Given('at least one voice sample exists', async () => {
	throw new Error('not implemented');
});

Given('a voice sample exists', async () => {
	throw new Error('not implemented');
});

Given('a queued job exists', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

Given('a completed job exists', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

Given('no voice samples exist', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

When('I upload an EPUB file and select a voice sample', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

When('I submit the conversion form', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

When('I submit a conversion job that will fail', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

When('I click cancel on the queued job', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

When('I click download on the completed job', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

Then('a new job appears in the queue with status {string}', async ({ audiobookConvertPage }, status: string) => {
	throw new Error('not implemented');
});

Then('the job progresses to {string}', async ({ audiobookConvertPage }, status: string) => {
	throw new Error('not implemented');
});

Then('a download button is visible for the completed job', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

Then('the job status changes to {string}', async ({ audiobookConvertPage }, status: string) => {
	throw new Error('not implemented');
});

Then('the file download is initiated', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

Then('an error message is visible on the failed job row', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

Then('the conversion form is disabled', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

Then('a message directing me to the Voice Samples tab is visible', async ({ audiobookConvertPage }) => {
	throw new Error('not implemented');
});

// --- Voice samples page steps ---

Given('I am on the audiobook voice samples page', async ({ audiobookVoiceSamplesPage }) => {
	await audiobookVoiceSamplesPage.goto();
});

When('I upload a WAV file as a voice sample', async ({ audiobookVoiceSamplesPage }) => {
	throw new Error('not implemented');
});

When('I delete a voice sample', async ({ audiobookVoiceSamplesPage }) => {
	throw new Error('not implemented');
});

Then('the new voice sample appears in the list', async ({ audiobookVoiceSamplesPage }) => {
	throw new Error('not implemented');
});

Then('the voice sample is removed from the list', async ({ audiobookVoiceSamplesPage }) => {
	throw new Error('not implemented');
});
