import { Page } from '@playwright/test';

export class WeatherPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/weather');
		// Wait until the page settles into one of its three terminal states
		await this.page
			.locator(
				'[data-testid="weather-temperature"], [data-testid="weather-unavailable"], [data-testid="weather-disabled"]'
			)
			.waitFor({ state: 'visible', timeout: 10000 });
	}

	async gotoWithApiError(): Promise<void> {
		await this.page.route('**/api/weather/current', (route) =>
			route.fulfill({ status: 500, body: 'Internal Server Error' })
		);
		await this.page.goto('/weather');
		await this.page
			.locator('[data-testid="weather-unavailable"]')
			.waitFor({ state: 'visible', timeout: 10000 });
	}

	async hasTemperatureValue(): Promise<boolean> {
		return this.page.locator('[data-testid="weather-temperature"]').isVisible();
	}

	async hasConditionDescription(): Promise<boolean> {
		return this.page.locator('[data-testid="weather-condition"]').isVisible();
	}

	async hasHumidityValue(): Promise<boolean> {
		return this.page.locator('[data-testid="weather-humidity"]').isVisible();
	}

	async hasWindSpeedValue(): Promise<boolean> {
		return this.page.locator('[data-testid="weather-wind"]').isVisible();
	}

	async hasMessage(text: string): Promise<boolean> {
		return this.page.locator(`text=${text}`).isVisible();
	}

	async isNotFoundState(): Promise<boolean> {
		return this.page.locator('[data-testid="weather-disabled"]').isVisible();
	}
}
