<script lang="ts">
	import { onMount } from 'svelte';
	import { Bookmark, Pencil, Trash2, Star, ExternalLink } from 'lucide-svelte';
	import BookmarksApi, { type Bookmark as BookmarkItem } from '$lib/api/BookmarksApi';
	import Favicon from '$lib/ui/media/Favicon.svelte';

	const api = new BookmarksApi();

	const ICON_EMPTY = 32;
	const ICON_FAVICON = 40;
	const ICON_HOST = 11;
	const ICON_STAR = 16;
	const ICON_ACTION = 14;

	let bookmarks: BookmarkItem[] = [];
	let loading = true;
	let loadError: string | null = null;

	$: sorted = [...bookmarks].sort((a, b) => a.name.localeCompare(b.name));

	function displayHost(url: string): string {
		try {
			return new URL(url).hostname.replace(/^www\./, '');
		} catch {
			return url;
		}
	}

	onMount(async () => {
		try {
			bookmarks = await api.listBookmarks();
		} catch {
			loadError = 'Failed to load bookmarks. Please refresh the page.';
		} finally {
			loading = false;
		}
	});
</script>

<svelte:head>
	<title>HomeHub — Bookmarks</title>
</svelte:head>

<div class="bookmarks-page" data-testid="bookmarks-page">
	<div class="page-header">
		<h1 class="page-title">Bookmarks</h1>
		<button class="btn btn--primary" data-testid="add-bookmark-button"> Add Bookmark </button>
	</div>

	{#if loadError}
		<p class="error" data-testid="load-error">{loadError}</p>
	{/if}

	{#if loading}
		<p class="loading" data-testid="loading">Loading…</p>
	{:else if sorted.length === 0}
		<div class="empty-state-zero" data-testid="bookmarks-empty-state">
			<div class="empty-icon"><Bookmark size={ICON_EMPTY} /></div>
			<h2>No bookmarks yet</h2>
			<p>
				Save links to anything — selfhosted services, docs, repos. They show up here as a grid you
				can search.
			</p>
			<button class="btn btn--primary" data-testid="empty-add-bookmark-button">
				Add Bookmark
			</button>
		</div>
	{:else}
		<p class="results-summary" data-testid="results-summary">
			<strong>{sorted.length}</strong>
			{sorted.length === 1 ? 'bookmark' : 'bookmarks'}
		</p>

		<div class="bookmark-grid" data-testid="bookmark-grid">
			{#each sorted as bm (bm.id)}
				<a
					class="bookmark-card"
					href={bm.url}
					target="_blank"
					rel="noopener noreferrer"
					data-testid="bookmark-card"
					data-bookmark-name={bm.name}
					title={bm.url}
				>
					<div class="card-top">
						<Favicon url={bm.url} name={bm.name} size={ICON_FAVICON} />
						<div class="title-block">
							<span class="bm-name">{bm.name}</span>
							<span class="bm-host">
								{displayHost(bm.url)}
								<ExternalLink size={ICON_HOST} />
							</span>
						</div>
						<button
							class="star-btn"
							class:starred={bm.starred}
							aria-label={bm.starred ? 'Unstar' : 'Star'}
							data-testid="star-button"
							on:click|preventDefault|stopPropagation={() => {}}
						>
							<Star size={ICON_STAR} fill={bm.starred ? 'currentColor' : 'none'} />
						</button>
					</div>

					{#if bm.description}
						<p class="bm-description">{bm.description}</p>
					{/if}

					<div class="card-actions" on:click|stopPropagation role="presentation">
						<button
							class="icon-btn"
							aria-label="Edit {bm.name}"
							data-testid="edit-bookmark-button"
							on:click|preventDefault={() => {}}
						>
							<Pencil size={ICON_ACTION} />
						</button>
						<button
							class="icon-btn icon-btn--danger"
							aria-label="Delete {bm.name}"
							data-testid="delete-bookmark-button"
							on:click|preventDefault={() => {}}
						>
							<Trash2 size={ICON_ACTION} />
						</button>
					</div>
				</a>
			{/each}
		</div>
	{/if}
</div>

<style>
	.bookmarks-page {
		display: flex;
		flex-direction: column;
		gap: var(--space-5);
	}

	.page-header {
		display: flex;
		align-items: center;
		justify-content: space-between;
	}

	.page-title {
		font-size: var(--font-size-xl);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
		margin: 0;
	}

	/* Results summary */
	.results-summary {
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
		margin: 0;
	}

	/* Grid */
	.bookmark-grid {
		display: grid;
		grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
		gap: var(--space-4);
	}

	/* Card */
	.bookmark-card {
		display: flex;
		flex-direction: column;
		gap: var(--space-3);
		padding: var(--space-4);
		background: var(--color-surface-raised);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md);
		text-decoration: none;
		color: inherit;
		transition:
			border-color 0.15s ease,
			background 0.15s ease;
		position: relative;
	}

	.bookmark-card:hover {
		border-color: var(--color-border-default);
		background: var(--color-surface-overlay);
	}

	.card-top {
		display: flex;
		align-items: center;
		gap: var(--space-3);
	}

	.title-block {
		flex: 1;
		min-width: 0;
		display: flex;
		flex-direction: column;
		gap: 2px;
	}

	.bm-name {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}

	.bm-host {
		display: flex;
		align-items: center;
		gap: 3px;
		font-size: var(--font-size-xs);
		color: var(--color-text-secondary);
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}

	.bm-description {
		font-size: var(--font-size-xs);
		color: var(--color-text-secondary);
		margin: 0;
		display: -webkit-box;
		-webkit-line-clamp: 2;
		-webkit-box-orient: vertical;
		overflow: hidden;
	}

	/* Star button */
	.star-btn {
		background: none;
		border: none;
		padding: var(--space-1);
		cursor: pointer;
		color: var(--color-text-secondary);
		border-radius: var(--radius-sm);
		display: flex;
		align-items: center;
		justify-content: center;
		flex-shrink: 0;
		transition: color 0.15s ease;
	}

	.star-btn:hover {
		color: var(--color-text-primary);
	}

	.star-btn.starred {
		color: #e9b84a;
	}

	/* Card actions */
	.card-actions {
		display: flex;
		gap: var(--space-2);
	}

	.icon-btn {
		background: none;
		border: none;
		padding: var(--space-1) var(--space-2);
		cursor: pointer;
		color: var(--color-text-secondary);
		border-radius: var(--radius-sm);
		display: flex;
		align-items: center;
		transition:
			color 0.15s ease,
			background 0.15s ease;
		font-size: var(--font-size-xs);
	}

	.icon-btn:hover {
		background: var(--color-surface-deep);
		color: var(--color-text-primary);
	}

	.icon-btn--danger:hover {
		background: var(--color-error-surface, #fed7d7);
		color: var(--color-error, #e53e3e);
	}

	/* Empty state */
	.empty-state-zero {
		display: flex;
		flex-direction: column;
		align-items: center;
		text-align: center;
		gap: var(--space-4);
		padding: var(--space-10, 5rem) var(--space-6);
		color: var(--color-text-secondary);
	}

	.empty-icon {
		color: var(--color-text-disabled, var(--color-text-secondary));
		opacity: 0.5;
	}

	.empty-state-zero h2 {
		font-size: var(--font-size-lg);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
		margin: 0;
	}

	.empty-state-zero p {
		font-size: var(--font-size-sm);
		max-width: 380px;
		margin: 0;
		line-height: 1.6;
	}

	.loading,
	.error {
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
	}

	.error {
		color: var(--color-error, #e53e3e);
	}

	/* Button */
	.btn {
		padding: var(--space-2) var(--space-4);
		border-radius: var(--radius-sm);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		cursor: pointer;
		border: 1px solid transparent;
		transition: opacity 0.15s ease;
	}

	.btn:hover:not(:disabled) {
		opacity: 0.85;
	}

	.btn--primary {
		background: var(--color-brand-lighter);
		color: white;
	}
</style>
