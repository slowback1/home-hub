<script lang="ts">
	import { onMount } from 'svelte';
	import { Footprints } from 'lucide-svelte';
	import WalkSessionApi, { type WalkSession } from '$lib/api/WalkSessionApi';
	import { fmtDuration, relTime, weekdayTime } from '$lib/utils/walkFormatters';

	const POLL_MS = 3_600_000;
	const MAX_ROWS = 3;
	const ICON_SM = 22;

	const api = new WalkSessionApi();
	let sessions: WalkSession[] = [];
	let loading = true;
	let error = false;

	$: recent = sessions.slice(0, MAX_ROWS);

	async function fetchData() {
		try {
			sessions = await api.listSessions();
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

<div class="walk-widget">
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
	{:else if sessions.length === 0}
		<div class="widget-state widget-state--empty" data-testid="walk-widget-empty">
			<span class="widget-empty-icon" aria-hidden="true">
				<Footprints size={ICON_SM} />
			</span>
			<span class="widget-state__label">No walks yet — start one on your phone to see it here.</span
			>
		</div>
	{:else}
		<ul class="walk-rows" data-testid="walk-widget-rows">
			{#each recent as session (session.id)}
				<li class="walk-row" data-testid="walk-widget-row">
					<div class="walk-when">
						<div class="walk-when__primary">{relTime(session.startedAt)}</div>
						<div class="walk-when__sub">{weekdayTime(session.startedAt)}</div>
					</div>
					<div class="walk-figs">
						<div class="walk-fig">
							<span class="walk-fig__value" data-testid="walk-widget-steps"
								>{session.stepCount.toLocaleString('en-US')}</span
							>
							<span class="walk-fig__unit">steps</span>
						</div>
						<div class="walk-fig">
							<span class="walk-fig__value" data-testid="walk-widget-duration"
								>{fmtDuration(session.durationSeconds)}</span
							>
							<span class="walk-fig__unit">time</span>
						</div>
					</div>
				</li>
			{/each}
		</ul>
		<div class="walk-footer">
			<a href="/walk-history" class="walk-footer__link" data-testid="walk-widget-view-all">
				View all walks →
			</a>
		</div>
	{/if}
</div>

<style>
	.walk-widget {
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

	.walk-rows {
		list-style: none;
		padding: 0;
		margin: 0;
		display: flex;
		flex-direction: column;
		flex: 1;
	}

	.walk-row {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: var(--space-3);
		padding: var(--space-3);
		border-radius: var(--radius-sm);
		transition: background var(--duration-fast, 0.15s) ease;
	}

	.walk-row:hover {
		background: var(--color-surface-overlay);
	}

	.walk-row + .walk-row {
		border-top: 1px solid var(--color-border-subtle);
	}

	.walk-when {
		flex: 1;
		min-width: 0;
	}

	.walk-when__primary {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}

	.walk-when__sub {
		font-size: var(--font-size-xs);
		color: var(--color-text-secondary);
		margin-top: 1px;
	}

	.walk-figs {
		display: flex;
		align-items: center;
		gap: var(--space-4);
		flex-shrink: 0;
	}

	.walk-fig {
		display: flex;
		flex-direction: column;
		align-items: flex-end;
	}

	.walk-fig__value {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
		font-variant-numeric: tabular-nums;
	}

	.walk-fig__unit {
		font-size: 10px;
		letter-spacing: 0.03em;
		text-transform: uppercase;
		color: var(--color-text-secondary);
	}

	.walk-footer {
		padding: var(--space-3) var(--space-3);
		border-top: 1px solid var(--color-border-subtle);
	}

	.walk-footer__link {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-brand-lighter);
		text-decoration: none;
	}

	.walk-footer__link:hover {
		text-decoration: underline;
	}
</style>
