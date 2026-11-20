<script lang="ts">
	import { onMount } from 'svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import NbaApi, { type NbaGame } from '$lib/api/NbaApi';
	import { formatStartLocal, visibleGames } from '$lib/services/NbaGames/nbaGameVisibility';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
	import { FeatureFlags } from '$lib/services/FeatureFlag/FeatureFlags';

	const RETENTION_DAYS = 3;
	const MS_PER_DAY = 86_400_000;

	const api = new NbaApi();
	let games: NbaGame[] = [];
	let loading = true;
	let error = false;
	let enabled = true;

	onMount(async () => {
		enabled = FeatureFlagService.featureFlags.some(
			(f) => f.name === FeatureFlags.NBA_SCHEDULE_ENABLED && f.isEnabled
		);

		if (!enabled) {
			loading = false;
			return;
		}

		try {
			const now = new Date();
			const from = new Date(now.getTime() - RETENTION_DAYS * MS_PER_DAY);
			const to = new Date(now.getTime() + RETENTION_DAYS * MS_PER_DAY);
			games = await api.getSchedule(from, to);
		} catch {
			error = true;
		} finally {
			loading = false;
		}
	});

	$: visible = visibleGames(games, new Date());
</script>

<section class="nba-page">
	<header class="nba-page__header">
		<Heading level={1}>NBA Games</Heading>
	</header>

	{#if !enabled}
		<div data-testid="nba-disabled">
			<p>NBA schedule is not available.</p>
		</div>
	{:else if loading}
		<Spinner />
	{:else if error}
		<p class="nba-page__error" role="alert">Failed to load the NBA schedule.</p>
	{:else if visible.length === 0}
		<p class="nba-page__empty">No games</p>
	{:else}
		<ul class="nba-page__list">
			{#each visible as game (game.id)}
				<li class="nba-page__row" data-testid="nba-game-row">
					<span class="nba-page__row-text"
						>{game.homeTeam} @ {game.awayTeam} — {formatStartLocal(game.startAt)}</span
					>
				</li>
			{/each}
		</ul>
	{/if}
</section>

<style>
	.nba-page__header {
		margin-bottom: var(--space-4);
	}

	.nba-page__error {
		color: var(--color-danger, #c0392b);
	}

	.nba-page__empty {
		color: var(--color-text-secondary);
		font-style: italic;
	}

	.nba-page__list {
		list-style: none;
		padding: 0;
		margin: 0;
		display: flex;
		flex-direction: column;
	}

	.nba-page__row {
		padding: var(--space-2) 0;
		border-bottom: 1px solid var(--color-border-subtle);
	}

	.nba-page__row-text {
		font-size: var(--font-size-md);
		color: var(--color-text-primary);
	}
</style>
