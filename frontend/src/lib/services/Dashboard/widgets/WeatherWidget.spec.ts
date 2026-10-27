import { render, waitFor } from '@testing-library/svelte';
import { vi } from 'vitest';
import WeatherWidget from './WeatherWidget.svelte';

vi.mock('$lib/api/WeatherApi', () => ({ default: vi.fn() }));

import WeatherApi from '$lib/api/WeatherApi';

const mockConditions = {
	temperature: 73.13,
	conditionLabel: 'Sunny',
	humidityPercent: 45,
	windSpeed: 8.5,
	units: 'imperial'
};

function mockApi(result: Promise<unknown>) {
	vi.mocked(WeatherApi).mockImplementation(
		() => ({ getCurrent: vi.fn().mockReturnValue(result) }) as never
	);
}

describe('WeatherWidget', () => {
	beforeEach(() => {
		vi.mocked(WeatherApi).mockClear();
	});

	it('rounds the temperature to the nearest whole number', async () => {
		mockApi(Promise.resolve(mockConditions));
		const { getByTestId } = render(WeatherWidget);

		await waitFor(() => expect(getByTestId('weather-widget-temperature')).toBeInTheDocument());
		expect(getByTestId('weather-widget-temperature').textContent?.trim()).toBe('73°F');
	});

	it('shows unavailable state on API error', async () => {
		mockApi(Promise.reject(new Error('API error')));
		const { getByTestId } = render(WeatherWidget);

		await waitFor(() => expect(getByTestId('weather-widget-error')).toBeInTheDocument());
	});
});
