<script lang="ts">
	import { onMount } from 'svelte';
	import { ListTodo } from 'lucide-svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import TextBox from '$lib/ui/inputs/TextBox/TextBox.svelte';
	import Select from '$lib/ui/inputs/Select/Select.svelte';
	import ToastService, { ToastVariant } from '$lib/ui/containers/toast/ToastService';
	import ShoppingListApi, {
		type ShoppingList,
		type ShoppingListItem
	} from '$lib/api/ShoppingListApi';
	import SavedItemsApi, { type SavedItem } from '$lib/api/SavedItemsApi';
	import PremadeListsApi, { type PremadeList } from '$lib/api/PremadeListsApi';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';

	const ICON_LG = 24;

	const api = new ShoppingListApi();
	const savedItemsApi = new SavedItemsApi();
	const premadeApi = new PremadeListsApi();
	const toast = new ToastService();

	let loading = true;
	let error = false;
	let enabled = true;
	let savedItemsEnabled = false;
	let premadeEnabled = false;
	let savedItems: SavedItem[] = [];
	let premadeLists: PremadeList[] = [];
	let premadeDraft = '';
	let lists: ShoppingList[] = [];
	let archivedLists: ShoppingList[] = [];
	let newListName = '';
	let itemDrafts: Record<string, string> = {};

	$: premadeOptions = [
		{ value: '', label: 'None' },
		...premadeLists.map((premade) => ({ value: premade.id, label: premade.name }))
	];

	onMount(async () => {
		enabled = FeatureFlagService.featureFlags.some(
			(f) => f.name === 'SHOPPING_LISTS_ENABLED' && f.isEnabled
		);
		savedItemsEnabled = FeatureFlagService.featureFlags.some(
			(f) => f.name === 'SAVED_ITEMS_ENABLED' && f.isEnabled
		);
		premadeEnabled = FeatureFlagService.featureFlags.some(
			(f) => f.name === 'PREMADE_LISTS_ENABLED' && f.isEnabled
		);

		if (!enabled) {
			loading = false;
			return;
		}

		try {
			const [active, archived] = await Promise.all([api.listActive(), api.listArchived()]);
			lists = active;
			archivedLists = archived;
		} catch {
			error = true;
		} finally {
			loading = false;
		}

		if (savedItemsEnabled) await loadSavedItems();
		if (premadeEnabled) await loadPremadeLists();
	});

	async function loadSavedItems() {
		try {
			savedItems = await savedItemsApi.list();
		} catch {
			savedItems = [];
		}
	}

	async function loadPremadeLists() {
		try {
			premadeLists = await premadeApi.list();
		} catch {
			premadeLists = [];
		}
	}

	function createViaApi(name: string) {
		if (premadeEnabled && premadeDraft) return api.create(name, premadeDraft);
		return api.create(name);
	}

	async function createList() {
		const name = newListName.trim();
		if (!name) return;

		try {
			const created = await createViaApi(name);
			lists = [...lists, created];
			newListName = '';
			premadeDraft = '';
			toast.AddToast({ message: 'List created.', variant: ToastVariant.success });
		} catch {
			toast.AddToast({ message: 'Failed to create list.', variant: ToastVariant.error });
		}
	}

	async function addItem(list: ShoppingList) {
		const text = (itemDrafts[list.id] ?? '').trim();
		if (!text) return;
		await addText(list, text);
	}

	async function addText(list: ShoppingList, text: string) {
		try {
			const created = await api.addItem(list.id, text);
			lists = lists.map((l) => (l.id === list.id ? { ...l, items: [...l.items, created] } : l));
			itemDrafts[list.id] = '';
		} catch {
			toast.AddToast({ message: 'Failed to add item.', variant: ToastVariant.error });
		}
	}

	function suggestionsFor(listId: string): SavedItem[] {
		const draft = (itemDrafts[listId] ?? '').trim().toLowerCase();
		if (!savedItemsEnabled || !draft) return [];
		return savedItems.filter((s) => s.name.toLowerCase().includes(draft));
	}

	async function addSavedItem(list: ShoppingList, saved: SavedItem) {
		await addText(list, saved.name);
	}

	async function toggleItem(list: ShoppingList, item: ShoppingListItem) {
		try {
			const updated = await api.toggleItem(list.id, item.id);
			lists = lists.map((l) =>
				l.id === list.id ? { ...l, items: l.items.map((i) => (i.id === item.id ? updated : i)) } : l
			);
		} catch {
			toast.AddToast({ message: 'Failed to update item.', variant: ToastVariant.error });
		}
	}

	async function completeList(list: ShoppingList) {
		try {
			const completed = await api.complete(list.id);
			lists = lists.filter((l) => l.id !== list.id);
			archivedLists = [...archivedLists, completed];
			toast.AddToast({ message: 'List completed.', variant: ToastVariant.success });
		} catch {
			toast.AddToast({ message: 'Failed to complete list.', variant: ToastVariant.error });
		}
	}

	function formatArchivedDate(list: ShoppingList): string {
		return list.archivedAt ? new Date(list.archivedAt).toLocaleDateString() : '';
	}
</script>

<svelte:head>
	<title>Shopping Lists — HomeHub</title>
</svelte:head>

<div class="shopping-lists-page" data-testid="shopping-lists-page">
	<header class="shopping-lists-header">
		<span class="shopping-lists-header__icon" aria-hidden="true">
			<ListTodo size={ICON_LG} />
		</span>
		<Heading level={1}>Shopping Lists</Heading>
	</header>

	{#if !enabled}
		<div data-testid="shopping-lists-disabled">
			<p>Shopping lists are not available.</p>
		</div>
	{:else if loading}
		<Spinner />
	{:else if error}
		<p class="shopping-lists-error" role="alert">Failed to load shopping lists.</p>
	{:else}
		{#if savedItemsEnabled || premadeEnabled}
			<nav class="shopping-lists-subnav">
				{#if savedItemsEnabled}
					<a
						class="shopping-lists-subnav__link"
						href="/saved-items"
						data-testid="shopping-lists-saved-items-link"
					>
						Saved Items
					</a>
				{/if}
				{#if premadeEnabled}
					<a
						class="shopping-lists-subnav__link"
						href="/premade-lists"
						data-testid="shopping-lists-premade-lists-link"
					>
						Premade Lists
					</a>
				{/if}
			</nav>
		{/if}
		{#if lists.length === 0}
			<p class="shopping-lists-empty" data-testid="shopping-lists-empty-state">
				No shopping lists yet — create your first one below.
			</p>
		{:else}
			<ul class="shopping-list-list" data-testid="shopping-lists-list">
				{#each lists as list (list.id)}
					<li class="shopping-list-card" data-testid="shopping-list-row-{list.name}">
						<div class="shopping-list-card__header">
							<h2 class="shopping-list-name">{list.name}</h2>
							<Button
								testId="shopping-list-complete-{list.name}"
								variant="primary"
								onClick={() => completeList(list)}
							>
								Complete
							</Button>
						</div>

						{#if list.items.length === 0}
							<p class="shopping-list-hint" data-testid="shopping-list-empty-{list.name}">
								No items yet.
							</p>
						{:else}
							<ul class="shopping-item-list">
								{#each list.items as item (item.id)}
									<li>
										<button
											class="shopping-item"
											class:shopping-item--checked={item.isChecked}
											data-testid="shopping-item-{item.text}"
											on:click={() => toggleItem(list, item)}
										>
											{item.text}
										</button>
									</li>
								{/each}
							</ul>
						{/if}

						<div class="shopping-item-add">
							<div class="shopping-item-add__field">
								<TextBox
									label="New item"
									id="shopping-item-input-{list.name}"
									bind:value={itemDrafts[list.id]}
									placeholder="Item name"
								/>
								{#if suggestionsFor(list.id).length > 0}
									<ul
										class="saved-item-suggestions"
										data-testid="saved-item-suggestions-{list.name}"
									>
										{#each suggestionsFor(list.id) as suggestion (suggestion.id)}
											<li>
												<button
													class="saved-item-suggestion"
													data-testid="saved-item-suggestion-{suggestion.name}"
													on:click={() => addSavedItem(list, suggestion)}
												>
													{suggestion.name}
												</button>
											</li>
										{/each}
									</ul>
								{/if}
							</div>
							<Button
								testId="shopping-item-add-{list.name}"
								variant="secondary"
								onClick={() => addItem(list)}
							>
								Add item
							</Button>
						</div>
					</li>
				{/each}
			</ul>
		{/if}

		<section class="shopping-list-card" data-testid="shopping-list-form">
			<h3 class="shopping-list-form__title">New list</h3>
			<TextBox
				label="Name"
				id="shopping-list-form-name"
				bind:value={newListName}
				placeholder="List name"
			/>
			{#if premadeEnabled}
				<div class="shopping-list-premade-pick">
					<Select
						label="Premade list"
						id="shopping-list-premade-picker"
						data-testid="shopping-list-premade-picker"
						bind:value={premadeDraft}
						options={premadeOptions}
					/>
				</div>
			{/if}
			<div class="form-actions">
				<Button testId="shopping-list-form-add" variant="primary" onClick={createList}>Add</Button>
			</div>
		</section>

		{#if archivedLists.length > 0}
			<section class="shopping-archived" data-testid="shopping-lists-archived">
				<h3 class="shopping-archived__title">Archived</h3>
				<ul class="shopping-list-list">
					{#each archivedLists as list (list.id)}
						<li
							class="shopping-list-card shopping-list-card--archived"
							data-testid="shopping-archived-row-{list.name}"
						>
							<div class="shopping-list-card__header">
								<h2 class="shopping-list-name">{list.name}</h2>
								<span
									class="shopping-archived-date"
									data-testid="shopping-archived-date-{list.name}"
								>
									{formatArchivedDate(list)}
								</span>
							</div>
							{#if list.items.length > 0}
								<ul class="shopping-item-list">
									{#each list.items as item (item.id)}
										<li>
											<span
												class="shopping-item"
												class:shopping-item--checked={item.isChecked}
												data-testid="shopping-archived-item-{item.text}"
											>
												{item.text}
											</span>
										</li>
									{/each}
								</ul>
							{/if}
						</li>
					{/each}
				</ul>
			</section>
		{/if}
	{/if}
</div>

<style>
	.shopping-lists-header {
		display: flex;
		align-items: center;
		gap: var(--space-3);
		margin-bottom: var(--space-6);
	}

	.shopping-lists-header :global(h1) {
		margin: 0;
	}

	.shopping-lists-header__icon {
		display: flex;
		color: var(--color-brand);
	}

	.shopping-lists-empty,
	.shopping-list-hint {
		color: var(--color-text-secondary);
	}

	.shopping-lists-subnav {
		display: flex;
		gap: var(--space-4);
		margin-bottom: var(--space-6);
	}

	.shopping-lists-subnav__link {
		color: var(--color-brand-lighter);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		text-decoration: none;
	}

	.shopping-lists-subnav__link:hover {
		text-decoration: underline;
	}

	.shopping-lists-error {
		color: var(--color-error, red);
	}

	.shopping-list-list {
		list-style: none;
		margin: 0 0 var(--space-6);
		padding: 0;
		display: grid;
		gap: var(--space-4);
	}

	.shopping-list-card {
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md, 8px);
		padding: var(--space-4);
	}

	.shopping-list-card__header {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: var(--space-3);
		margin-bottom: var(--space-3);
	}

	.shopping-list-name {
		margin: 0;
		font-size: var(--font-size-lg);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
	}

	.shopping-item-list {
		list-style: none;
		margin: 0 0 var(--space-3);
		padding: 0;
	}

	.shopping-item {
		background: none;
		border: none;
		cursor: pointer;
		padding: var(--space-1) 0;
		font-family: var(--font-family-primary);
		font-size: var(--font-size-md);
		color: var(--color-text-primary);
		text-align: left;
	}

	.shopping-item--checked {
		text-decoration: line-through;
		color: var(--color-text-secondary);
	}

	.shopping-item-add {
		display: flex;
		align-items: flex-end;
		gap: var(--space-2);
		margin-top: var(--space-3);
	}

	.shopping-item-add__field {
		flex: 1;
		min-width: 0;
	}

	.saved-item-suggestions {
		list-style: none;
		margin: var(--space-2) 0 0;
		padding: 0;
		display: grid;
		gap: var(--space-1);
	}

	.saved-item-suggestion {
		background: none;
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-sm);
		cursor: pointer;
		padding: var(--space-1) var(--space-2);
		font-family: var(--font-family-primary);
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
		text-align: left;
	}

	.shopping-list-form__title {
		margin: 0 0 var(--space-3);
		font-size: var(--font-size-lg);
		color: var(--color-text-primary);
	}

	.shopping-list-premade-pick {
		margin-top: var(--space-3);
	}

	.shopping-archived {
		margin-top: var(--space-6);
	}

	.shopping-archived__title {
		margin: 0 0 var(--space-3);
		font-size: var(--font-size-lg);
		color: var(--color-text-secondary);
	}

	.shopping-list-card--archived {
		opacity: 0.85;
	}

	.shopping-archived-date {
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
	}

	.form-actions {
		display: flex;
		gap: var(--space-2);
		margin-top: var(--space-4);
	}
</style>
