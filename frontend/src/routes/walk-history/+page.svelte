<script lang="ts">
	import { onMount } from 'svelte';
	import { Footprints, Smartphone } from 'lucide-svelte';
	import WalkSessionApi, { type WalkSession } from '$lib/api/WalkSessionApi';
	import { fmtDuration, relTime, weekdayTime } from '$lib/utils/walkFormatters';

	const ICON_LG = 24;
	const ICON_SM = 38;
	const ICON_HINT = 14;

	const api = new WalkSessionApi();
	let sessions: WalkSession[] = [];
	let loading = true;
	let error = false;

	onMount(async () => {
		try {
			sessions = await api.listSessions();
		} catch {
			error = true;
		} finally {
			loading = false;
		}
	});
</script>

<svelte:head>
	<title>Walk History — HomeHub</title>
</svelte:head>

<div class="walk-page" data-testid="walk-history-page">
	<header class="walk-header">
		<div class="walk-title">
			<span class="walk-title-icon" aria-hidden="true">
				<Footprints size={ICON_LG} />
			</span>
			<div>
				<h1>Walk History</h1>
				{#if !loading}
					<p class="walk-sub">
						{sessions.length}
						{sessions.length === 1 ? 'session' : 'sessions'}
					</p>
				{/if}
			</div>
		</div>
	</header>

	{#if loading}
		<div class="page-state" role="status" data-testid="walk-loading">
			<span class="page-spinner" aria-hidden="true"></span>
			<span>Loading…</span>
		</div>
	{:else if error}
		<div class="page-state" role="alert">
			<span>Failed to load walk sessions.</span>
		</div>
	{:else if sessions.length === 0}
		<div class="walk-empty" data-testid="walk-empty-state">
			<span class="walk-empty__icon" aria-hidden="true">
				<Footprints size={ICON_SM} />
			</span>
			<h2>No walks recorded yet</h2>
			<p>
				Start a walk in the HomeHub app on your phone and your sessions will sync and show up here.
			</p>
			<span class="walk-empty__hint">
				<Smartphone size={ICON_HINT} />
				Open SlowWalk on your phone to begin
			</span>
		</div>
	{:else}
		<table class="walk-table" data-testid="walk-sessions-table">
			<thead>
				<tr>
					<th>When</th>
					<th class="num">Steps</th>
					<th class="num">Duration</th>
				</tr>
			</thead>
			<tbody>
				{#each sessions as session (session.id)}
					<tr data-testid="walk-session-row">
						<td>
							<span class="when-primary">{relTime(session.startedAt)}</span>
							<span class="when-sub">{weekdayTime(session.startedAt)}</span>
						</td>
						<td class="num steps" data-testid="walk-session-steps">
							{session.stepCount.toLocaleString('en-US')}
						</td>
						<td class="num dur" data-testid="walk-session-duration">
							{fmtDuration(session.durationSeconds)}
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	{/if}
</div>

<style>
	.walk-page {
		display: flex;
		flex-direction: column;
		gap: var(--space-6);
	}

	/* Header */
	.walk-header {
		display: flex;
		align-items: flex-start;
	}

	.walk-title {
		display: flex;
		align-items: center;
		gap: var(--space-3);
	}

	.walk-title-icon {
		display: flex;
		align-items: center;
		justify-content: center;
		width: 44px;
		height: 44px;
		flex-shrink: 0;
		border-radius: var(--radius-md);
		background: var(--color-brand);
		color: var(--color-brand-lighter);
	}

	.walk-title h1 {
		margin: 0;
		font-size: var(--font-size-2xl);
		font-weight: var(--font-weight-bold);
		line-height: var(--line-height-tight);
		color: var(--color-text-primary);
	}

	.walk-sub {
		margin: 2px 0 0;
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
	}

	/* Loading */
	.page-state {
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		gap: var(--space-3);
		padding: var(--space-12, 4rem) var(--space-4);
		color: var(--color-text-secondary);
		font-size: var(--font-size-sm);
	}

	.page-spinner {
		width: 24px;
		height: 24px;
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

	/* Empty state */
	.walk-empty {
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		gap: var(--space-4);
		text-align: center;
		padding: var(--space-8) var(--space-6);
	}

	.walk-empty__icon {
		display: flex;
		align-items: center;
		justify-content: center;
		width: 84px;
		height: 84px;
		border-radius: var(--radius-full);
		background: var(--color-surface-raised);
		border: 1px solid var(--color-border-subtle);
		color: var(--color-text-disabled);
	}

	.walk-empty h2 {
		margin: 0;
		font-size: var(--font-size-xl);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
	}

	.walk-empty p {
		margin: 0;
		max-width: 360px;
		font-size: var(--font-size-sm);
		line-height: var(--line-height-normal);
		color: var(--color-text-secondary);
	}

	.walk-empty__hint {
		display: inline-flex;
		align-items: center;
		gap: var(--space-2);
		margin-top: var(--space-2);
		font-size: var(--font-size-xs);
		color: var(--color-text-secondary);
		padding: var(--space-2) var(--space-3);
		background: var(--color-surface-raised);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-full);
	}

	/* Table */
	.walk-table {
		width: 100%;
		border-collapse: collapse;
	}

	.walk-table thead th {
		text-align: left;
		padding: 0 var(--space-4) var(--space-3);
		font-size: var(--font-size-xs);
		font-weight: var(--font-weight-semibold);
		letter-spacing: 0.04em;
		text-transform: uppercase;
		color: var(--color-text-secondary);
		border-bottom: 1px solid var(--color-border-subtle);
	}

	.walk-table thead th.num {
		text-align: right;
	}

	.walk-table tbody td {
		padding: var(--space-3) var(--space-4);
		border-bottom: 1px solid var(--color-border-subtle);
		vertical-align: middle;
	}

	.walk-table tbody tr:last-child td {
		border-bottom: none;
	}

	.walk-table tbody tr:hover td {
		background: var(--color-surface-raised);
	}

	.walk-table td.num {
		text-align: right;
		font-variant-numeric: tabular-nums;
	}

	.walk-table td.steps {
		font-size: var(--font-size-md);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
	}

	.walk-table td.dur {
		font-size: var(--font-size-md);
		color: var(--color-text-secondary);
	}

	.when-primary {
		display: block;
		font-size: var(--font-size-md);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
	}

	.when-sub {
		display: block;
		font-size: var(--font-size-xs);
		color: var(--color-text-secondary);
		margin-top: 1px;
	}
</style>
