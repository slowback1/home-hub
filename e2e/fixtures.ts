import { test as base } from 'playwright-bdd';
import { createBdd } from 'playwright-bdd';
import { ExamplePage } from './pages/ExamplePage';
import { DesignSystemPage } from './pages/DesignSystemPage';
import { SystemConfigPage } from './pages/SystemConfigPage';
import { ActivityConfigPage } from './pages/ActivityConfigPage';
import { ActivityPage } from './pages/ActivityPage';
import { FeatureFlagsPage } from './pages/FeatureFlagsPage';
import { WeatherPage } from './pages/WeatherPage';

type Fixtures = {
	examplePage: ExamplePage;
	designSystemPage: DesignSystemPage;
	systemConfigPage: SystemConfigPage;
	activityConfigPage: ActivityConfigPage;
	activityPage: ActivityPage;
	featureFlagsPage: FeatureFlagsPage;
	weatherPage: WeatherPage;
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
	},
	featureFlagsPage: async ({ page }, use) => {
		await use(new FeatureFlagsPage(page));
	},
	weatherPage: async ({ page }, use) => {
		await use(new WeatherPage(page));
	}
});

export const { Given, When, Then, Before, After } = createBdd(test);
export { test };
