import { Page } from '@playwright/test';

export class ComfyUiConfigPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/comfyui/config');
		await this.page.waitForSelector('[data-testid="workflow-table"]', { state: 'visible' });
	}

	async fillWorkflowForm(name: string, json: string): Promise<void> {
		await this.page.fill('#workflow-name', name);
		await this.page.fill('[data-testid="workflow-json-input"]', json);
	}

	async submitAddWorkflowForm(): Promise<void> {
		await this.page.click('[data-testid="add-workflow-btn"]');
	}

	async deleteFirstWorkflow(): Promise<void> {
		const deleteBtn = this.page.locator('[data-testid^="delete-btn-"]').first();
		const label = await deleteBtn.getAttribute('aria-label');
		const name = label?.replace('Delete ', '') ?? '';
		await deleteBtn.click();
		if (name) {
			await this.page
				.locator(`[data-testid="workflow-row-${name}"]`)
				.waitFor({ state: 'detached' });
		}
	}

	async getWorkflowNames(): Promise<string[]> {
		const rows = this.page.locator('[data-testid^="workflow-row-"]');
		const count = await rows.count();
		const names: string[] = [];
		for (let i = 0; i < count; i++) {
			const testId = await rows.nth(i).getAttribute('data-testid');
			if (testId) names.push(testId.replace('workflow-row-', ''));
		}
		return names;
	}

	async getWorkflowCount(): Promise<number> {
		return this.page.locator('[data-testid^="workflow-row-"]').count();
	}

	async waitForWorkflow(name: string, timeout = 5000): Promise<void> {
		await this.page
			.locator(`[data-testid="workflow-row-${name}"]`)
			.waitFor({ state: 'visible', timeout });
	}
}
