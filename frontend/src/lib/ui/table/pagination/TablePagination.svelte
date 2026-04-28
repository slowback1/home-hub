<script lang="ts">
	import type PaginationService from '$lib/ui/table/pagination/paginationService.svelte';
	import Select from '$lib/ui/inputs/Select/Select.svelte';

	const { service }: { service: PaginationService } = $props();

	function onUpdateRowsPerPage(value: string) {
		service.updateRowsPerPage(Number(value));
	}

	let currentPageRequest = $derived.by(() => service.getPageParameters());
	let pageNumbers = $derived.by(() => service.getVisiblePageNumbers());
	let shouldDisableNextButton = $derived.by(() => service.shouldDisableNextButton());
	let shouldDisablePreviousButton = $derived.by(() => service.shouldDisablePreviousButton());
</script>

<div class="table-pagination-wrapper">
	<div class="table-pagination table-pagination__base">
		<button
			disabled={shouldDisablePreviousButton}
			class="table-pagination__page-number"
			onclick={() => service.goToFirstPage()}
		>
			First
		</button>
		<button
			class="table-pagination__page-number"
			disabled={shouldDisablePreviousButton}
			onclick={() => service.goToPreviousPage()}
		>
			Previous
		</button>
		{#each pageNumbers as pageNumber (pageNumber)}
			<button
				class="table-pagination__page-number"
				class:table-pagination__page-number--active={pageNumber === currentPageRequest.page}
				disabled={pageNumber === currentPageRequest.page}
				onclick={() => service.goToPage(pageNumber)}
			>
				{pageNumber}
			</button>
		{/each}
		<button
			class="table-pagination__page-number"
			disabled={shouldDisableNextButton}
			onclick={() => service.goToNextPage()}
		>
			Next
		</button>
		<button
			disabled={shouldDisableNextButton}
			onclick={() => service.goToLastPage()}
			class="table-pagination__page-number"
		>
			Last
		</button>
	</div>
	<Select
		label="Items per page"
		id="items-per-page"
		onChange={onUpdateRowsPerPage}
		options={service.settings.rowsPerPageOptions.map((opt) => ({ value: opt, label: `${opt}` }))}
		bind:value={currentPageRequest.rowsPerPage}
	/>
</div>

<style>
	.table-pagination-wrapper {
		display: flex;
		flex-direction: row;
		align-items: center;
		justify-content: center;
		gap: var(--space-4);
	}

	.table-pagination {
		display: flex;
		flex-direction: row;
		align-items: center;
		justify-content: center;
		gap: var(--space-1);
	}

	.table-pagination__page-number {
		background-color: transparent;
		border: 1px solid var(--color-border-default);
		color: var(--color-text-secondary);
		padding: var(--space-2) var(--space-3);
		border-radius: var(--radius-sm);
		font-family: var(--font-family-primary);
		font-size: var(--font-size-sm);
		cursor: pointer;
		transition:
			background-color 0.15s ease,
			color 0.15s ease;
	}

	.table-pagination__page-number:hover:not(:disabled) {
		background-color: var(--color-surface-raised);
		color: var(--color-text-primary);
	}

	.table-pagination__page-number:disabled {
		opacity: 0.4;
		cursor: not-allowed;
	}

	.table-pagination__page-number--active {
		background-color: var(--color-brand);
		border-color: var(--color-brand-light);
		color: #fff;
	}
</style>
