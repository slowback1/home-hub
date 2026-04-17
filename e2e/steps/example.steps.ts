import { Given, Then, When } from '../fixtures';

Given('I have a placeholder step', async ({ examplePage }) => {
	await examplePage.visit();
});

When('I run the test', async ({ examplePage }) => {
	await examplePage.doSomething();
});

Then('it should pass successfully', async ({ examplePage }) => {
	await examplePage.assertSomething();
});
