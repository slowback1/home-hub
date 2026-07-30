<script lang="ts">
	import { onMount } from 'svelte';
	import { Disc3 } from 'lucide-svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import Select from '$lib/ui/inputs/Select/Select.svelte';
	import WheelApi, { type Wheel } from '$lib/api/WheelApi';
	import { parseItems, pickRandom } from '$lib/utils/spinWheel';

	const ICON_SM = 22;

	const api = new WheelApi();
	let wheels: Wheel[] = [];
	let loading = true;
	let error = false;

	let selectedWheelId = '';
	let spinResult: string | null = null;

	$: selectOptions = wheels.map((wheel) => ({ label: wheel.name, value: wheel.id }));
	$: selectedWheel = wheels.find((wheel) => wheel.id === selectedWheelId) ?? null;
	$: selectedItems = parseItems(selectedWheel?.items ?? '');
	$: canSpin = selectedItems.length > 0;

	onMount(async () => {
		try {
			wheels = await api.getAll();
			// Always default to the first saved wheel (no selection memory).
			if (wheels.length > 0) selectedWheelId = wheels[0].id;
		} catch {
			error = true;
		} finally {
			loading = false;
		}
	});

	function spin() {
		spinResult = pickRandom(selectedItems);
	}

	function onSelectWheel(value: string) {
		selectedWheelId = value;
		spinResult = null;
	}
</script>

<div class="wheels-widget" data-testid="wheels-widget">
	{#if loading}
		<div class="widget-state" role="status">
			<span class="widget-spinner" aria-hidden="true"></span>
			<span class="widget-state__label">Loading…</span>
		</div>
	{:else if error}
		<div class="widget-state" role="alert">
			<span class="widget-state__dash" aria-hidden="true">—</span>
			<span class="widget-state__label">Unavailable</span>
		</div>
	{:else if wheels.length === 0}
		<div class="widget-state widget-state--empty" data-testid="wheel-widget-empty">
			<span class="widget-empty-icon" aria-hidden="true">
				<Disc3 size={ICON_SM} />
			</span>
			<span class="widget-state__label">No wheels yet — create one to start spinning.</span>
		</div>
	{:else}
		<div class="widget-controls">
			<Select
				data-testid="wheel-widget-select"
				label=""
				id="wheel-widget-select"
				options={selectOptions}
				value={selectedWheelId}
				onChange={onSelectWheel}
			/>
			<Button testId="wheel-widget-spin-btn" variant="primary" disabled={!canSpin} onClick={spin}>
				Spin
			</Button>
		</div>

		<div class="widget-result">
			{#if spinResult}
				<span class="widget-result__value" data-testid="wheel-widget-result">{spinResult}</span>
			{:else if !canSpin}
				<span class="widget-result__hint">This wheel has no items yet.</span>
			{:else}
				<span class="widget-result__hint">Spin to pick an item.</span>
			{/if}
		</div>

		<div class="widget-footer">
			<a href="/wheels" class="widget-footer__link" data-testid="wheel-widget-view-all">
				Manage wheels →
			</a>
		</div>
	{/if}
</div>

<style>
	.wheels-widget {
		height: 100%;
		display: flex;
		flex-direction: column;
	}

	.widget-state {
		flex: 1;
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		gap: var(--space-2);
		color: var(--color-text-secondary);
		padding: var(--space-4);
		text-align: center;
	}

	.widget-state--empty {
		gap: var(--space-3);
	}

	.widget-empty-icon {
		display: flex;
		align-items: center;
		justify-content: center;
		width: 48px;
		height: 48px;
		border-radius: var(--radius-full);
		background: var(--color-surface-overlay);
		color: var(--color-text-disabled);
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
		to {
			transform: rotate(360deg);
		}
	}

	.widget-state__dash {
		font-size: var(--font-size-2xl);
		line-height: 1;
	}

	.widget-state__label {
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
		max-width: 200px;
	}

	.widget-controls {
		display: flex;
		align-items: flex-end;
		gap: var(--space-2);
		padding: var(--space-3);
	}

	.widget-controls :global(.select__group) {
		flex: 1;
	}

	.widget-result {
		flex: 1;
		display: flex;
		align-items: center;
		justify-content: center;
		padding: var(--space-3);
		text-align: center;
	}

	.widget-result__value {
		font-size: var(--font-size-2xl);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
	}

	.widget-result__hint {
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
	}

	.widget-footer {
		padding: var(--space-3);
		border-top: 1px solid var(--color-border-subtle);
	}

	.widget-footer__link {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-brand-lighter);
		text-decoration: none;
	}

	.widget-footer__link:hover {
		text-decoration: underline;
	}
</style>
