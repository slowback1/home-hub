<script lang="ts">
	import { onMount } from 'svelte';
	import ShoppingListApi, { type ShoppingList } from '$lib/api/ShoppingListApi';

	const POLL_MS = 3_600_000;
	const MAX_VISIBLE = 5;

	const api = new ShoppingListApi();
	let lists: ShoppingList[] = [];
	let loading = true;
	let error = false;

	async function fetchData() {
		try {
			lists = await api.listActive();
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

	$: sortedLists = [...lists].sort((a, b) => b.createdAt.localeCompare(a.createdAt));
	$: visibleLists = sortedLists.slice(0, MAX_VISIBLE);
	$: overflow = Math.max(0, sortedLists.length - MAX_VISIBLE);
</script>

<div class="shopping-lists-widget">
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
	{:else if lists.length === 0}
		<div class="widget-state">
			<span class="widget-state__label">No active lists</span>
		</div>
	{:else}
		<ul class="shopping-lists-list">
			{#each visibleLists as list (list.id)}
				<li class="shopping-list-item">
					<a
						href={'/shopping-lists?fullscreen=' + list.id}
						class="shopping-list-link"
						data-testid="shopping-lists-widget-link"
					>
						<span class="shopping-list-name">{list.name}</span>
						<span class="shopping-list-chev" aria-hidden="true">›</span>
					</a>
				</li>
			{/each}
		</ul>
		{#if overflow > 0}
			<div class="shopping-lists-overflow" data-testid="shopping-lists-widget-overflow">
				and {overflow} more…
			</div>
		{/if}
	{/if}
</div>

<style>
	.shopping-lists-widget {
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

	.shopping-lists-list {
		list-style: none;
		padding: 0;
		margin: 0;
		display: flex;
		flex-direction: column;
	}

	.shopping-list-link {
		display: flex;
		align-items: center;
		gap: var(--space-2);
		padding: var(--space-1) 0;
		color: var(--color-text-primary);
		text-decoration: none;
		font-size: var(--font-size-sm);
		border-bottom: 1px solid var(--color-border-subtle);
		transition: color 0.15s ease;
	}

	.shopping-list-link:hover {
		color: var(--color-brand-lighter);
	}

	.shopping-list-link:hover .shopping-list-name {
		text-decoration: underline;
		text-decoration-color: var(--color-brand-lighter);
	}

	.shopping-list-link:hover .shopping-list-chev {
		transform: translateX(2px);
	}

	.shopping-list-name {
		flex: 1;
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}

	.shopping-list-chev {
		color: var(--color-text-disabled);
		font-size: var(--font-size-md);
		flex-shrink: 0;
		transition: transform 0.15s ease;
	}

	.shopping-lists-overflow {
		padding-top: var(--space-2);
		color: var(--color-text-secondary);
		font-size: var(--font-size-xs);
		font-style: italic;
	}
</style>
