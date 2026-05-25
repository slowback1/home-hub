<script lang="ts">
	import { onMount } from 'svelte';
	import { ChevronRight, Plus } from 'lucide-svelte';
	import DashboardApi from '$lib/api/DashboardApi';
	import { WIDGET_REGISTRY, getVisibleWidgets } from '$lib/services/Dashboard/widgetRegistry';

	const SLOT_COUNT = 6;
	const HOUR_NOON = 12;
	const HOUR_EVENING = 17;
	const HOUR_NIGHT = 22;
	const TOAST_DURATION_MS = 4000;
	const ICON_SM = 16;
	const ICON_MD = 18;
	const ICON_LG = 20;
	const ICON_XL = 22;
	const ICON_STROKE = 2.25;
	const api = new DashboardApi();

	let slots: (string | null)[] = Array(SLOT_COUNT).fill(null);
	let loading = true;
	let editMode = false;
	let pickerSlotIndex: number | null = null;
	let undoToast: {
		widgetId: string;
		slotIndex: number;
		timer: ReturnType<typeof setTimeout>;
	} | null = null;

	$: allEmpty = slots.every((s) => !s);
	$: placedCount = slots.filter(Boolean).length;
	const visibleWidgets = getVisibleWidgets();
	$: placedIds = new Set(slots.filter(Boolean) as string[]);

	function greeting(): string {
		const h = new Date().getHours();
		if (h < HOUR_NOON) return 'good morning';
		if (h < HOUR_EVENING) return 'good afternoon';
		if (h < HOUR_NIGHT) return 'good evening';
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

	async function removeWidget(slotIndex: number) {
		const removed = slots[slotIndex];
		if (!removed) return;
		slots = slots.map((v, i) => (i === slotIndex ? null : v));
		await saveLayout();
		if (undoToast) clearTimeout(undoToast.timer);
		const timer = setTimeout(() => {
			undoToast = null;
		}, TOAST_DURATION_MS);
		undoToast = { widgetId: removed, slotIndex, timer };
	}

	async function undoRemove() {
		if (!undoToast) return;
		clearTimeout(undoToast.timer);
		slots = slots.map((v, i) => (i === undoToast!.slotIndex ? undoToast!.widgetId : v));
		await saveLayout();
		undoToast = null;
	}

	const PRESETS: { id: string; label: string; widgetIds: string[] }[] = [
		{ id: 'daily', label: 'Daily essentials', widgetIds: ['tasks', 'weather', 'activity'] },
		{ id: 'media', label: 'Media & generation', widgetIds: ['audiobook', 'comfyui', 'retro'] },
		{
			id: 'everything',
			label: 'Everything',
			widgetIds: ['tasks', 'weather', 'activity', 'audiobook', 'comfyui', 'retro']
		}
	];

	async function applyPreset(widgetIds: string[]) {
		const available = widgetIds.filter((id) => visibleWidgets.some((w) => w.id === id));
		const next: (string | null)[] = Array(SLOT_COUNT).fill(null);
		available.forEach((id, i) => {
			if (i < SLOT_COUNT) next[i] = id;
		});
		slots = next;
		await saveLayout();
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

		{#if allEmpty}
			<div class="first-run" data-testid="first-run-hero">
				<div class="first-run__panel">
					<h2 class="first-run__title">Your dashboard is empty.</h2>
					<p class="first-run__body">
						Pin widgets here to see your tasks, the weather, today's activity pick, and whatever
						else you actually look at every morning. Up to six at a time.
					</p>
					<div class="first-run__actions">
						<button class="btn btn--primary" on:click={() => (pickerSlotIndex = 0)}>
							<Plus size={ICON_SM} strokeWidth={ICON_STROKE} /> Add your first widget
						</button>
					</div>
				</div>
				<div class="first-run__suggestions">
					<div class="first-run__suggestions-label">or start from a preset</div>
					<div class="first-run__suggestions-grid">
						{#each PRESETS as preset (preset.id)}
							{@const available = preset.widgetIds.filter((id) =>
								visibleWidgets.some((w) => w.id === id)
							)}
							<button class="first-run__preset" on:click={() => applyPreset(preset.widgetIds)}>
								<div class="first-run__preset-head">
									<span class="first-run__preset-label">{preset.label}</span>
									<span class="first-run__preset-count"
										>{available.length} widget{available.length === 1 ? '' : 's'}</span
									>
								</div>
								<div class="first-run__preset-icons">
									{#each available as id (id)}
										{@const w = widgetEntry(id)}
										{#if w}
											<span class="first-run__preset-icon" title={w.name}>
												<svelte:component this={w.icon} size={ICON_SM} />
											</span>
										{/if}
									{/each}
								</div>
								<div class="first-run__preset-picks">
									{available
										.map((id) => widgetEntry(id)?.name)
										.filter(Boolean)
										.join(' · ')}
								</div>
							</button>
						{/each}
					</div>
				</div>
			</div>
		{:else}
			<div class="slot-grid">
				{#each slots as widgetId, i (i)}
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
											<svelte:component this={entry.icon} size={ICON_MD} />
										</span>
										<span class="widget-header__name">{entry.name}</span>
									</div>
									{#if !editMode}
										<a href={entry.href} class="widget-header__open" title="Open {entry.name}">
											<ChevronRight size={ICON_SM} />
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
										on:click={() => removeWidget(i)}
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
									<Plus size={ICON_XL} strokeWidth={ICON_STROKE} />
								</span>
								<span class="slot-empty__label">Add widget</span>
							</button>
						{/if}
					</div>
				{/each}
			</div>
		{/if}
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
					<button class="modal-close" aria-label="Close" on:click={() => (pickerSlotIndex = null)}
						>✕</button
					>
				</div>

				<ul class="picker-list">
					{#each visibleWidgets.filter((w) => !placedIds.has(w.id)) as w (w.id)}
						<li>
							<button
								class="picker-card"
								data-testid="widget-picker-card"
								data-widget-id={w.id}
								on:click={() => assignWidget(pickerSlotIndex, w.id)}
							>
								<span class="picker-card__icon">
									<svelte:component this={w.icon} size={ICON_LG} />
								</span>
								<span class="picker-card__text">
									<span class="picker-card__name">{w.name}</span>
									<span class="picker-card__desc">{w.description}</span>
								</span>
								<span class="picker-card__add"
									><Plus size={ICON_SM} strokeWidth={ICON_STROKE} /></span
								>
							</button>
						</li>
					{/each}
					{#if [...placedIds].length > 0}
						<li class="picker-divider"><span>already on dashboard</span></li>
						{#each visibleWidgets.filter((w) => placedIds.has(w.id)) as w (w.id)}
							<li>
								<div class="picker-card picker-card--disabled" aria-disabled="true">
									<span class="picker-card__icon">
										<svelte:component this={w.icon} size={ICON_LG} />
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

	{#if undoToast}
		{@const removed = widgetEntry(undoToast.widgetId)}
		<div class="toast" data-testid="undo-toast" role="status">
			<span>Removed <strong>{removed?.name ?? undoToast.widgetId}</strong>.</span>
			<button class="undo-btn" on:click={undoRemove}>Undo</button>
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

	/* Primary button */
	.btn--primary {
		background: var(--color-brand-lighter);
		color: #fff;
		border-color: var(--color-brand-lighter);
	}
	.btn--primary:hover {
		background: var(--color-brand-light);
		border-color: var(--color-brand-light);
	}

	/* First-run hero */
	.first-run {
		display: flex;
		flex-direction: column;
		gap: var(--space-6);
		max-width: 900px;
	}
	.first-run__panel {
		display: flex;
		flex-direction: column;
		gap: var(--space-3);
		padding: var(--space-8) var(--space-6);
		background: var(--color-surface-raised);
		border: 1px dashed var(--color-border-default);
		border-radius: var(--radius-md);
		max-width: 640px;
	}
	.first-run__title {
		margin: 0;
		font-size: var(--font-size-2xl);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
		line-height: var(--line-height-tight);
	}
	.first-run__body {
		margin: 0;
		color: var(--color-text-secondary);
		font-size: var(--font-size-md);
		line-height: var(--line-height-relaxed);
		max-width: 56ch;
	}
	.first-run__actions {
		display: flex;
		gap: var(--space-3);
		margin-top: var(--space-2);
	}
	.first-run__suggestions {
		display: flex;
		flex-direction: column;
		gap: var(--space-3);
	}
	.first-run__suggestions-label {
		font-size: var(--font-size-xs);
		color: var(--color-text-secondary);
		text-transform: uppercase;
		letter-spacing: 0.06em;
		font-weight: var(--font-weight-semibold);
	}
	.first-run__suggestions-grid {
		display: grid;
		grid-template-columns: repeat(3, minmax(0, 1fr));
		gap: var(--space-3);
	}
	.first-run__preset {
		display: flex;
		flex-direction: column;
		gap: var(--space-2);
		padding: var(--space-4);
		background: var(--color-surface-raised);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md);
		cursor: pointer;
		text-align: left;
		font-family: var(--font-family-primary);
		color: inherit;
		transition:
			border-color 0.15s ease,
			background-color 0.15s ease;
	}
	.first-run__preset:hover {
		border-color: var(--color-brand-lighter);
		background: var(--color-surface-overlay);
	}
	.first-run__preset-head {
		display: flex;
		align-items: baseline;
		justify-content: space-between;
		gap: var(--space-2);
	}
	.first-run__preset-label {
		font-size: var(--font-size-md);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
	}
	.first-run__preset-count {
		font-size: var(--font-size-xs);
		color: var(--color-text-disabled);
	}
	.first-run__preset-icons {
		display: flex;
		gap: var(--space-1);
		flex-wrap: wrap;
	}
	.first-run__preset-icon {
		width: 28px;
		height: 28px;
		display: inline-flex;
		align-items: center;
		justify-content: center;
		background: var(--color-surface-base);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-sm);
		color: var(--color-brand-lighter);
	}
	.first-run__preset-picks {
		font-size: var(--font-size-xs);
		color: var(--color-text-secondary);
		line-height: var(--line-height-normal);
	}
	@media (max-width: 960px) {
		.first-run__suggestions-grid {
			grid-template-columns: 1fr;
		}
	}

	/* Toast */
	.toast {
		position: fixed;
		bottom: var(--space-6);
		left: 50%;
		transform: translateX(-50%);
		background: var(--color-surface-overlay);
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-sm);
		padding: var(--space-3) var(--space-4);
		display: flex;
		align-items: center;
		gap: var(--space-4);
		color: var(--color-text-secondary);
		font-size: var(--font-size-sm);
		z-index: 200;
		white-space: nowrap;
	}
	.toast strong {
		color: var(--color-text-primary);
		font-weight: var(--font-weight-semibold);
	}
	.undo-btn {
		background: none;
		border: none;
		color: var(--color-brand-lighter);
		cursor: pointer;
		font-family: var(--font-family-primary);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-semibold);
		padding: 0;
	}
	.undo-btn:hover {
		text-decoration: underline;
	}
</style>
