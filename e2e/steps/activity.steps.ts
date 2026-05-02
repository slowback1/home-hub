import { expect } from '@playwright/test';
import { Given, When, Then } from '../fixtures';

// --- Config page steps ---

Given('I am on the activity config page', async ({ activityConfigPage }) => {
	throw new Error('not implemented');
});

When(
	'I add a new activity with name {string} and weight {int}',
	async ({ activityConfigPage }, name: string, weight: number) => {
		throw new Error('not implemented');
	}
);

Then(
	'I should see {string} in the activity list with weight {int}',
	async ({ activityConfigPage }, name: string, weight: number) => {
		throw new Error('not implemented');
	}
);

Given(
	'the activity list contains {string} with weight {int}',
	async ({ activityConfigPage }, name: string, weight: number) => {
		throw new Error('not implemented');
	}
);

When(
	'I change the weight of {string} to {int}',
	async ({ activityConfigPage }, name: string, weight: number) => {
		throw new Error('not implemented');
	}
);

Given('the activity list contains {string}', async ({ activityConfigPage }, name: string) => {
	throw new Error('not implemented');
});

When('I delete {string}', async ({ activityConfigPage }, name: string) => {
	throw new Error('not implemented');
});

Then(
	'I should not see {string} in the activity list',
	async ({ activityConfigPage }, name: string) => {
		throw new Error('not implemented');
	}
);

// --- Display page steps ---

Given('there are no activity picks recorded', async ({ activityPage }) => {
	throw new Error('not implemented');
});

When('I navigate to the activity display page', async ({ activityPage }) => {
	throw new Error('not implemented');
});

Then(
	'I should see a placeholder message prompting me to configure activities',
	async ({ activityPage }) => {
		throw new Error('not implemented');
	}
);

Given(
	'an activity pick exists for the current hour with name {string}',
	async ({ activityPage }, name: string) => {
		throw new Error('not implemented');
	}
);

Then(
	'I should see {string} displayed as the current pick',
	async ({ activityPage }, name: string) => {
		throw new Error('not implemented');
	}
);
