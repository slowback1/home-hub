import { defineConfig } from '@playwright/test';
import { defineBddConfig } from 'playwright-bdd';

const testDir = defineBddConfig({
	features: 'features/*.feature',
	steps: ['steps/**/*.ts', 'fixtures.ts'],
	outputDir: './tests'
});

export default defineConfig({
	testDir,
	globalSetup: './global-setup.ts',
	globalTeardown: './global-teardown.ts',
	use: {
		baseURL: 'http://localhost:5173'
	},
	projects: [
		{
			name: 'chromium',
			use: { browserName: 'chromium' }
		}
	],
	webServer: [
		{
			command: 'dotnet run --project backend/WebAPI',
			cwd: '..',
			url: 'http://localhost:5272/HealthCheck',
			reuseExistingServer: !process.env.CI,
			timeout: 120000
		},
		{
			command: 'npm run dev',
			cwd: '../frontend',
			url: 'http://localhost:5173',
			reuseExistingServer: !process.env.CI,
			timeout: 60000
		}
	]
});
