<script lang="ts">
	import { onMount } from 'svelte';
	import { ChevronRight, Plus } from 'lucide-svelte';
	import DashboardApi from '$lib/api/DashboardApi';
	import { WIDGET_REGISTRY, getVisibleWidgets } from '$lib/services/Dashboard/widgetRegistry';

	const SLOT_COUNT = 6;
	const api = new DashboardApi();

	let slots: (string | null)[] = Array(SLOT_COUNT).fill(null);
	let loading = true;
	let editMode = false;
	let pickerSlotIndex: number | null = null;

	$: allEmpty = slots.every((s) => !s);
	$: placedCount = slots.filter(Boolean).length;
	$: visibleWidgets = getVisibleWidgets();
	$: placedIds = new Set(slots.filter(Boolean) as string[]);

	function greeting(): string {
		const h = new Date().getHours();
		if (h < 12) return 'good morning';
		if (h < 17) return 'good afternoon';
		if (h < 22) return 'good evening';
		return 'late night';
	}

	onMount(async () => {
		const layout = await api.getLayout();
		const filled = Object.fromEntries(layout.slots.map((s) => [s.slotIndex, s.widgetType]));
		slots = Array.from({ length: SLOT_COUNT }, (_, i) => filled[i] ?? null);
		loading = false;
	});

	async function assignWidget(slotIndex: number, widgetId: string) {
		slots = slots.map((v, i) => (i === slotIndex ? widgetId : v));
		pickerSlotIndex = null;
		await saveLayout();
	}

	async function saveLayout() {
		const payload = slots.map((widgetType, slotIndex) => ({ slotIndex, widgetType }));
		await api.saveLayout(payload);
	}

	function widgetEntry(id: string) {
		return WIDGET_REGISTRY.find((w) => w.id === id) ?? null;
	}
</script>

<svelte:head>
	<title>Dashboard — HomeHub</title>
</svelte:head>

{#if !loading}
	<div class="dashboard-page" data-testid="dashboard-page">
		<header class="dashboard-header">
			<div class="dashboard-header__left">
				<h1>Dashboard</h1>
				<p class="dashboard-header__sub">
					{#if allEmpty}
						Pin the modules you want to see at a glance.
					{:else}
						{placedCount}/{SLOT_COUNT} widgets · {greeting()}
					{/if}
				</p>
			</div>
			<div class="dashboard-header__right">
				{#if editMode}
					<button
						class="btn btn--done"
						data-testid="done-edit-button"
						on:click={() => (editMode = false)}
					>
						Done
					</button>
				{:else}
					<button
						class="btn btn--secondary"
						data-testid="edit-dashboard-button"
						disabled={allEmpty}
						on:click={() => (editMode = true)}
					>
						Edit Dashboard
					</button>
				{/if}
			</div>
		</header>

		<div class="slot-grid">
			{#each slots as widgetId, i}
				{@const entry = widgetId ? widgetEntry(widgetId) : null}
				<div class="slot-cell">
					{#if entry}
						<div
							class="slot slot--filled"
							class:editing={editMode}
							data-testid="filled-slot"
							data-widget-id={entry.id}
						>
							<div class="widget-header">
								<div class="widget-header__title">
									<span class="widget-header__icon">
										<svelte:component this={entry.icon} size={18} />
									</span>
									<span class="widget-header__name">{entry.name}</span>
								</div>
								{#if !editMode}
									<a href={entry.href} class="widget-header__open" title="Open {entry.name}">
										<ChevronRight size={16} />
									</a>
								{/if}
							</div>
							<div class="widget-content">
								<svelte:component this={entry.component} />
							</div>
							{#if editMode}
								<button
									class="slot-remove"
									data-testid="remove-widget-button"
									aria-label="Remove {entry.name}"
									on:click={async () => {
										slots = slots.map((v, idx) => (idx === i ? null : v));
										await saveLayout();
									}}
								>
									×
								</button>
							{/if}
						</div>
					{:else}
						<button
							class="slot slot--empty"
							class:editing={editMode}
							data-testid="empty-slot"
							aria-label="Add a widget"
							on:click={() => (pickerSlotIndex = i)}
						>
							<span class="slot-empty__plus" data-testid="add-widget-button">
								<Plus size={22} strokeWidth={2.25} />
							</span>
							<span class="slot-empty__label">Add widget</span>
						</button>
					{/if}
				</div>
			{/each}
		</div>
	</div>

	{#if pickerSlotIndex !== null}
		<div
			class="modal-backdrop"
			role="presentation"
			on:click|self={() => (pickerSlotIndex = null)}
			on:keydown={(e) => e.key === 'Escape' && (pickerSlotIndex = null)}
		>
			<div
				class="modal picker-modal"
				role="dialog"
				aria-modal="true"
				aria-labelledby="picker-title"
				data-testid="widget-picker-modal"
			>
				<div class="modal-header">
					<div>
						<h2 class="modal-title" id="picker-title">Add a widget</h2>
						<p class="modal-subtitle">Pick one to add to your dashboard.</p>
					</div>
					<button
						class="modal-close"
						aria-label="Close"
						on:click={() => (pickerSlotIndex = null)}
					>✕</button>
				</div>

				<ul class="picker-list">
					{#each visibleWidgets.filter((w) => !placedIds.has(w.id)) as w}
						<li>
							<button
								class="picker-card"
								data-testid="widget-picker-card"
								data-widget-id={w.id}
								on:click={() => assignWidget(pickerSlotIndex, w.id)}
							>
								<span class="picker-card__icon">
									<svelte:component this={w.icon} size={20} />
								</span>
								<span class="picker-card__text">
									<span class="picker-card__name">{w.name}</span>
									<span class="picker-card__desc">{w.description}</span>
								</span>
								<span class="picker-card__add"><Plus size={16} strokeWidth={2.25} /></span>
							</button>
						</li>
					{/each}
					{#if [...placedIds].length > 0}
						<li class="picker-divider"><span>already on dashboard</span></li>
						{#each visibleWidgets.filter((w) => placedIds.has(w.id)) as w}
							<li>
								<div class="picker-card picker-card--disabled" aria-disabled="true">
									<span class="picker-card__icon">
										<svelte:component this={w.icon} size={20} />
									</span>
									<span class="picker-card__text">
										<span class="picker-card__name">{w.name}</span>
										<span class="picker-card__desc">Already placed.</span>
									</span>
									<span class="picker-card__check">✓</span>
								</div>
							</li>
						{/each}
					{/if}
				</ul>

				<div class="modal-actions">
					<button class="btn btn--text" on:click={() => (pickerSlotIndex = null)}>Cancel</button>
				</div>
			</div>
		</div>
	{/if}
{/if}

<style>
	.dashboard-page {
		display: flex;
		flex-direction: column;
		gap: var(--space-6);
	}

	/* Header */
	.dashboard-header {
		display: flex;
		align-items: flex-end;
		justify-content: space-between;
		gap: var(--space-4);
		flex-wrap: wrap;
	}
	.dashboard-header__left {
		display: flex;
		flex-direction: column;
		gap: var(--space-1);
		min-width: 0;
	}
	.dashboard-header h1 {
		margin: 0;
		font-size: var(--font-size-3xl);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
		line-height: var(--line-height-tight);
	}
	.dashboard-header__sub {
		margin: 0;
		color: var(--color-text-secondary);
		font-size: var(--font-size-sm);
	}

	/* Grid */
	.slot-grid {
		display: grid;
		grid-template-columns: repeat(3, minmax(0, 1fr));
		grid-auto-rows: minmax(260px, auto);
		gap: var(--space-4);
	}
	.slot-cell {
		min-height: 0;
		min-width: 0;
		display: flex;
	}

	/* Slot base */
	.slot {
		position: relative;
		flex: 1;
		display: flex;
		flex-direction: column;
		background: var(--color-surface-raised);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md);
		padding: var(--space-4);
		gap: var(--space-3);
		min-width: 0;
	}
	.slot--filled.editing {
		border-color: var(--color-brand);
		box-shadow: inset 0 0 0 1px var(--color-brand);
	}

	/* Empty slot */
	.slot--empty {
		background: transparent;
		border: 1.5px dashed var(--color-border-default);
		color: var(--color-text-secondary);
		cursor: pointer;
		align-items: center;
		justify-content: center;
		transition:
			border-color 0.15s ease,
			color 0.15s ease,
			background-color 0.15s ease;
		font-family: var(--font-family-primary);
	}
	.slot--empty:hover {
		border-color: var(--color-brand-lighter);
		color: var(--color-brand-lighter);
		background: rgba(26, 122, 181, 0.05);
	}
	.slot-empty__plus {
		width: 44px;
		height: 44px;
		display: inline-flex;
		align-items: center;
		justify-content: center;
		border-radius: var(--radius-full);
		border: 1.5px solid currentColor;
		margin-bottom: var(--space-2);
	}
	.slot-empty__label {
		font-size: var(--font-size-md);
		font-weight: var(--font-weight-semibold);
	}

	/* Widget header */
	.widget-header {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: var(--space-2);
		padding-bottom: var(--space-2);
		border-bottom: 1px solid var(--color-border-subtle);
	}
	.widget-header__title {
		display: flex;
		align-items: center;
		gap: var(--space-2);
		min-width: 0;
	}
	.widget-header__icon {
		color: var(--color-text-secondary);
		display: inline-flex;
	}
	.widget-header__name {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-secondary);
		letter-spacing: 0.01em;
		text-transform: uppercase;
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}
	.widget-header__open {
		color: var(--color-text-secondary);
		display: inline-flex;
		padding: var(--space-1);
		border-radius: var(--radius-sm);
		transition:
			color 0.15s ease,
			background-color 0.15s ease;
	}
	.widget-header__open:hover {
		color: var(--color-brand-lighter);
		background: var(--color-surface-overlay);
	}

	/* Widget content */
	.widget-content {
		flex: 1;
		min-height: 0;
	}

	/* Remove badge */
	.slot-remove {
		position: absolute;
		top: -10px;
		right: -10px;
		width: 28px;
		height: 28px;
		background: var(--color-error);
		color: #fff;
		border: 2px solid var(--color-surface-deep);
		border-radius: var(--radius-full);
		display: flex;
		align-items: center;
		justify-content: center;
		cursor: pointer;
		padding: 0;
		font-size: 16px;
		line-height: 1;
		transition:
			transform 0.15s ease,
			background-color 0.15s ease;
		z-index: 1;
	}
	.slot-remove:hover {
		transform: scale(1.08);
	}

	/* Buttons */
	.btn {
		display: inline-flex;
		align-items: center;
		gap: var(--space-2);
		padding: var(--space-2) var(--space-4);
		border-radius: var(--radius-sm);
		font-family: var(--font-family-primary);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-semibold);
		cursor: pointer;
		border: 1px solid transparent;
		transition:
			background-color 0.15s ease,
			color 0.15s ease,
			border-color 0.15s ease;
	}
	.btn--secondary {
		background: var(--color-surface-raised);
		color: var(--color-text-secondary);
		border-color: var(--color-border-default);
	}
	.btn--secondary:hover:not(:disabled) {
		color: var(--color-text-primary);
		border-color: var(--color-brand-lighter);
	}
	.btn--secondary:disabled {
		opacity: 0.4;
		cursor: not-allowed;
	}
	.btn--done {
		background: var(--color-success);
		color: #fff;
		border-color: var(--color-success);
	}
	.btn--done:hover {
		background: var(--color-success-surface);
		color: var(--color-success-text);
	}
	.btn--text {
		background: none;
		border-color: transparent;
		color: var(--color-text-secondary);
	}
	.btn--text:hover {
		color: var(--color-text-primary);
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
		background: var(--color-surface-raised);
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-md);
		padding: var(--space-6);
		width: 100%;
		max-height: 90vh;
		overflow-y: auto;
		display: flex;
		flex-direction: column;
		gap: var(--space-4);
	}
	.picker-modal {
		max-width: 560px;
	}
	.modal-header {
		display: flex;
		align-items: flex-start;
		justify-content: space-between;
		gap: var(--space-4);
	}
	.modal-title {
		margin: 0;
		font-size: var(--font-size-xl);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
	}
	.modal-subtitle {
		margin: 4px 0 0;
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
	}
	.modal-close {
		background: none;
		border: none;
		color: var(--color-text-secondary);
		cursor: pointer;
		font-size: var(--font-size-lg);
		padding: var(--space-1);
		line-height: 1;
	}
	.modal-close:hover {
		color: var(--color-text-primary);
	}
	.modal-actions {
		display: flex;
		justify-content: flex-end;
	}

	/* Picker list */
	.picker-list {
		list-style: none;
		padding: 0;
		margin: 0;
		display: flex;
		flex-direction: column;
		gap: var(--space-2);
		max-height: 60vh;
		overflow-y: auto;
	}
	.picker-divider {
		display: flex;
		align-items: center;
		gap: var(--space-3);
		color: var(--color-text-disabled);
		font-size: var(--font-size-xs);
		text-transform: lowercase;
		letter-spacing: 0.04em;
		margin: var(--space-2) 0 0;
	}
	.picker-divider::before,
	.picker-divider::after {
		content: '';
		flex: 1;
		border-top: 1px solid var(--color-border-subtle);
	}
	.picker-card {
		width: 100%;
		display: flex;
		align-items: center;
		gap: var(--space-3);
		padding: var(--space-3) var(--space-4);
		background: var(--color-surface-raised);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-sm);
		cursor: pointer;
		text-align: left;
		font-family: var(--font-family-primary);
		color: inherit;
		transition:
			background-color 0.15s ease,
			border-color 0.15s ease;
	}
	.picker-card:hover {
		background: var(--color-surface-overlay);
		border-color: var(--color-brand-lighter);
	}
	.picker-card__icon {
		width: 40px;
		height: 40px;
		display: inline-flex;
		align-items: center;
		justify-content: center;
		background: var(--color-surface-overlay);
		color: var(--color-brand-lighter);
		border-radius: var(--radius-sm);
		flex-shrink: 0;
	}
	.picker-card__text {
		flex: 1;
		display: flex;
		flex-direction: column;
		gap: 2px;
		min-width: 0;
	}
	.picker-card__name {
		font-size: var(--font-size-md);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
	}
	.picker-card__desc {
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
	}
	.picker-card__add,
	.picker-card__check {
		display: inline-flex;
		align-items: center;
		justify-content: center;
		width: 28px;
		height: 28px;
		border-radius: var(--radius-sm);
		color: var(--color-brand-lighter);
		background: var(--color-surface-base);
		border: 1px solid var(--color-border-subtle);
		flex-shrink: 0;
	}
	.picker-card--disabled {
		cursor: not-allowed;
		opacity: 0.5;
	}
	.picker-card--disabled:hover {
		background: var(--color-surface-raised);
		border-color: var(--color-border-subtle);
	}

	/* Responsive */
	@media (max-width: 960px) {
		.slot-grid {
			grid-template-columns: repeat(2, minmax(0, 1fr));
		}
	}
	@media (max-width: 640px) {
		.slot-grid {
			grid-template-columns: 1fr;
		}
	}
</style>
