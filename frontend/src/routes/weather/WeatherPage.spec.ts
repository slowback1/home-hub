import { render, waitFor } from '@testing-library/svelte';
import { vi } from 'vitest';
import WeatherPage from './+page.svelte';
import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';

vi.mock('$lib/api/WeatherApi', () => ({ default: vi.fn() }));

import WeatherApi from '$lib/api/WeatherApi';

const mockConditions = {
	temperature: 72.0,
	conditionLabel: 'Sunny',
	humidityPercent: 45,
	windSpeed: 8.5,
	units: 'imperial'
};

function makeApiMock(result: Promise<unknown>) {
	vi.mocked(WeatherApi).mockImplementation(
		() => ({ getCurrent: vi.fn().mockReturnValue(result) }) as never
	);
}

function enableWeatherFlag() {
	FeatureFlagService.featureFlags = [{ name: 'WEATHER_ENABLED', isEnabled: true }];
}
function disableWeatherFlag() {
	FeatureFlagService.featureFlags = [{ name: 'WEATHER_ENABLED', isEnabled: false }];
}

describe('Weather page', () => {
	beforeEach(() => {
		vi.mocked(WeatherApi).mockClear();
		enableWeatherFlag();
	});

	it('shows a spinner while loading', () => {
		makeApiMock(new Promise(() => {}));
		const { getByRole } = render(WeatherPage);
		expect(getByRole('status')).toBeInTheDocument();
	});

	it('displays temperature, condition, humidity, and wind speed on success', async () => {
		makeApiMock(Promise.resolve(mockConditions));
		const { queryByRole, getByTestId } = render(WeatherPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		expect(getByTestId('weather-temperature').textContent).toContain('72');
		expect(getByTestId('weather-condition').textContent).toContain('Sunny');
		expect(getByTestId('weather-humidity').textContent).toContain('45');
		expect(getByTestId('weather-wind').textContent).toContain('8.5');
	});

	it('displays imperial unit labels', async () => {
		makeApiMock(Promise.resolve(mockConditions));
		const { queryByRole, getByTestId } = render(WeatherPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		expect(getByTestId('weather-temperature').textContent).toContain('°F');
		expect(getByTestId('weather-wind').textContent).toContain('mph');
	});

	it('shows Weather unavailable message on API error', async () => {
		makeApiMock(Promise.reject(new Error('API error')));
		const { queryByRole, getByTestId } = render(WeatherPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		expect(getByTestId('weather-unavailable')).toBeInTheDocument();
	});

	it('shows disabled state when WEATHER_ENABLED flag is off', async () => {
		disableWeatherFlag();
		makeApiMock(Promise.resolve(mockConditions));
		const { getByTestId } = render(WeatherPage);

		expect(getByTestId('weather-disabled')).toBeInTheDocument();
	});
});
