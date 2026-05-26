<script lang="ts">
	import { onMount } from 'svelte';
	import ActivityPickApi, { type ActivityPick } from '$lib/api/ActivityPickApi';

	const POLL_MS = 300_000;

	const api = new ActivityPickApi();
	let pick: ActivityPick | null = null;
	let loading = true;
	let error = false;

	function formatDate(iso: string): string {
		return new Date(iso).toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
	}

	async function fetchData() {
		try {
			pick = await api.getCurrent();
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

<div class="activity-widget">
	{#if loading}
		<div class="widget-state" role="status">
			<span class="widget-spinner" aria-hidden="true"></span>
			<span class="widget-state__label">Loading…</span>
		</div>
	{:else if error || !pick}
		<div class="widget-state" role="alert">
			<span class="widget-state__dash" aria-hidden="true">—</span>
			<span class="widget-state__label">Unavailable</span>
		</div>
	{:else}
		<div class="activity-body">
			<span class="activity-name" data-testid="activity-widget-name">{pick.activityName}</span>
			<span class="activity-date">picked {formatDate(pick.pickedAt)}</span>
		</div>
	{/if}
</div>

<style>
	.activity-widget {
		height: 100%;
		display: flex;
		align-items: center;
		justify-content: center;
	}

	.widget-state {
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

	.activity-body {
		display: flex;
		flex-direction: column;
		align-items: center;
		gap: var(--space-2);
		text-align: center;
		padding: var(--space-2);
	}

	.activity-name {
		font-size: var(--font-size-xl);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
		line-height: var(--line-height-tight);
	}

	.activity-date {
		font-size: var(--font-size-xs);
		font-family: var(--font-family-mono, monospace);
		color: var(--color-text-secondary);
	}
</style>
