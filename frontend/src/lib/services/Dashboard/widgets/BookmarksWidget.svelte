<script lang="ts">
	import { onMount } from 'svelte';
	import BookmarksApi, { type Bookmark } from '$lib/api/BookmarksApi';

	const POLL_MS = 3_600_000;
	const MAX_BOOKMARKS = 5;

	const api = new BookmarksApi();
	let bookmarks: Bookmark[] = [];
	let loading = true;
	let error = false;

	function selectBookmarks(all: Bookmark[]): Bookmark[] {
		const starred = all.filter((b) => b.starred);
		if (starred.length > 0) return starred.slice(0, MAX_BOOKMARKS);
		return [...all].sort((a, b) => b.createdAt.localeCompare(a.createdAt)).slice(0, MAX_BOOKMARKS);
	}

	async function fetchData() {
		try {
			const all = await api.listBookmarks();
			bookmarks = selectBookmarks(all);
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

<div class="bookmarks-widget">
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
	{:else if bookmarks.length === 0}
		<div class="widget-state">
			<span class="widget-state__label">No bookmarks yet</span>
		</div>
	{:else}
		<ul class="bookmarks-list">
			{#each bookmarks as bookmark (bookmark.id)}
				<li class="bookmark-item">
					<a
						href={bookmark.url}
						class="bookmark-link"
						data-testid="bookmarks-widget-link"
						target="_blank"
						rel="noopener noreferrer"
					>
						<span class="bookmark-star" aria-hidden="true">★</span>
						<span class="bookmark-name">{bookmark.name}</span>
						<span class="bookmark-chev" aria-hidden="true">›</span>
					</a>
				</li>
			{/each}
		</ul>
	{/if}
</div>

<style>
	.bookmarks-widget {
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

	.bookmarks-list {
		list-style: none;
		padding: 0;
		margin: 0;
		display: flex;
		flex-direction: column;
	}

	.bookmark-link {
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

	.bookmark-link:hover {
		color: var(--color-brand-lighter);
	}

	.bookmark-link:hover .bookmark-star {
		color: var(--color-warning);
	}

	.bookmark-link:hover .bookmark-name {
		text-decoration: underline;
		text-decoration-color: var(--color-brand-lighter);
	}

	.bookmark-link:hover .bookmark-chev {
		transform: translateX(2px);
	}

	.bookmark-star {
		color: var(--color-text-disabled);
		font-size: var(--font-size-xs);
		flex-shrink: 0;
		transition: color 0.15s ease;
	}

	.bookmark-name {
		flex: 1;
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}

	.bookmark-chev {
		color: var(--color-text-disabled);
		font-size: var(--font-size-md);
		flex-shrink: 0;
		transition: transform 0.15s ease;
	}
</style>
