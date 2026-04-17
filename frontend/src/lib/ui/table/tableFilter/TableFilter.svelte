<script lang="ts">
	import type {
		TableFilterFieldProps,
		TableFilterProps
	} from '$lib/ui/table/tableFilter/tableFilterTypes';
	import TableFilterService from '$lib/ui/table/tableFilter/tableFilterService.svelte';
	import FilterField from '$lib/ui/table/tableFilter/filterFields/FilterField.svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';

	const filterProps: TableFilterProps = $props();

	const service = new TableFilterService(filterProps);

	const fields: TableFilterFieldProps[] = $derived.by(() => service.getInputProps());
</script>

<div class="table-filter">
	<div class="table-filter__fields">
		{#each fields as field (field.field.id)}
			<FilterField {...field} />
		{/each}
	</div>
	<div class="table-filter__reset">
		<Button size="small" variant="text" onClick={() => service.resetFields()}>Reset Filters</Button>
	</div>
</div>

<style>
	.table-filter {
		display: flex;
		flex-direction: column;
		align-items: flex-start;
		border: 1px solid var(--color-font);
		padding: 1rem;
		border-radius: 12px;
	}

	.table-filter__fields {
		display: flex;
		flex-direction: row;
		align-items: center;
		gap: 12px;
		flex-wrap: wrap;
	}

	.table-filter__reset {
		align-self: flex-end;
	}
</style>
