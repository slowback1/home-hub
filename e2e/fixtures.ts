import { test as base } from 'playwright-bdd';
import { createBdd } from 'playwright-bdd';
import { ExamplePage } from './pages/ExamplePage';
import { DesignSystemPage } from './pages/DesignSystemPage';

type Fixtures = {
	examplePage: ExamplePage;
	designSystemPage: DesignSystemPage;
};

const test = base.extend<Fixtures>({
	examplePage: async ({ page }, use) => {
		await use(new ExamplePage(page));
	},
	designSystemPage: async ({ page }, use) => {
		await use(new DesignSystemPage(page));
	}
});

export const { Given, When, Then } = createBdd(test);
export { test };
