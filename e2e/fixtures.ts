import { test as base } from 'playwright-bdd';
import { createBdd } from 'playwright-bdd';
import { ExamplePage } from './pages/ExamplePage';
import { DesignSystemPage } from './pages/DesignSystemPage';
import { SystemConfigPage } from './pages/SystemConfigPage';
import { ActivityConfigPage } from './pages/ActivityConfigPage';
import { ActivityPage } from './pages/ActivityPage';

type Fixtures = {
	examplePage: ExamplePage;
	designSystemPage: DesignSystemPage;
	systemConfigPage: SystemConfigPage;
	activityConfigPage: ActivityConfigPage;
	activityPage: ActivityPage;
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
	},
	activityConfigPage: async ({ page }, use) => {
		await use(new ActivityConfigPage(page));
	},
	activityPage: async ({ page }, use) => {
		await use(new ActivityPage(page));
	}
});

export const { Given, When, Then, Before } = createBdd(test);
export { test };
