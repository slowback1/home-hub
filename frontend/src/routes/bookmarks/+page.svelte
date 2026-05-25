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

	// Modal state
	let modalOpen = false;
	let editingBookmark: BookmarkItem | null = null;
	let formUrl = '';
	let formName = '';
	let formDescription = '';
	let formUrlError: string | null = null;
	let formSubmitting = false;

	// Search and filter state
	let searchQuery = '';
	let starredOnly = false;

	$: sorted = [...bookmarks].sort((a, b) => a.name.localeCompare(b.name));
	$: starredCount = bookmarks.filter((b) => b.starred).length;
	$: filtered = sorted.filter((bm) => {
		if (starredOnly && !bm.starred) return false;
		return !searchQuery.trim() || matchesSearch(bm, searchQuery);
	});

	function matchesSearch(bm: BookmarkItem, query: string): boolean {
		const q = query.toLowerCase();
		return (
			bm.name.toLowerCase().includes(q) ||
			bm.url.toLowerCase().includes(q) ||
			(bm.description?.toLowerCase().includes(q) ?? false)
		);
	}

	function clearFilters() {
		searchQuery = '';
		starredOnly = false;
	}

	$: derivedHost = (() => {
		try {
			return new URL(normalizeUrl(formUrl)).hostname.replace(/^www\./, '');
		} catch {
			return '';
		}
	})();

	function displayHost(url: string): string {
		try {
			return new URL(url).hostname.replace(/^www\./, '');
		} catch {
			return url;
		}
	}

	function normalizeUrl(raw: string): string {
		const trimmed = raw.trim();
		if (!trimmed) return '';
		return /^[a-z][a-z0-9+.-]*:\/\//i.test(trimmed) ? trimmed : `https://${trimmed}`;
	}

	function openAddModal() {
		editingBookmark = null;
		formUrl = '';
		formName = '';
		formDescription = '';
		formUrlError = null;
		modalOpen = true;
	}

	function openEditModal(bm: BookmarkItem) {
		editingBookmark = bm;
		formUrl = bm.url;
		formName = bm.name;
		formDescription = bm.description ?? '';
		formUrlError = null;
		modalOpen = true;
	}

	function closeModal() {
		modalOpen = false;
		editingBookmark = null;
		formUrlError = null;
	}

	const TEXTAREA_ROWS = 3;
	const UNDO_TIMEOUT_MS = 5000;

	// Delete confirmation state
	let confirmOpen = false;
	let deletingBookmark: BookmarkItem | null = null;

	// Undo toast state
	let undoBookmark: BookmarkItem | null = null;
	let undoTimer: ReturnType<typeof setTimeout> | null = null;

	function openDeleteConfirm(bm: BookmarkItem) {
		deletingBookmark = bm;
		confirmOpen = true;
	}

	function closeDeleteConfirm() {
		confirmOpen = false;
		deletingBookmark = null;
	}

	function clearUndoToast() {
		undoBookmark = null;
		if (undoTimer) {
			clearTimeout(undoTimer);
			undoTimer = null;
		}
	}

	async function confirmDelete() {
		if (!deletingBookmark) return;
		const snapshot = { ...deletingBookmark };
		bookmarks = bookmarks.filter((b) => b.id !== snapshot.id);
		closeDeleteConfirm();
		try {
			await api.deleteBookmark(snapshot.id);
			undoBookmark = snapshot;
			undoTimer = setTimeout(clearUndoToast, UNDO_TIMEOUT_MS);
		} catch {
			bookmarks = [...bookmarks, snapshot];
		}
	}

	async function handleStar(bm: BookmarkItem) {
		try {
			const updated = await api.toggleStar(bm.id);
			bookmarks = bookmarks.map((b) => (b.id === updated.id ? updated : b));
		} catch {
			// star toggle failed silently
		}
	}

	async function handleUndo() {
		if (!undoBookmark) return;
		const snapshot = undoBookmark;
		clearUndoToast();
		try {
			const restored = await api.createBookmark({
				url: snapshot.url,
				name: snapshot.name,
				description: snapshot.description
			});
			bookmarks = [...bookmarks, restored];
		} catch {
			// undo failed silently
		}
	}

	function handleModalKeydown(e: KeyboardEvent) {
		if (e.key === 'Escape') closeModal();
	}

	function handleConfirmKeydown(e: KeyboardEvent) {
		if (e.key === 'Escape') closeDeleteConfirm();
	}

	function validateBookmarkUrl(normalized: string): string | null {
		if (!normalized) return 'URL is required.';
		try {
			new URL(normalized);
			return null;
		} catch {
			return 'Not a valid URL.';
		}
	}

	async function saveBookmark(normalized: string, finalName: string): Promise<void> {
		const desc = formDescription.trim() || null;
		if (editingBookmark) {
			const updated = await api.updateBookmark(editingBookmark.id, {
				url: normalized,
				name: finalName,
				description: desc
			});
			bookmarks = bookmarks.map((b) => (b.id === updated.id ? updated : b));
		} else {
			const created = await api.createBookmark({
				url: normalized,
				name: finalName,
				description: desc
			});
			bookmarks = [...bookmarks, created];
		}
		closeModal();
	}

	async function handleSubmit() {
		const normalized = normalizeUrl(formUrl);
		const urlError = validateBookmarkUrl(normalized);
		if (urlError) {
			formUrlError = urlError;
			return;
		}
		formUrlError = null;
		formSubmitting = true;
		try {
			await saveBookmark(normalized, formName.trim() || derivedHost);
		} catch {
			formUrlError = 'Something went wrong. Please try again.';
		} finally {
			formSubmitting = false;
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
		<button class="btn btn--primary" data-testid="add-bookmark-button" on:click={openAddModal}
			>Add Bookmark</button
		>
	</div>

	{#if loadError}
		<p class="error" data-testid="load-error">{loadError}</p>
	{/if}

	{#if loading}
		<p class="loading" data-testid="loading">Loading…</p>
	{:else if bookmarks.length === 0}
		<div class="empty-state-zero" data-testid="bookmarks-empty-state">
			<div class="empty-icon"><Bookmark size={ICON_EMPTY} /></div>
			<h2>No bookmarks yet</h2>
			<p>
				Save links to anything — selfhosted services, docs, repos. They show up here as a grid you
				can search.
			</p>
			<button
				class="btn btn--primary"
				data-testid="empty-add-bookmark-button"
				on:click={openAddModal}
			>
				Add Bookmark
			</button>
		</div>
	{:else}
		<div class="toolbar">
			<input
				type="search"
				class="search-input"
				bind:value={searchQuery}
				placeholder="Search bookmarks…"
				data-testid="bookmark-search"
			/>
			<button
				class="starred-filter"
				class:active={starredOnly}
				data-testid="starred-filter"
				on:click={() => (starredOnly = !starredOnly)}
			>
				<Star size={ICON_STAR} fill={starredOnly ? 'currentColor' : 'none'} />
				Starred
				{#if starredCount > 0}<span class="starred-count">{starredCount}</span>{/if}
			</button>
		</div>

		<p class="results-summary" data-testid="results-summary">
			<strong>{filtered.length}</strong> of <strong>{bookmarks.length}</strong>
			{bookmarks.length === 1 ? 'bookmark' : 'bookmarks'}
		</p>

		{#if filtered.length === 0}
			<div class="empty-state-filtered" data-testid="bookmarks-filtered-empty-state">
				<p>No bookmarks match your filters.</p>
				<button class="btn btn--secondary" on:click={clearFilters}>Clear filters</button>
			</div>
		{:else}
			<div class="bookmark-grid" data-testid="bookmark-grid">
				{#each filtered as bm (bm.id)}
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
								on:click|preventDefault|stopPropagation={() => handleStar(bm)}
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
								on:click|preventDefault={() => openEditModal(bm)}
							>
								<Pencil size={ICON_ACTION} />
							</button>
							<button
								class="icon-btn icon-btn--danger"
								aria-label="Delete {bm.name}"
								data-testid="delete-bookmark-button"
								on:click|preventDefault={() => openDeleteConfirm(bm)}
							>
								<Trash2 size={ICON_ACTION} />
							</button>
						</div>
					</a>
				{/each}
			</div>
		{/if}
	{/if}
</div>

{#if modalOpen}
	<div
		class="modal-backdrop"
		data-testid="bookmark-modal-backdrop"
		on:click|self={closeModal}
		on:keydown={handleModalKeydown}
		role="presentation"
	>
		<div class="modal" role="dialog" aria-modal="true" data-testid="bookmark-modal">
			<div class="modal-header">
				<h2 class="modal-title">{editingBookmark ? 'Edit Bookmark' : 'Add Bookmark'}</h2>
				<button
					class="modal-close"
					aria-label="Close"
					data-testid="modal-close-button"
					on:click={closeModal}>✕</button
				>
			</div>

			<form class="modal-form" on:submit|preventDefault={handleSubmit} data-testid="bookmark-form">
				<div class="form-field">
					<label for="bm-url">URL</label>
					<input
						id="bm-url"
						type="text"
						bind:value={formUrl}
						placeholder="https://example.com"
						data-testid="bookmark-url-input"
						autocomplete="off"
						spellcheck="false"
					/>
					{#if formUrlError}
						<p class="field-error" data-testid="url-error">{formUrlError}</p>
					{/if}
				</div>

				<div class="form-field">
					<label for="bm-name">
						Name <span class="optional">(optional)</span>
					</label>
					<input
						id="bm-name"
						type="text"
						bind:value={formName}
						placeholder={derivedHost ? `Defaults to "${derivedHost}"` : 'Defaults to hostname'}
						data-testid="bookmark-name-input"
					/>
				</div>

				<div class="form-field">
					<label for="bm-description">
						Description <span class="optional">(optional)</span>
					</label>
					<textarea
						id="bm-description"
						bind:value={formDescription}
						rows={TEXTAREA_ROWS}
						placeholder="One-line reminder of what this is for."
						data-testid="bookmark-description-input"
					></textarea>
				</div>

				<div class="modal-actions">
					<button
						type="button"
						class="btn btn--secondary"
						data-testid="cancel-bookmark-button"
						on:click={closeModal}
					>
						Cancel
					</button>
					<button
						type="submit"
						class="btn btn--primary"
						data-testid="submit-bookmark-button"
						disabled={formSubmitting}
					>
						{#if formSubmitting}Saving…{:else}{editingBookmark ? 'Save' : 'Add Bookmark'}{/if}
					</button>
				</div>
			</form>
		</div>
	</div>
{/if}

{#if confirmOpen && deletingBookmark}
	<div
		class="modal-backdrop"
		data-testid="delete-confirm-backdrop"
		on:click|self={closeDeleteConfirm}
		on:keydown={handleConfirmKeydown}
		role="presentation"
	>
		<div
			class="modal modal--compact"
			role="dialog"
			aria-modal="true"
			data-testid="delete-confirm-dialog"
		>
			<div class="modal-header">
				<h2 class="modal-title">Delete Bookmark?</h2>
			</div>
			<p class="confirm-text">
				Delete <strong>{deletingBookmark.name}</strong>
				({displayHost(deletingBookmark.url)})?
			</p>
			<div class="modal-actions">
				<button
					class="btn btn--secondary"
					data-testid="cancel-delete-button"
					on:click={closeDeleteConfirm}
				>
					Cancel
				</button>
				<button
					class="btn btn--danger"
					data-testid="confirm-delete-button"
					on:click={confirmDelete}
				>
					Delete
				</button>
			</div>
		</div>
	</div>
{/if}

{#if undoBookmark}
	<div class="undo-toast" data-testid="undo-toast" role="status">
		<span>Deleted "{undoBookmark.name}".</span>
		<button class="undo-btn" data-testid="undo-button" on:click={handleUndo}>Undo</button>
	</div>
{/if}

<style>
	.bookmarks-page {
		display: flex;
		flex-direction: column;
		gap: var(--space-6);
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

	/* Toolbar */
	.toolbar {
		display: flex;
		align-items: center;
		gap: var(--space-3);
	}

	.search-input {
		flex: 1;
		padding: var(--space-2) var(--space-3);
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-sm);
		background-color: var(--color-surface-raised);
		color: var(--color-text-primary);
		font-size: var(--font-size-sm);
		font-family: inherit;
	}

	.search-input:focus {
		outline: none;
		border-color: var(--color-brand-lighter);
	}

	.starred-filter {
		display: flex;
		align-items: center;
		gap: var(--space-2);
		padding: var(--space-2) var(--space-3);
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-full, 9999px);
		background: var(--color-surface-raised);
		color: var(--color-text-secondary);
		font-size: var(--font-size-sm);
		cursor: pointer;
		white-space: nowrap;
		transition:
			background 0.15s ease,
			color 0.15s ease,
			border-color 0.15s ease;
	}

	.starred-filter:hover {
		background: var(--color-surface-overlay);
		color: var(--color-text-primary);
	}

	.starred-filter.active {
		background: var(--color-surface-overlay);
		color: #e9b84a;
		border-color: #e9b84a;
	}

	.starred-count {
		background: var(--color-surface-deep);
		color: var(--color-text-secondary);
		font-size: var(--font-size-xs);
		padding: 0 var(--space-1);
		border-radius: var(--radius-sm);
		min-width: 1.2em;
		text-align: center;
	}

	/* Filtered empty state */
	.empty-state-filtered {
		display: flex;
		flex-direction: column;
		align-items: center;
		gap: var(--space-4);
		padding: var(--space-8, 3rem) var(--space-6);
		color: var(--color-text-secondary);
		text-align: center;
		font-size: var(--font-size-sm);
	}

	.empty-state-filtered p {
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

	.btn--secondary {
		background: var(--color-surface-overlay);
		color: var(--color-text-primary);
	}

	.btn--danger {
		background: var(--color-error, #e53e3e);
		color: white;
	}

	.btn:disabled {
		opacity: 0.5;
		cursor: not-allowed;
	}

	/* Modal */
	.modal-backdrop {
		position: fixed;
		inset: 0;
		background: rgba(0, 0, 0, 0.5);
		display: flex;
		align-items: center;
		justify-content: center;
		z-index: 100;
	}

	.modal {
		background: var(--color-surface-base);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md);
		padding: var(--space-6);
		width: 100%;
		max-width: 480px;
		display: flex;
		flex-direction: column;
		gap: var(--space-4);
	}

	.modal-header {
		display: flex;
		align-items: center;
		justify-content: space-between;
	}

	.modal-title {
		font-size: var(--font-size-lg);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
		margin: 0;
	}

	.modal-close {
		background: none;
		border: none;
		font-size: var(--font-size-md);
		color: var(--color-text-secondary);
		cursor: pointer;
		padding: var(--space-1);
		line-height: 1;
	}

	.modal-close:hover {
		color: var(--color-text-primary);
	}

	.modal-form {
		display: flex;
		flex-direction: column;
		gap: var(--space-4);
	}

	.form-field {
		display: flex;
		flex-direction: column;
		gap: var(--space-1);
	}

	.form-field label {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-text-secondary);
	}

	.optional {
		font-weight: normal;
		color: var(--color-text-secondary);
	}

	.form-field input,
	.form-field textarea {
		padding: var(--space-2) var(--space-3);
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-sm);
		background-color: var(--color-surface-raised);
		color: var(--color-text-primary);
		font-size: var(--font-size-sm);
		font-family: inherit;
		resize: vertical;
	}

	.form-field input:focus,
	.form-field textarea:focus {
		outline: none;
		border-color: var(--color-brand-lighter);
	}

	.field-error {
		font-size: var(--font-size-xs);
		color: var(--color-error, #e53e3e);
		margin: 0;
	}

	.modal-actions {
		display: flex;
		align-items: center;
		justify-content: flex-end;
		gap: var(--space-3);
		padding-top: var(--space-2);
	}

	.modal--compact {
		max-width: 360px;
	}

	.confirm-text {
		font-size: var(--font-size-sm);
		color: var(--color-text-primary);
		margin: 0;
	}

	/* Undo toast */
	.undo-toast {
		position: fixed;
		bottom: var(--space-6);
		left: 50%;
		transform: translateX(-50%);
		display: flex;
		align-items: center;
		gap: var(--space-4);
		padding: var(--space-3) var(--space-6);
		background: var(--color-surface-overlay);
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-md);
		box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
		z-index: 200;
		font-size: var(--font-size-sm);
		color: var(--color-text-primary);
		white-space: nowrap;
	}

	.undo-btn {
		background: none;
		border: none;
		color: var(--color-brand-lighter);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		cursor: pointer;
		padding: 0;
		text-decoration: underline;
	}

	.undo-btn:hover {
		opacity: 0.8;
	}
</style>
