import { test as base } from 'playwright-bdd';
import { createBdd } from 'playwright-bdd';
import { ExamplePage } from './pages/ExamplePage';
import { DesignSystemPage } from './pages/DesignSystemPage';
import { SystemConfigPage } from './pages/SystemConfigPage';
import { ActivityConfigPage } from './pages/ActivityConfigPage';
import { ActivityPage } from './pages/ActivityPage';
import { FeatureFlagsPage } from './pages/FeatureFlagsPage';
import { WeatherPage } from './pages/WeatherPage';
import { AudiobookConvertPage } from './pages/AudiobookConvertPage';
import { AudiobookVoiceSamplesPage } from './pages/AudiobookVoiceSamplesPage';
import { ComfyUiPage } from './pages/ComfyUiPage';
import { ComfyUiConfigPage } from './pages/ComfyUiConfigPage';
import { TasksPage } from './pages/TasksPage';
import { BookmarksPage } from './pages/BookmarksPage';
import { DashboardPage } from './pages/DashboardPage';
import { WalkHistoryPage } from './pages/WalkHistoryPage';
import { WheelsPage } from './pages/WheelsPage';

type Fixtures = {
	walkHistoryPage: WalkHistoryPage;
	wheelsPage: WheelsPage;
	examplePage: ExamplePage;
	designSystemPage: DesignSystemPage;
	systemConfigPage: SystemConfigPage;
	activityConfigPage: ActivityConfigPage;
	activityPage: ActivityPage;
	featureFlagsPage: FeatureFlagsPage;
	weatherPage: WeatherPage;
	audiobookConvertPage: AudiobookConvertPage;
	audiobookVoiceSamplesPage: AudiobookVoiceSamplesPage;
	comfyUiPage: ComfyUiPage;
	comfyUiConfigPage: ComfyUiConfigPage;
	tasksPage: TasksPage;
	bookmarksPage: BookmarksPage;
	dashboardPage: DashboardPage;
};

const test = base.extend<Fixtures>({
	walkHistoryPage: async ({ page }, use) => {
		await use(new WalkHistoryPage(page));
	},
	wheelsPage: async ({ page }, use) => {
		await use(new WheelsPage(page));
	},
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
	},
	audiobookConvertPage: async ({ page }, use) => {
		await use(new AudiobookConvertPage(page));
	},
	audiobookVoiceSamplesPage: async ({ page }, use) => {
		await use(new AudiobookVoiceSamplesPage(page));
	},
	comfyUiPage: async ({ page }, use) => {
		await use(new ComfyUiPage(page));
	},
	comfyUiConfigPage: async ({ page }, use) => {
		await use(new ComfyUiConfigPage(page));
	},
	tasksPage: async ({ page }, use) => {
		await use(new TasksPage(page));
	},
	bookmarksPage: async ({ page }, use) => {
		await use(new BookmarksPage(page));
	},
	dashboardPage: async ({ page }, use) => {
		await use(new DashboardPage(page));
	}
});

export const { Given, When, Then, Before, After } = createBdd(test);
export { test };
