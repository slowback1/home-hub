<script lang="ts">
	import { onMount } from 'svelte';
	import { Disc3, Pencil, Trash2 } from 'lucide-svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import TextBox from '$lib/ui/inputs/TextBox/TextBox.svelte';
	import Select from '$lib/ui/inputs/Select/Select.svelte';
	import ToastService, { ToastVariant } from '$lib/ui/containers/toast/ToastService';
	import WheelApi, { type Wheel } from '$lib/api/WheelApi';
	import { parseItems, pickRandom } from '$lib/utils/spinWheel';

	const ICON_LG = 24;
	const ICON_SM = 16;

	const api = new WheelApi();
	const toast = new ToastService();

	let loading = true;
	let error = false;
	let wheels: Wheel[] = [];

	// Editor state (null editingId = create mode).
	let editingId: string | null = null;
	let formName = '';
	let formItems = '';

	// Spin state.
	let selectedWheelId = '';
	let spinResult: string | null = null;

	onMount(async () => {
		try {
			wheels = await api.getAll();
			if (wheels.length > 0) selectedWheelId = wheels[0].id;
		} catch {
			error = true;
		} finally {
			loading = false;
		}
	});

	$: selectOptions = wheels.map((wheel) => ({ label: wheel.name, value: wheel.id }));
	$: selectedWheel = wheels.find((wheel) => wheel.id === selectedWheelId) ?? null;
	$: selectedItems = parseItems(selectedWheel?.items ?? '');
	$: canSpin = selectedItems.length > 0;

	function itemCount(wheel: Wheel): number {
		return parseItems(wheel.items).length;
	}

	function resetForm() {
		editingId = null;
		formName = '';
		formItems = '';
	}

	function startEdit(wheel: Wheel) {
		editingId = wheel.id;
		formName = wheel.name;
		formItems = wheel.items;
	}

	async function saveWheel() {
		const name = formName.trim();
		if (!name) return;

		try {
			if (editingId) {
				const updated = await api.update(editingId, name, formItems);
				wheels = wheels.map((wheel) => (wheel.id === updated.id ? updated : wheel));
				toast.AddToast({ message: 'Wheel updated.', variant: ToastVariant.success });
			} else {
				const created = await api.create(name, formItems);
				wheels = [...wheels, created];
				if (!selectedWheelId) selectedWheelId = created.id;
				toast.AddToast({ message: 'Wheel created.', variant: ToastVariant.success });
			}
			resetForm();
		} catch {
			toast.AddToast({ message: 'Failed to save wheel.', variant: ToastVariant.error });
		}
	}

	async function deleteWheel(wheel: Wheel) {
		try {
			await api.delete(wheel.id);
			wheels = wheels.filter((existing) => existing.id !== wheel.id);
			if (selectedWheelId === wheel.id) {
				selectedWheelId = wheels.length > 0 ? wheels[0].id : '';
				spinResult = null;
			}
			if (editingId === wheel.id) resetForm();
			toast.AddToast({ message: 'Wheel deleted.', variant: ToastVariant.success });
		} catch {
			toast.AddToast({ message: 'Failed to delete wheel.', variant: ToastVariant.error });
		}
	}

	function spin() {
		spinResult = pickRandom(selectedItems);
	}

	function onSelectWheel(value: string) {
		selectedWheelId = value;
		spinResult = null;
	}
</script>

<svelte:head>
	<title>Wheels — HomeHub</title>
</svelte:head>

<div class="wheels-page" data-testid="wheels-page">
	<header class="wheels-header">
		<span class="wheels-header__icon" aria-hidden="true">
			<Disc3 size={ICON_LG} />
		</span>
		<Heading level={1}>Wheels</Heading>
	</header>

	{#if loading}
		<Spinner />
	{:else if error}
		<p class="wheels-error" role="alert">Failed to load wheels.</p>
	{:else}
		<div class="wheels-layout">
			<section class="wheels-card" data-testid="spin-section">
				<h2 class="wheels-card__title">Spin</h2>

				{#if wheels.length === 0}
					<p class="wheels-hint" data-testid="spin-empty-hint">Create a wheel to start spinning.</p>
				{:else}
					<div class="spin-controls">
						<Select
							data-testid="wheel-select"
							label="Wheel"
							id="wheel-select"
							options={selectOptions}
							value={selectedWheelId}
							onChange={onSelectWheel}
						/>
						<Button testId="spin-btn" variant="primary" disabled={!canSpin} onClick={spin}>
							Spin
						</Button>
					</div>

					{#if !canSpin}
						<p class="wheels-hint" data-testid="spin-no-items">This wheel has no items yet.</p>
					{/if}

					{#if spinResult}
						<div class="spin-result">
							<p class="spin-result__value" data-testid="wheel-spin-result">{spinResult}</p>
						</div>
					{/if}
				{/if}
			</section>

			<section class="wheels-card" data-testid="manage-section">
				<h2 class="wheels-card__title">Manage wheels</h2>

				{#if wheels.length === 0}
					<p class="wheels-empty" data-testid="wheels-empty-state">
						No wheels yet — create your first one below.
					</p>
				{:else}
					<ul class="wheel-list" data-testid="wheels-list">
						{#each wheels as wheel (wheel.id)}
							<li class="wheel-row" data-testid="wheel-row-{wheel.name}">
								<div class="wheel-info">
									<span class="wheel-name">{wheel.name}</span>
									<span class="wheel-count" data-testid="wheel-item-count-{wheel.name}">
										{itemCount(wheel)}
										{itemCount(wheel) === 1 ? 'item' : 'items'}
									</span>
								</div>
								<div class="wheel-actions">
									<button
										class="icon-btn"
										data-testid="edit-wheel-btn-{wheel.name}"
										aria-label="Edit {wheel.name}"
										on:click={() => startEdit(wheel)}
									>
										<Pencil size={ICON_SM} />
									</button>
									<button
										class="icon-btn icon-btn--danger"
										data-testid="delete-wheel-btn-{wheel.name}"
										aria-label="Delete {wheel.name}"
										on:click={() => deleteWheel(wheel)}
									>
										<Trash2 size={ICON_SM} />
									</button>
								</div>
							</li>
						{/each}
					</ul>
				{/if}

				<div class="wheel-editor" data-testid="wheel-editor">
					<h3 class="wheel-editor__title">{editingId ? 'Edit wheel' : 'New wheel'}</h3>
					<TextBox
						label="Name"
						id="wheel-form-name"
						bind:value={formName}
						placeholder="Wheel name"
					/>
					<label class="items-label" for="wheel-form-items">Items (one per line)</label>
					<textarea
						id="wheel-form-items"
						class="items-input"
						data-testid="wheel-form-items"
						rows="6"
						bind:value={formItems}
						placeholder="One item per line"
					></textarea>
					<div class="editor-actions">
						<Button testId="wheel-form-save" variant="primary" onClick={saveWheel}>
							{editingId ? 'Save' : 'Add'}
						</Button>
						{#if editingId}
							<Button testId="wheel-form-cancel" variant="secondary" onClick={resetForm}>
								Cancel
							</Button>
						{/if}
					</div>
				</div>
			</section>
		</div>
	{/if}
</div>

<style>
	.wheels-header {
		display: flex;
		align-items: center;
		gap: var(--space-3);
		margin-bottom: var(--space-6);
	}

	.wheels-header :global(h1) {
		margin: 0;
	}

	.wheels-header__icon {
		display: flex;
		color: var(--color-brand);
	}

	.wheels-layout {
		display: grid;
		grid-template-columns: 1fr;
		gap: var(--space-6);
	}

	@media (min-width: 720px) {
		.wheels-layout {
			grid-template-columns: 1fr 1fr;
			align-items: start;
		}
	}

	.wheels-card {
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md, 8px);
		padding: var(--space-5);
	}

	.wheels-card__title {
		margin-top: 0;
		margin-bottom: var(--space-4);
		font-size: var(--font-size-xl);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
	}

	.spin-controls {
		display: flex;
		align-items: flex-end;
		gap: var(--space-3);
		flex-wrap: wrap;
	}

	.spin-result {
		margin-top: var(--space-5);
		padding: var(--space-6) var(--space-4);
		text-align: center;
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md, 8px);
		background: var(--color-brand-light);
	}

	.spin-result__value {
		margin: 0;
		font-size: var(--font-size-3xl, 2rem);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
	}

	.wheels-hint,
	.wheels-empty {
		color: var(--color-text-secondary);
	}

	.wheel-list {
		list-style: none;
		margin: 0 0 var(--space-5);
		padding: 0;
	}

	.wheel-row {
		display: flex;
		align-items: center;
		justify-content: space-between;
		padding: var(--space-3) 0;
		border-bottom: 1px solid var(--color-border-subtle);
	}

	.wheel-info {
		display: flex;
		flex-direction: column;
		gap: var(--space-1);
	}

	.wheel-name {
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
	}

	.wheel-count {
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
	}

	.wheel-actions {
		display: flex;
		gap: var(--space-1);
	}

	.icon-btn {
		background: none;
		border: none;
		cursor: pointer;
		color: var(--color-text-secondary);
		padding: var(--space-2);
		display: inline-flex;
		align-items: center;
		border-radius: var(--radius-sm, 4px);
	}

	.icon-btn:hover {
		color: var(--color-brand);
	}

	.icon-btn--danger:hover {
		color: var(--color-error, red);
	}

	.wheel-editor {
		border-top: 1px solid var(--color-border-subtle);
		padding-top: var(--space-4);
	}

	.wheel-editor__title {
		margin: 0 0 var(--space-3);
		font-size: var(--font-size-lg);
		color: var(--color-text-primary);
	}

	.items-label {
		display: block;
		margin: var(--space-3) 0 var(--space-1);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-text-secondary);
	}

	.items-input {
		width: 100%;
		padding: var(--space-2) var(--space-3);
		background-color: var(--color-surface-raised);
		color: var(--color-text-primary);
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-sm);
		font-family: var(--font-family-primary);
		font-size: var(--font-size-md);
		resize: vertical;
	}

	.items-input:focus {
		outline: none;
		border-color: var(--color-brand-lighter);
	}

	.editor-actions {
		display: flex;
		gap: var(--space-2);
		margin-top: var(--space-4);
	}

	.wheels-error {
		color: var(--color-error, red);
	}
</style>
