import { expect } from '@playwright/test';
import { When, Then } from '../fixtures';

Then(
	'the {string} field in the {string} section should render as a dropdown',
	async ({ systemConfigPage }, label: string, section: string) => {
		const visible = await systemConfigPage.hasSelectField(label, section);
		expect(visible).toBe(true);
	}
);

Then(
	'the dropdown should contain {string} and {string} as options',
	async ({ systemConfigPage }, option1: string, option2: string) => {
		const options = await systemConfigPage.getSelectOptions('Provider');
		expect(options).toContain(option1);
		expect(options).toContain(option2);
	}
);

When(
	'I change the {string} dropdown to {string}',
	async ({ systemConfigPage }, label: string, option: string) => {
		await systemConfigPage.selectDropdownOption(label, option);
	}
);

Then(
	'the {string} field should display {string}',
	async ({ systemConfigPage }, label: string, value: string) => {
		const displayed = await systemConfigPage.getSelectValue(label);
		expect(displayed).toBe(value);
	}
);

Then('I should see a {string} section header', async ({ systemConfigPage }, header: string) => {
	const visible = await systemConfigPage.hasSectionHeader(header);
	expect(visible).toBe(true);
});

Then(
	'I should see a {string} label in the {string} section',
	async ({ systemConfigPage }, label: string, section: string) => {
		const visible = await systemConfigPage.hasFieldLabel(label, section);
		expect(visible).toBe(true);
	}
);
