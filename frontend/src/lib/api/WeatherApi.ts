import BaseApi from './baseApi';

export type WeatherConditions = {
	temperature: number;
	conditionLabel: string;
	humidityPercent: number;
	windSpeed: number;
	units: string;
};

export default class WeatherApi extends BaseApi {
	constructor() {
		super();
	}

	async getCurrent(): Promise<WeatherConditions> {
		return this.Get<WeatherConditions>('/api/weather/current');
	}
}
