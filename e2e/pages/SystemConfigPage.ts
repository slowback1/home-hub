import { expect, Page } from '@playwright/test';

export class SystemConfigPage {
	private readonly MASK = '••••••••';

	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/admin/system-config');
		await this.page.waitForSelector('[data-testid^="namespace-"]', { state: 'visible' });
	}

	async hasNamespaceSection(namespace: string): Promise<boolean> {
		return this.page.locator(`[data-testid="namespace-${namespace}"]`).isVisible();
	}

	async getEntryValue(key: string): Promise<string> {
		const span = this.page.locator(`[data-testid="value-${key}"]`);
		await span.waitFor({ state: 'visible' });
		return (await span.textContent()) ?? '';
	}

	async isEntryMasked(key: string): Promise<boolean> {
		const text = await this.getEntryValue(key);
		return text === this.MASK;
	}

	async clickEntryValue(key: string): Promise<void> {
		await this.page.locator(`[data-testid="value-${key}"]`).click();
		await this.page.waitForSelector('role=textbox', { state: 'visible' });
	}

	async isEditModeVisible(key: string): Promise<boolean> {
		const saveBtn = this.page.locator('[data-testid="btn-save"]');
		const cancelBtn = this.page.locator('[data-testid="btn-cancel"]');
		const input = this.page.locator('role=textbox');
		return (await saveBtn.isVisible()) && (await cancelBtn.isVisible()) && (await input.isVisible());
	}

	async typeValueAndSave(value: string): Promise<void> {
		const input = this.page.locator('role=textbox');
		await input.fill(value);
		await this.page.locator('[data-testid="btn-save"]').click();
		await this.page.waitForSelector('role=textbox', { state: 'hidden' });
	}

	async clickEntryValueAndType(key: string, value: string): Promise<void> {
		await this.clickEntryValue(key);
		await this.page.locator('role=textbox').fill(value);
	}

	async clickCancel(): Promise<void> {
		await this.page.locator('[data-testid="btn-cancel"]').click();
		await this.page.waitForSelector('role=textbox', { state: 'hidden' });
	}

	async clickShowToggle(key: string): Promise<void> {
		await this.page.locator(`[data-testid="toggle-${key}"]`).click();
		// Wait until Svelte flushes the reactivity update and the span no longer shows the mask.
		await expect(this.page.locator(`[data-testid="value-${key}"]`)).not.toHaveText(this.MASK);
	}

	async hasSuccessToast(): Promise<boolean> {
		const toast = this.page.locator('[data-testid="toast-item"].toast-item__success');
		await toast.waitFor({ state: 'visible', timeout: 5000 });
		return toast.isVisible();
	}

	async hasErrorToast(): Promise<boolean> {
		const toast = this.page.locator('[data-testid="toast-item"].toast-item__error');
		await toast.waitFor({ state: 'visible', timeout: 5000 });
		return toast.isVisible();
	}

	async isInputOpen(): Promise<boolean> {
		return this.page.locator('role=textbox').isVisible();
	}

	async hasSelectField(label: string, _sectionHeader: string): Promise<boolean> {
		const row = this.page.locator('tr').filter({ has: this.page.locator('td', { hasText: label }) });
		return row.locator('select').isVisible();
	}

	async getSelectOptions(label: string): Promise<string[]> {
		const row = this.page.locator('tr').filter({ has: this.page.locator('td', { hasText: label }) });
		const options = await row.locator('select option').all();
		return Promise.all(options.map((o) => o.textContent().then((t) => t?.trim() ?? '')));
	}

	async selectDropdownOption(label: string, option: string): Promise<void> {
		const row = this.page.locator('tr').filter({ has: this.page.locator('td', { hasText: label }) });
		await row.locator('select').selectOption({ label: option });
	}

	async getSelectValue(label: string): Promise<string> {
		const row = this.page.locator('tr').filter({ has: this.page.locator('td', { hasText: label }) });
		const select = row.locator('select');
		const selectedValue = await select.inputValue();
		const selectedOption = row.locator(`select option[value="${selectedValue}"]`);
		return (await selectedOption.textContent())?.trim() ?? '';
	}

	async hasSectionHeader(header: string): Promise<boolean> {
		return this.page.locator('h2', { hasText: header }).isVisible();
	}

	async hasFieldLabel(label: string, _sectionHeader: string): Promise<boolean> {
		return this.page.locator('td', { hasText: label }).isVisible();
	}

	async clickEntryValueWithErrorOnSave(key: string): Promise<void> {
		await this.page.route('**/api/system-config/**', (route) => {
			if (route.request().method() === 'PUT') {
				route.fulfill({ status: 500, body: 'Internal Server Error' });
			} else {
				route.continue();
			}
		});
		await this.clickEntryValue(key);
		await this.page.locator('[data-testid="btn-save"]').click();
	}
}
