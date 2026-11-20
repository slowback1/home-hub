<script lang="ts">
	import { onMount } from 'svelte';
	import NbaApi, { type NbaGame } from '$lib/api/NbaApi';
	import { formatStartLocal, visibleGames } from '$lib/services/NbaGames/nbaGameVisibility';

	const api = new NbaApi();
	let games: NbaGame[] = [];
	let loading = true;
	let error = false;

	function localTodayRange(): [Date, Date] {
		const now = new Date();
		const from = new Date(now.getFullYear(), now.getMonth(), now.getDate());
		const to = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1);
		return [from, to];
	}

	async function fetchData() {
		try {
			const [from, to] = localTodayRange();
			games = await api.getSchedule(from, to);
			error = false;
		} catch {
			error = true;
		}
	}

	onMount(() => {
		fetchData().finally(() => (loading = false));
	});

	$: visible = visibleGames(games, new Date(), { todayOnly: true });
</script>

<div class="nba-games-widget">
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
	{:else if visible.length === 0}
		<div class="widget-state">
			<span class="widget-state__label">No games today</span>
		</div>
	{:else}
		<div class="nba-games-scroll" data-testid="nba-games-scroll">
			<ul class="nba-games-list">
				{#each visible as game (game.id)}
					<li class="nba-game-row" data-testid="nba-game-row">
						<span class="nba-game-text"
							>{game.homeTeam} @ {game.awayTeam} — {formatStartLocal(game.startAt)}</span
						>
					</li>
				{/each}
			</ul>
		</div>
	{/if}
</div>

<style>
	.nba-games-widget {
		height: 100%;
		display: flex;
		flex-direction: column;
		justify-content: flex-start;
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
		font-style: italic;
	}

	.nba-games-scroll {
		flex: 1;
		min-height: 0;
		overflow-y: auto;
	}

	.nba-games-list {
		list-style: none;
		padding: 0;
		margin: 0;
		display: flex;
		flex-direction: column;
	}

	.nba-game-row {
		padding: var(--space-1) 0;
		border-bottom: 1px solid var(--color-border-subtle);
	}

	.nba-game-text {
		display: block;
		font-size: var(--font-size-sm);
		color: var(--color-text-primary);
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}
</style>
