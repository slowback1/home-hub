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
	.table-pagination__base {
		/* spacing and shaping */
		--gap-between-buttons: 0.5rem;
		--button-padding: 0.5rem;
		--button-border-radius: 0.25rem;

		/* default colors */
		--background-color: transparent;
		--border-color: var(--color-font);
		--color: var(--color-font);
		--disabled-opacity: 0.5;

		/* active colors */
		--active-background-color: color-mix(
			in lab,
			var(--color-primary-background),
			var(--color-font) 10%
		);
		--active-color: var(--color-primary-font);
	}

	.table-pagination-wrapper {
		display: flex;
		flex-direction: row;
		align-items: center;
		justify-content: center;
		gap: 12px;
	}

	.table-pagination {
		display: flex;
		flex-direction: row;
		align-items: center;
		justify-content: center;
		gap: var(--gap-between-buttons);
	}

	.table-pagination__page-number {
		background-color: var(--background-color);
		border: 1px solid var(--border-color);
		color: var(--color);
		padding: var(--button-padding);
		border-radius: var(--button-border-radius);
		cursor: pointer;
	}

	.table-pagination__page-number--active {
		background-color: var(--active-background-color);
		color: var(--active-color);
	}
</style>
