import { expect } from '@playwright/test';
import { When, Then } from '../fixtures';

Then(
	'the {string} field in the {string} section should render as a dropdown',
	async ({ systemConfigPage }, label: string, section: string) => {
		throw new Error('not implemented');
	}
);

Then(
	'the dropdown should contain {string} and {string} as options',
	async ({ systemConfigPage }, option1: string, option2: string) => {
		throw new Error('not implemented');
	}
);

When(
	'I change the {string} dropdown to {string}',
	async ({ systemConfigPage }, label: string, option: string) => {
		throw new Error('not implemented');
	}
);

Then(
	'the {string} field should display {string}',
	async ({ systemConfigPage }, label: string, value: string) => {
		throw new Error('not implemented');
	}
);

Then('I should see a {string} section header', async ({ systemConfigPage }, header: string) => {
	throw new Error('not implemented');
});

Then(
	'I should see a {string} label in the {string} section',
	async ({ systemConfigPage }, label: string, section: string) => {
		throw new Error('not implemented');
	}
);
