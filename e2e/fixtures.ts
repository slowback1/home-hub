import { test as base } from 'playwright-bdd';
import { createBdd } from 'playwright-bdd';
import { ExamplePage } from './pages/ExamplePage';
import { DesignSystemPage } from './pages/DesignSystemPage';
import { SystemConfigPage } from './pages/SystemConfigPage';

type Fixtures = {
	examplePage: ExamplePage;
	designSystemPage: DesignSystemPage;
	systemConfigPage: SystemConfigPage;
};

const test = base.extend<Fixtures>({
	examplePage: async ({ page }, use) => {
		await use(new ExamplePage(page));
	},
	designSystemPage: async ({ page }, use) => {
		await use(new DesignSystemPage(page));
	},
	systemConfigPage: async ({ page }, use) => {
		await use(new SystemConfigPage(page));
	}
});

export const { Given, When, Then } = createBdd(test);
export { test };
