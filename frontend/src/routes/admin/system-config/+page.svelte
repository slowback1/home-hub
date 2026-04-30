<script lang="ts">
	import { onMount } from 'svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import SystemConfigApi, { type SystemConfig } from '$lib/api/SystemConfigApi';

	let loading = true;
	let error: string | null = null;
	let grouped = new Map<string, SystemConfig[]>();

	onMount(async () => {
		try {
			const api = new SystemConfigApi();
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
			<h2 class="namespace-header" data-testid="namespace-{namespace}">{namespace}</h2>
			<table class="config-table">
				<tbody>
					{#each entries as entry (entry.id)}
						<tr data-testid="config-row-{entry.key}">
							<td class="cell-key">{entry.key}</td>
							<td class="cell-value">{entry.value}</td>
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
		text-transform: capitalize;
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

	.error {
		color: var(--color-error, red);
		padding: var(--space-4);
	}
</style>
