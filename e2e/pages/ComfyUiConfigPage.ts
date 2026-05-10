import { Page } from '@playwright/test';

export class ComfyUiConfigPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		throw new Error('not implemented');
	}

	async fillWorkflowForm(name: string, json: string): Promise<void> {
		throw new Error('not implemented');
	}

	async submitAddWorkflowForm(): Promise<void> {
		throw new Error('not implemented');
	}

	async deleteFirstWorkflow(): Promise<void> {
		throw new Error('not implemented');
	}

	async getWorkflowNames(): Promise<string[]> {
		throw new Error('not implemented');
	}

	async getWorkflowCount(): Promise<number> {
		throw new Error('not implemented');
	}
}
