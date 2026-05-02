<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { SvelteMap, SvelteDate } from 'svelte/reactivity';
	import { Settings } from 'lucide-svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import ActivityPickApi, { type ActivityPick } from '$lib/api/ActivityPickApi';

	const POLL_INTERVAL_MS = 60_000;
	const HISTORY_DAYS = 7;
	const HOURS_PER_DAY = 24;
	const ICON_SIZE = 18;
	const TRUNCATE_LEN = 14;

	const api = new ActivityPickApi();

	let loading = true;
	let currentPick: ActivityPick | null = null;
	let pollTimer: ReturnType<typeof setInterval> | null = null;
	let grid: { date: Date; hour: number; pick: ActivityPick | null }[][] = [];

	onMount(async () => {
		await loadAll();
		pollTimer = setInterval(refreshCurrent, POLL_INTERVAL_MS);
	});

	onDestroy(() => {
		if (pollTimer !== null) clearInterval(pollTimer);
	});

	async function loadAll() {
		try {
			const [pick, hist] = await Promise.all([fetchCurrent(), api.getHistory()]);
			currentPick = pick;
			grid = buildGrid(hist);
		} finally {
			loading = false;
		}
	}

	async function refreshCurrent() {
		currentPick = await fetchCurrent();
	}

	async function fetchCurrent(): Promise<ActivityPick | null> {
		try {
			return await api.getCurrent();
		} catch {
			return null;
		}
	}

	function buildGrid(picks: ActivityPick[]) {
		const pickMap = new SvelteMap<string, ActivityPick>();
		for (const pick of picks) {
			const local = new SvelteDate(pick.pickedAt);
			const key = gridKey(local.getFullYear(), local.getMonth(), local.getDate(), local.getHours());
			pickMap.set(key, pick);
		}

		const now = new SvelteDate();
		return Array.from({ length: HISTORY_DAYS }, (_, dayOffset) => {
			const date = new SvelteDate(now);
			date.setDate(now.getDate() - (HISTORY_DAYS - 1) + dayOffset);
			date.setHours(0, 0, 0, 0);
			return Array.from({ length: HOURS_PER_DAY }, (__, hour) => {
				const key = gridKey(date.getFullYear(), date.getMonth(), date.getDate(), hour);
				return { date, hour, pick: pickMap.get(key) ?? null };
			});
		});
	}

	function gridKey(year: number, month: number, day: number, hour: number) {
		return `${year}-${month}-${day}-${hour}`;
	}

	function formatPickedAt(isoString: string): string {
		return new Date(isoString).toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' });
	}

	function formatDayHeader(date: Date): string {
		return date.toLocaleDateString([], { weekday: 'short', month: 'short', day: 'numeric' });
	}

	function isFutureCell(date: Date, hour: number): boolean {
		const now = new SvelteDate();
		const cell = new SvelteDate(date);
		cell.setHours(hour, 0, 0, 0);
		return cell > now;
	}

	function isCurrentHour(date: Date, hour: number): boolean {
		const now = new Date();
		return (
			date.getDate() === now.getDate() &&
			date.getMonth() === now.getMonth() &&
			date.getFullYear() === now.getFullYear() &&
			hour === now.getHours()
		);
	}

	function truncate(text: string, max: number): string {
		return text.length > max ? text.slice(0, max - 1) + '…' : text;
	}
</script>

<svelte:head>
	<title>HomeHub — Activity Picker</title>
</svelte:head>

<div class="page-header">
	<Heading level={1}>Activity Picker</Heading>
	<a
		href="/activity/config"
		class="settings-link"
		aria-label="Configure activities"
		data-testid="settings-link"
	>
		<Settings size={ICON_SIZE} />
	</a>
</div>

{#if loading}
	<Spinner />
{:else}
	<div class="hero" data-testid="hero">
		{#if currentPick}
			<p class="current-activity" data-testid="current-activity">{currentPick.activityName}</p>
			<p class="picked-at">picked at {formatPickedAt(currentPick.pickedAt)}</p>
		{:else}
			<p class="placeholder" data-testid="placeholder">
				Add some activities to get started — <a href="/activity/config">configure here</a>
			</p>
		{/if}
	</div>

	{#if grid.length > 0}
		<div class="grid-wrapper">
			<table class="week-grid" data-testid="week-grid">
				<thead>
					<tr>
						<th class="hour-col"></th>
						{#each grid as col (col[0].date.toDateString())}
							<th class="day-col">{formatDayHeader(col[0].date)}</th>
						{/each}
					</tr>
				</thead>
				<tbody>
					{#each Array.from({ length: HOURS_PER_DAY }, (_, i) => i) as hour (hour)}
						<tr>
							<td class="hour-label">{hour}:00</td>
							{#each grid as col (col[0].date.toDateString())}
								{@const cell = col[hour]}
								<td
									class="grid-cell"
									class:grid-cell--future={isFutureCell(cell.date, cell.hour)}
									class:grid-cell--current={isCurrentHour(cell.date, cell.hour)}
									title={cell.pick?.activityName ?? ''}
								>
									{#if cell.pick}
										<span class="cell-label">{truncate(cell.pick.activityName, TRUNCATE_LEN)}</span>
									{/if}
								</td>
							{/each}
						</tr>
					{/each}
				</tbody>
			</table>
		</div>
	{/if}
{/if}

<style>
	.page-header {
		display: flex;
		align-items: center;
		gap: var(--space-3);
		margin-bottom: var(--space-4);
	}

	.page-header :global(h1) {
		margin: 0;
	}

	.settings-link {
		color: var(--color-text-secondary);
		display: flex;
		align-items: center;
	}

	.settings-link:hover {
		color: var(--color-brand);
	}

	.hero {
		padding: var(--space-8) var(--space-4);
		text-align: center;
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md, 8px);
		margin-bottom: var(--space-6);
	}

	.current-activity {
		font-size: var(--font-size-3xl, 2rem);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
		margin: 0 0 var(--space-2);
	}

	.picked-at {
		color: var(--color-text-secondary);
		margin: 0;
	}

	.placeholder {
		color: var(--color-text-secondary);
		font-size: var(--font-size-lg);
	}

	.grid-wrapper {
		overflow-x: auto;
	}

	.week-grid {
		width: 100%;
		border-collapse: collapse;
		font-size: var(--font-size-xs, 0.75rem);
	}

	.week-grid th {
		padding: var(--space-1) var(--space-2);
		text-align: center;
		color: var(--color-text-secondary);
		font-weight: var(--font-weight-semibold);
		border-bottom: 1px solid var(--color-border-subtle);
		white-space: nowrap;
	}

	.hour-col {
		width: 3rem;
	}

	.hour-label {
		padding: var(--space-1) var(--space-2);
		color: var(--color-text-secondary);
		text-align: right;
		font-size: var(--font-size-xs, 0.75rem);
		white-space: nowrap;
	}

	.grid-cell {
		padding: var(--space-1) var(--space-2);
		border: 1px solid var(--color-border-subtle);
		min-width: 6rem;
		height: 1.75rem;
		vertical-align: middle;
		overflow: hidden;
	}

	.grid-cell--future {
		background: var(--color-surface-tertiary, #f0f0f0);
		opacity: 0.4;
	}

	.grid-cell--current {
		background: var(--color-brand-lightest, #eff6ff);
		border-color: var(--color-brand);
	}

	.cell-label {
		display: block;
		overflow: hidden;
		text-overflow: ellipsis;
		white-space: nowrap;
		color: var(--color-text-primary);
	}
</style>
