import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';
const STUB_SERVER_URL = 'http://localhost:8199';
const MINIMAL_WORKFLOW_JSON = JSON.stringify({
	'1': { inputs: { text: '{{prompt}}' }, class_type: 'CLIPTextEncode' }
});
const WORKFLOW_NAME = 'E2E Test Workflow';

Before({ tags: '@comfyui' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/comfyui`);
});

After({ tags: '@comfyui' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/comfyui`);
});

// --- Setup steps ---

Given('a ComfyUI workflow exists', async ({ request }) => {
	await request.post(`${BACKEND_URL}/api/comfyui/workflows`, {
		data: { name: WORKFLOW_NAME, workflowJson: MINIMAL_WORKFLOW_JSON }
	});
});

Given('the ComfyUI stub server is set to fail', async ({ request }) => {
	await request.post(`${STUB_SERVER_URL}/stub/fail`);
});

// --- Config page steps ---

Given('I am on the ComfyUI config page', async ({ comfyUiConfigPage }) => {
	await comfyUiConfigPage.goto();
});

When('I fill in a workflow name and paste a workflow JSON', async ({ comfyUiConfigPage }) => {
	await comfyUiConfigPage.fillWorkflowForm('New Workflow', MINIMAL_WORKFLOW_JSON);
});

When('I submit the Add Workflow form', async ({ comfyUiConfigPage }) => {
	await comfyUiConfigPage.submitAddWorkflowForm();
});

When('I delete the workflow', async ({ comfyUiConfigPage }) => {
	await comfyUiConfigPage.deleteFirstWorkflow();
});

Then('the new workflow should appear in the workflow list', async ({ comfyUiConfigPage }) => {
	await comfyUiConfigPage.waitForWorkflow('New Workflow');
	const names = await comfyUiConfigPage.getWorkflowNames();
	expect(names).toContain('New Workflow');
});

Then('the workflow should no longer appear in the list', async ({ comfyUiConfigPage }) => {
	const count = await comfyUiConfigPage.getWorkflowCount();
	expect(count).toBe(0);
});

// --- Inference page steps ---

Given('I am on the ComfyUI page', async ({ comfyUiPage }) => {
	await comfyUiPage.goto();
});

When('I select the workflow from the dropdown', async ({ comfyUiPage }) => {
	await comfyUiPage.selectFirstWorkflow();
});

When('I enter a prompt', async ({ comfyUiPage }) => {
	await comfyUiPage.enterPrompt('a beautiful landscape');
});

When('I click Generate', async ({ comfyUiPage }) => {
	await comfyUiPage.clickGenerate();
});

When('I select the workflow and enter a prompt', async ({ comfyUiPage }) => {
	await comfyUiPage.selectFirstWorkflow();
	await comfyUiPage.enterPrompt('a beautiful landscape');
});

Then('I should see a loading state while the image is generating', async ({ comfyUiPage }) => {
	// The loading indicator is visible immediately after clicking generate
	// and disappears when the image arrives. We assert the image is present which
	// implies the loading state was shown during generation.
	await comfyUiPage.waitForImage(15000);
});

Then('I should see the generated image when complete', async ({ comfyUiPage }) => {
	const src = await comfyUiPage.getResultImageSrc();
	expect(src).toMatch(/^data:image\//);
});

Then('I should see an inline error message', async ({ comfyUiPage }) => {
	await comfyUiPage.waitForError(15000);
	expect(await comfyUiPage.isErrorVisible()).toBe(true);
});

Then('the Generate button should be re-enabled', async ({ comfyUiPage }) => {
	expect(await comfyUiPage.isGenerateButtonEnabled()).toBe(true);
});
