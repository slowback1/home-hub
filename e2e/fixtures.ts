import { test as base } from 'playwright-bdd';
import { createBdd } from 'playwright-bdd';
import { ExamplePage } from './pages/ExamplePage';

type Fixtures = {
	examplePage: ExamplePage;
};

const test = base.extend<Fixtures>({
	examplePage: async ({ page }, use) => {
		await use(new ExamplePage(page));
	}
});

export const { Given, When, Then } = createBdd(test);
export { test };
