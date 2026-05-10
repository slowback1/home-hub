import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';
const STUB_SERVER_URL = 'http://localhost:8199';
const MINIMAL_WORKFLOW_JSON = JSON.stringify({
	'1': { inputs: { text: '{{prompt}}' }, class_type: 'CLIPTextEncode' }
});

Before({ tags: '@comfyui' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/comfyui`);
});

After({ tags: '@comfyui' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/comfyui`);
});

// --- Setup steps ---

Given('a ComfyUI workflow exists', async ({ request }) => {
	throw new Error('not implemented');
});

Given('the ComfyUI stub server is set to fail', async ({ request }) => {
	throw new Error('not implemented');
});

// --- Config page steps ---

Given('I am on the ComfyUI config page', async ({ comfyUiConfigPage }) => {
	throw new Error('not implemented');
});

When('I fill in a workflow name and paste a workflow JSON', async ({ comfyUiConfigPage }) => {
	throw new Error('not implemented');
});

When('I submit the Add Workflow form', async ({ comfyUiConfigPage }) => {
	throw new Error('not implemented');
});

When('I delete the workflow', async ({ comfyUiConfigPage }) => {
	throw new Error('not implemented');
});

Then('the new workflow should appear in the workflow list', async ({ comfyUiConfigPage }) => {
	throw new Error('not implemented');
});

Then('the workflow should no longer appear in the list', async ({ comfyUiConfigPage }) => {
	throw new Error('not implemented');
});

// --- Inference page steps ---

Given('I am on the ComfyUI page', async ({ comfyUiPage }) => {
	throw new Error('not implemented');
});

When('I select the workflow from the dropdown', async ({ comfyUiPage }) => {
	throw new Error('not implemented');
});

When('I enter a prompt', async ({ comfyUiPage }) => {
	throw new Error('not implemented');
});

When('I click Generate', async ({ comfyUiPage }) => {
	throw new Error('not implemented');
});

When('I select the workflow and enter a prompt', async ({ comfyUiPage }) => {
	throw new Error('not implemented');
});

Then('I should see a loading state while the image is generating', async ({ comfyUiPage }) => {
	throw new Error('not implemented');
});

Then('I should see the generated image when complete', async ({ comfyUiPage }) => {
	throw new Error('not implemented');
});

Then('I should see an inline error message', async ({ comfyUiPage }) => {
	throw new Error('not implemented');
});

Then('the Generate button should be re-enabled', async ({ comfyUiPage }) => {
	throw new Error('not implemented');
});
