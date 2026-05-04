import { Page } from '@playwright/test';

export class WeatherPage {
	constructor(private readonly page: Page) {}

	async goto(): Promise<void> {
		throw new Error('not implemented');
	}

	async gotoWithApiError(): Promise<void> {
		throw new Error('not implemented');
	}

	async hasTemperatureValue(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async hasConditionDescription(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async hasHumidityValue(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async hasWindSpeedValue(): Promise<boolean> {
		throw new Error('not implemented');
	}

	async hasMessage(text: string): Promise<boolean> {
		throw new Error('not implemented');
	}

	async isNotFoundState(): Promise<boolean> {
		throw new Error('not implemented');
	}
}
