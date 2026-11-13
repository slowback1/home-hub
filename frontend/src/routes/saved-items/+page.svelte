<script lang="ts">
	import { onMount } from 'svelte';
	import { Bookmark } from 'lucide-svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import TextBox from '$lib/ui/inputs/TextBox/TextBox.svelte';
	import ToastService, { ToastVariant } from '$lib/ui/containers/toast/ToastService';
	import SavedItemsApi, { type SavedItem } from '$lib/api/SavedItemsApi';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';

	const ICON_LG = 24;

	const api = new SavedItemsApi();
	const toast = new ToastService();

	let loading = true;
	let error = false;
	let enabled = true;
	let items: SavedItem[] = [];
	let newItemName = '';
	let editingId: string | null = null;
	let editDraft = '';

	onMount(async () => {
		enabled = FeatureFlagService.featureFlags.some(
			(f) => f.name === 'SAVED_ITEMS_ENABLED' && f.isEnabled
		);

		if (!enabled) {
			loading = false;
			return;
		}

		try {
			items = await api.list();
		} catch {
			error = true;
		} finally {
			loading = false;
		}
	});

	async function createItem() {
		const name = newItemName.trim();
		if (!name) return;

		try {
			const created = await api.create(name);
			items = [...items, created];
			newItemName = '';
			toast.AddToast({ message: 'Saved item created.', variant: ToastVariant.success });
		} catch {
			toast.AddToast({ message: 'Failed to create saved item.', variant: ToastVariant.error });
		}
	}

	function startEdit(item: SavedItem) {
		editingId = item.id;
		editDraft = item.name;
	}

	function cancelEdit() {
		editingId = null;
		editDraft = '';
	}

	async function saveEdit(item: SavedItem) {
		const name = editDraft.trim();
		if (!name) return;

		try {
			const updated = await api.update(item.id, name);
			items = items.map((i) => (i.id === item.id ? updated : i));
			editingId = null;
			editDraft = '';
			toast.AddToast({ message: 'Saved item updated.', variant: ToastVariant.success });
		} catch {
			toast.AddToast({ message: 'Failed to update saved item.', variant: ToastVariant.error });
		}
	}
</script>

<svelte:head>
	<title>Saved Items — HomeHub</title>
</svelte:head>

<div class="saved-items-page" data-testid="saved-items-page">
	<header class="saved-items-header">
		<span class="saved-items-header__icon" aria-hidden="true">
			<Bookmark size={ICON_LG} />
		</span>
		<Heading level={1}>Saved Items</Heading>
	</header>

	{#if !enabled}
		<div data-testid="saved-items-disabled">
			<p>Saved items are not available.</p>
		</div>
	{:else if loading}
		<Spinner />
	{:else if error}
		<p class="saved-items-error" role="alert">Failed to load saved items.</p>
	{:else}
		{#if items.length === 0}
			<p class="saved-items-empty" data-testid="saved-items-empty-state">
				No saved items yet — create your first one below.
			</p>
		{:else}
			<ul class="saved-item-list" data-testid="saved-items-list">
				{#each items as item (item.id)}
					<li class="saved-item-card" data-testid="saved-item-row-{item.name}">
						{#if editingId === item.id}
							<div class="saved-item-edit">
								<TextBox
									label="Name"
									id="saved-item-edit-input-{item.name}"
									bind:value={editDraft}
									placeholder="Item name"
								/>
								<div class="form-actions">
									<Button
										testId="saved-item-save-{item.name}"
										variant="primary"
										onClick={() => saveEdit(item)}
									>
										Save
									</Button>
									<Button
										testId="saved-item-cancel-{item.name}"
										variant="secondary"
										onClick={cancelEdit}
									>
										Cancel
									</Button>
								</div>
							</div>
						{:else}
							<div class="saved-item-card__header">
								<h2 class="saved-item-name">{item.name}</h2>
								<Button
									testId="saved-item-edit-{item.name}"
									variant="secondary"
									onClick={() => startEdit(item)}
								>
									Edit
								</Button>
							</div>
						{/if}
					</li>
				{/each}
			</ul>
		{/if}

		<section class="saved-item-card" data-testid="saved-item-form">
			<h3 class="saved-item-form__title">New saved item</h3>
			<TextBox
				label="Name"
				id="saved-item-form-name"
				bind:value={newItemName}
				placeholder="Item name"
			/>
			<div class="form-actions">
				<Button testId="saved-item-form-add" variant="primary" onClick={createItem}>Add</Button>
			</div>
		</section>
	{/if}
</div>

<style>
	.saved-items-header {
		display: flex;
		align-items: center;
		gap: var(--space-3);
		margin-bottom: var(--space-6);
	}

	.saved-items-header :global(h1) {
		margin: 0;
	}

	.saved-items-header__icon {
		display: flex;
		color: var(--color-brand);
	}

	.saved-items-empty {
		color: var(--color-text-secondary);
	}

	.saved-items-error {
		color: var(--color-error, red);
	}

	.saved-item-list {
		list-style: none;
		margin: 0 0 var(--space-6);
		padding: 0;
		display: grid;
		gap: var(--space-4);
	}

	.saved-item-card {
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md, 8px);
		padding: var(--space-4);
	}

	.saved-item-card__header {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: var(--space-3);
	}

	.saved-item-name {
		margin: 0;
		font-size: var(--font-size-lg);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
	}

	.saved-item-form__title {
		margin: 0 0 var(--space-3);
		font-size: var(--font-size-lg);
		color: var(--color-text-primary);
	}

	.form-actions {
		display: flex;
		gap: var(--space-2);
		margin-top: var(--space-4);
	}
</style>
