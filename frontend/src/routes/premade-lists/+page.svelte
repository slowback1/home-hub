<script lang="ts">
	import { onMount } from 'svelte';
	import { ListTodo } from 'lucide-svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import TextBox from '$lib/ui/inputs/TextBox/TextBox.svelte';
	import Select from '$lib/ui/inputs/Select/Select.svelte';
	import ToastService, { ToastVariant } from '$lib/ui/containers/toast/ToastService';
	import PremadeListsApi, { type PremadeList } from '$lib/api/PremadeListsApi';
	import SavedItemsApi, { type SavedItem } from '$lib/api/SavedItemsApi';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';

	const ICON_LG = 24;

	const premadeApi = new PremadeListsApi();
	const savedApi = new SavedItemsApi();
	const toast = new ToastService();

	let loading = true;
	let error = false;
	let enabled = true;
	let lists: PremadeList[] = [];
	let savedItems: SavedItem[] = [];
	let newListName = '';
	let pickDrafts: Record<string, string> = {};

	$: pickOptions = [
		{ value: '', label: 'Pick a saved item…' },
		...savedItems.map((savedItem) => ({ value: savedItem.id, label: savedItem.name }))
	];

	onMount(async () => {
		enabled = FeatureFlagService.featureFlags.some(
			(f) => f.name === 'PREMADE_LISTS_ENABLED' && f.isEnabled
		);

		if (!enabled) {
			loading = false;
			return;
		}

		try {
			[lists, savedItems] = await Promise.all([premadeApi.list(), savedApi.list()]);
		} catch {
			error = true;
		} finally {
			loading = false;
		}
	});

	function savedItemName(id: string): string {
		return savedItems.find((s) => s.id === id)?.name ?? id;
	}

	async function createList() {
		const name = newListName.trim();
		if (!name) return;

		try {
			const created = await premadeApi.create(name);
			lists = [...lists, created];
			newListName = '';
			toast.AddToast({ message: 'Premade list created.', variant: ToastVariant.success });
		} catch {
			toast.AddToast({ message: 'Failed to create premade list.', variant: ToastVariant.error });
		}
	}

	async function deleteList(list: PremadeList) {
		try {
			await premadeApi.delete(list.id);
			lists = lists.filter((l) => l.id !== list.id);
			toast.AddToast({ message: 'Premade list deleted.', variant: ToastVariant.success });
		} catch {
			toast.AddToast({ message: 'Failed to delete premade list.', variant: ToastVariant.error });
		}
	}

	async function addSavedItem(list: PremadeList) {
		const savedItemId = pickDrafts[list.id];
		if (!savedItemId) return;

		try {
			const updated = await premadeApi.addItem(list.id, savedItemId);
			lists = lists.map((l) => (l.id === list.id ? updated : l));
			pickDrafts[list.id] = '';
			toast.AddToast({ message: 'Saved item added.', variant: ToastVariant.success });
		} catch {
			toast.AddToast({ message: 'Failed to add saved item.', variant: ToastVariant.error });
		}
	}

	async function removeSavedItem(list: PremadeList, savedItemId: string) {
		try {
			await premadeApi.removeItem(list.id, savedItemId);
			lists = lists.map((l) =>
				l.id === list.id
					? { ...l, savedItemIds: l.savedItemIds.filter((i) => i !== savedItemId) }
					: l
			);
			toast.AddToast({ message: 'Saved item removed.', variant: ToastVariant.success });
		} catch {
			toast.AddToast({ message: 'Failed to remove saved item.', variant: ToastVariant.error });
		}
	}
</script>

<svelte:head>
	<title>Premade Lists — HomeHub</title>
</svelte:head>

<div class="premade-lists-page" data-testid="premade-lists-page">
	<header class="premade-lists-header">
		<span class="premade-lists-header__icon" aria-hidden="true">
			<ListTodo size={ICON_LG} />
		</span>
		<Heading level={1}>Premade Lists</Heading>
	</header>

	{#if !enabled}
		<div data-testid="premade-lists-disabled">
			<p>Premade lists are not available.</p>
		</div>
	{:else if loading}
		<Spinner />
	{:else if error}
		<p class="premade-lists-error" role="alert">Failed to load premade lists.</p>
	{:else}
		{#if lists.length === 0}
			<p class="premade-lists-empty" data-testid="premade-lists-empty-state">
				No premade lists yet — create your first one below.
			</p>
		{:else}
			<ul class="premade-list-list" data-testid="premade-lists-list">
				{#each lists as list (list.id)}
					<li class="premade-list-card" data-testid="premade-list-row-{list.name}">
						<div class="premade-list-card__header">
							<h2 class="premade-list-name">{list.name}</h2>
							<Button
								testId="premade-list-delete-{list.name}"
								variant="secondary"
								onClick={() => deleteList(list)}
							>
								Delete
							</Button>
						</div>

						{#if list.savedItemIds.length > 0}
							<ul class="premade-list-items">
								{#each list.savedItemIds as savedItemId (savedItemId)}
									<li
										class="premade-list-item"
										data-testid="premade-list-item-{list.name}-{savedItemName(savedItemId)}"
									>
										<span>{savedItemName(savedItemId)}</span>
										<Button
											testId="premade-list-remove-item-{list.name}-{savedItemName(savedItemId)}"
											variant="secondary"
											onClick={() => removeSavedItem(list, savedItemId)}
										>
											Remove
										</Button>
									</li>
								{/each}
							</ul>
						{:else}
							<p class="premade-list-items-empty">No saved items in this list yet.</p>
						{/if}

						<div class="premade-list-pick">
							<Select
								id="premade-list-pick-{list.name}"
								data-testid="premade-list-pick-{list.name}"
								bind:value={pickDrafts[list.id]}
								options={pickOptions}
							/>
							<Button
								testId="premade-list-add-item-{list.name}"
								variant="primary"
								onClick={() => addSavedItem(list)}
							>
								Add saved item
							</Button>
						</div>
					</li>
				{/each}
			</ul>
		{/if}

		<section class="premade-list-card" data-testid="premade-list-form">
			<h3 class="premade-list-form__title">New premade list</h3>
			<TextBox
				label="Name"
				id="premade-list-form-name"
				bind:value={newListName}
				placeholder="List name"
			/>
			<div class="form-actions">
				<Button testId="premade-list-form-add" variant="primary" onClick={createList}>Add</Button>
			</div>
		</section>
	{/if}
</div>

<style>
	.premade-lists-header {
		display: flex;
		align-items: center;
		gap: var(--space-3);
		margin-bottom: var(--space-6);
	}

	.premade-lists-header :global(h1) {
		margin: 0;
	}

	.premade-lists-header__icon {
		display: flex;
		color: var(--color-brand);
	}

	.premade-lists-empty {
		color: var(--color-text-secondary);
	}

	.premade-lists-error {
		color: var(--color-error, red);
	}

	.premade-list-list {
		list-style: none;
		margin: 0 0 var(--space-6);
		padding: 0;
		display: grid;
		gap: var(--space-4);
	}

	.premade-list-card {
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md, 8px);
		padding: var(--space-4);
	}

	.premade-list-card__header {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: var(--space-3);
	}

	.premade-list-name {
		margin: 0;
		font-size: var(--font-size-lg);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
	}

	.premade-list-items {
		list-style: none;
		margin: var(--space-3) 0 0;
		padding: 0;
		display: grid;
		gap: var(--space-2);
	}

	.premade-list-item {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: var(--space-3);
	}

	.premade-list-items-empty {
		margin: var(--space-3) 0 0;
		color: var(--color-text-secondary);
	}

	.premade-list-pick {
		display: flex;
		align-items: center;
		gap: var(--space-3);
		margin-top: var(--space-4);
	}

	.premade-list-form__title {
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
