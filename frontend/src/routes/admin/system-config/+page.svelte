<script lang="ts">
	import { onMount } from 'svelte';
	import { SvelteSet } from 'svelte/reactivity';
	import { Eye, EyeOff } from 'lucide-svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import TextBox from '$lib/ui/inputs/TextBox/TextBox.svelte';
	import SystemConfigApi, { type SystemConfig } from '$lib/api/SystemConfigApi';
	import ToastService, { ToastVariant } from '$lib/ui/containers/toast/ToastService';
	import { toTitleCase } from '$lib/utils/stringUtils';

	const MASK = '••••••••';
	const ICON_SIZE = 16;

	const api = new SystemConfigApi();
	const toastService = new ToastService();

	let loading = true;
	let error: string | null = null;
	let grouped = new Map<string, SystemConfig[]>();
	let editingId: string | null = null;
	let editValue = '';
	let revealedIds = new SvelteSet<string>();

	onMount(async () => {
		try {
			const entries = await api.getAll();
			grouped = groupByNamespace(entries);
		} catch {
			error = 'Failed to load system config.';
		} finally {
			loading = false;
		}
	});

	function groupByNamespace(entries: SystemConfig[]): Map<string, SystemConfig[]> {
		return entries.reduce((map, entry) => {
			const items = map.get(entry.namespace) ?? [];
			items.push(entry);
			map.set(entry.namespace, items);
			return map;
		}, new Map<string, SystemConfig[]>());
	}

	function toggleReveal(id: string) {
		if (revealedIds.has(id)) {
			revealedIds.delete(id);
		} else {
			revealedIds.add(id);
		}
	}

	function displayValue(entry: SystemConfig): string {
		if (!entry.isSecret) return entry.value;
		return revealedIds.has(entry.id) ? entry.value : MASK;
	}

	function startEdit(entry: SystemConfig) {
		editingId = entry.id;
		editValue = entry.value;
	}

	async function saveEdit(entry: SystemConfig) {
		try {
			const updated = await api.update(entry.namespace, entry.key, editValue);
			const items = grouped.get(entry.namespace);
			if (items) {
				const idx = items.findIndex((e) => e.id === entry.id);
				if (idx >= 0) items[idx] = updated;
				grouped = new Map(grouped);
			}
			editingId = null;
			toastService.AddToast({ message: 'Config saved.', variant: ToastVariant.success });
		} catch {
			toastService.AddToast({ message: 'Failed to save config.', variant: ToastVariant.error });
		}
	}

	async function saveSelect(entry: SystemConfig, newValue: string) {
		try {
			const updated = await api.update(entry.namespace, entry.key, newValue);
			const items = grouped.get(entry.namespace);
			if (items) {
				const idx = items.findIndex((e) => e.id === entry.id);
				if (idx >= 0) items[idx] = updated;
				grouped = new Map(grouped);
			}
			toastService.AddToast({ message: 'Config saved.', variant: ToastVariant.success });
		} catch {
			toastService.AddToast({ message: 'Failed to save config.', variant: ToastVariant.error });
		}
	}

	function cancelEdit() {
		editingId = null;
		editValue = '';
	}

	function handleKeydown(event: KeyboardEvent) {
		if (event.key === 'Escape') cancelEdit();
	}
</script>

<svelte:head>
	<title>HomeHub — Admin: System Config</title>
</svelte:head>

{#if loading}
	<Spinner />
{:else if error}
	<p class="error" data-testid="load-error">{error}</p>
{:else}
	<Heading level={1}>System Config</Heading>
	{#each [...grouped.entries()] as [namespace, entries] (namespace)}
		<section class="namespace-section">
			<h2 class="namespace-header" data-testid="namespace-{namespace}">{toTitleCase(namespace)}</h2>
			<table class="config-table">
				<tbody>
					{#each entries as entry (entry.id)}
						<tr data-testid="config-row-{entry.key}">
							<td class="cell-key" data-testid="label-{entry.key}">{toTitleCase(entry.key)}</td>
							<td class="cell-value">
								{#if entry.type === 'select'}
									<select
										data-testid="select-{entry.key}"
										value={entry.value}
										on:change={(e) => saveSelect(entry, e.currentTarget.value)}
									>
										{#each entry.options as opt (opt.value)}
											<option value={opt.value}>{opt.label}</option>
										{/each}
									</select>
								{:else if editingId === entry.id}
									<div class="edit-row" role="presentation" on:keydown={handleKeydown}>
										<TextBox label="" id="edit-{entry.key}" bind:value={editValue} />
										<Button
											testId="btn-save"
											variant="primary"
											size="small"
											onClick={() => saveEdit(entry)}>Save</Button
										>
										<Button
											testId="btn-cancel"
											variant="secondary"
											size="small"
											onClick={cancelEdit}>Cancel</Button
										>
									</div>
								{:else}
									<div class="value-row">
										<span
											class="value-display"
											data-testid="value-{entry.key}"
											role="button"
											tabindex="0"
											on:click={() => startEdit(entry)}
											on:keydown={(e) => {
												if (e.key === 'Enter') startEdit(entry);
											}}>{displayValue(entry)}</span
										>
										{#if entry.isSecret}
											<button
												class="toggle-reveal"
												data-testid="toggle-{entry.key}"
												aria-label={revealedIds.has(entry.id) ? 'Hide value' : 'Show value'}
												on:click={() => toggleReveal(entry.id)}
											>
												{#if revealedIds.has(entry.id)}
													<EyeOff size={ICON_SIZE} />
												{:else}
													<Eye size={ICON_SIZE} />
												{/if}
											</button>
										{/if}
									</div>
								{/if}
							</td>
						</tr>
					{/each}
				</tbody>
			</table>
		</section>
	{/each}
{/if}

<style>
	.namespace-section {
		margin-bottom: var(--space-6);
	}

	.namespace-header {
		font-size: var(--font-size-lg);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
		margin-bottom: var(--space-2);
	}

	.config-table {
		width: 100%;
		border-collapse: collapse;
	}

	.config-table tr {
		border-bottom: 1px solid var(--color-border-subtle);
	}

	.cell-key {
		padding: var(--space-3) var(--space-4);
		color: var(--color-text-secondary);
		font-family: monospace;
		width: 30%;
	}

	.cell-value {
		padding: var(--space-3) var(--space-4);
		color: var(--color-text-primary);
	}

	.value-row {
		display: flex;
		align-items: center;
		gap: var(--space-2);
	}

	.value-display {
		cursor: pointer;
	}

	.value-display:hover {
		color: var(--color-brand-lighter);
		text-decoration: underline;
	}

	.toggle-reveal {
		background: none;
		border: none;
		padding: var(--space-1);
		cursor: pointer;
		color: var(--color-text-secondary);
		display: flex;
		align-items: center;
	}

	.toggle-reveal:hover {
		color: var(--color-text-primary);
	}

	.edit-row {
		display: flex;
		align-items: flex-end;
		gap: var(--space-2);
	}

	.edit-row :global(.text-box__label) {
		display: none;
	}

	.error {
		color: var(--color-error, red);
		padding: var(--space-4);
	}
</style>
