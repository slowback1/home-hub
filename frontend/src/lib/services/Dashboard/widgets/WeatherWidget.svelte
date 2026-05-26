<script lang="ts">
	import { onMount } from 'svelte';
	import WeatherApi, { type WeatherConditions } from '$lib/api/WeatherApi';

	const POLL_MS = 3_600_000;

	const api = new WeatherApi();
	let conditions: WeatherConditions | null = null;
	let loading = true;
	let error = false;

	const tempUnit = (units: string) => (units === 'metric' ? '°C' : '°F');
	const windUnit = (units: string) => (units === 'metric' ? 'km/h' : 'mph');

	async function fetchData() {
		try {
			conditions = await api.getCurrent();
			error = false;
		} catch {
			error = true;
		}
	}

	onMount(() => {
		fetchData().finally(() => (loading = false));
		const interval = setInterval(fetchData, POLL_MS);
		return () => clearInterval(interval);
	});
</script>

<div class="weather-widget">
	{#if loading}
		<div class="widget-state" role="status">
			<span class="widget-spinner" aria-hidden="true"></span>
			<span class="widget-state__label">Loading…</span>
		</div>
	{:else if error || !conditions}
		<div class="widget-state" data-testid="weather-widget-error" role="alert">
			<span class="widget-state__dash" aria-hidden="true">—</span>
			<span class="widget-state__label">Unavailable</span>
		</div>
	{:else}
		<div class="weather-primary">
			<span class="weather-temp" data-testid="weather-widget-temperature">
				{conditions.temperature}{tempUnit(conditions.units)}
			</span>
			<span class="weather-cond" data-testid="weather-widget-condition">
				{conditions.conditionLabel}
			</span>
		</div>
		<dl class="weather-secondary">
			<div class="weather-stat">
				<dt>Humidity</dt>
				<dd data-testid="weather-widget-humidity">{conditions.humidityPercent}%</dd>
			</div>
			<div class="weather-stat">
				<dt>Wind</dt>
				<dd data-testid="weather-widget-wind">{conditions.windSpeed} {windUnit(conditions.units)}</dd>
			</div>
		</dl>
	{/if}
</div>

<style>
	.weather-widget {
		height: 100%;
		display: flex;
		flex-direction: column;
		justify-content: space-between;
	}

	.widget-state {
		flex: 1;
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		gap: var(--space-2);
		color: var(--color-text-secondary);
	}

	.widget-spinner {
		width: 20px;
		height: 20px;
		border: 2px solid var(--color-border-subtle);
		border-top-color: var(--color-brand-lighter);
		border-radius: 50%;
		animation: spin 0.7s linear infinite;
	}

	@keyframes spin {
		to { transform: rotate(360deg); }
	}

	.widget-state__dash {
		font-size: var(--font-size-2xl);
		line-height: 1;
	}

	.widget-state__label {
		font-size: var(--font-size-sm);
		font-style: italic;
	}

	.weather-primary {
		flex: 1;
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		gap: var(--space-1);
	}

	.weather-temp {
		font-size: var(--font-size-3xl);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
		line-height: 1;
	}

	.weather-cond {
		font-size: var(--font-size-md);
		color: var(--color-text-secondary);
	}

	.weather-secondary {
		display: grid;
		grid-template-columns: 1fr 1fr;
		border-top: 1px solid var(--color-border-subtle);
		padding-top: var(--space-2);
		margin: 0;
	}

	.weather-stat {
		display: flex;
		flex-direction: column;
		gap: 2px;
	}

	.weather-stat dt {
		font-size: var(--font-size-xs);
		color: var(--color-text-disabled);
		text-transform: uppercase;
		letter-spacing: 0.04em;
	}

	.weather-stat dd {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
		margin: 0;
	}
</style>
