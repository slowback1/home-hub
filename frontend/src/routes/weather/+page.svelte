<script lang="ts">
	import { onMount } from 'svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import WeatherApi, { type WeatherConditions } from '$lib/api/WeatherApi';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';

	const api = new WeatherApi();

	let loading = true;
	let conditions: WeatherConditions | null = null;
	let error = false;
	let enabled = true;

	const tempLabel = (units: string) => (units === 'metric' ? '°C' : '°F');
	const windLabel = (units: string) => (units === 'metric' ? 'km/h' : 'mph');

	onMount(async () => {
		enabled = FeatureFlagService.featureFlags.some(
			(f) => f.name === 'WEATHER_ENABLED' && f.isEnabled
		);

		if (!enabled) {
			loading = false;
			return;
		}

		try {
			conditions = await api.getCurrent();
		} catch {
			error = true;
		} finally {
			loading = false;
		}
	});
</script>

<svelte:head>
	<title>HomeHub — Weather</title>
</svelte:head>

{#if !enabled}
	<div data-testid="weather-disabled">
		<p>Weather is not available.</p>
	</div>
{:else if loading}
	<Spinner />
{:else if error || !conditions}
	<div data-testid="weather-unavailable">
		<Heading level={1}>Weather</Heading>
		<p>Weather unavailable</p>
	</div>
{:else}
	<Heading level={1}>Weather</Heading>
	<div class="weather-grid">
		<div class="weather-stat" data-testid="weather-temperature">
			{conditions.temperature}{tempLabel(conditions.units)}
		</div>
		<div class="weather-stat" data-testid="weather-condition">
			{conditions.conditionLabel}
		</div>
		<div class="weather-stat" data-testid="weather-humidity">
			{conditions.humidityPercent}% humidity
		</div>
		<div class="weather-stat" data-testid="weather-wind">
			{conditions.windSpeed}
			{windLabel(conditions.units)} wind
		</div>
	</div>
{/if}

<style>
	.weather-grid {
		display: grid;
		grid-template-columns: 1fr 1fr;
		gap: var(--space-4);
		max-width: 400px;
		margin-top: var(--space-4);
	}

	.weather-stat {
		padding: var(--space-4);
		background: var(--color-surface-raised);
		border-radius: var(--radius-md, 8px);
		font-size: var(--font-size-lg);
		color: var(--color-text-primary);
	}
</style>
