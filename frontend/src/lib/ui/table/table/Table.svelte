<script lang="ts" generics="T">
	import type { TableProps } from '$lib/ui/table/table/tableTypes';

	const { columns, rows }: TableProps<T> = $props();
</script>

<table class="table table-base">
	<thead>
		<tr class="table__header-row">
			{#each columns as column (column.key)}
				<th class="table__header-cell" data-testid={`table__header-${column.key}`}
					>{column.title}</th
				>
			{/each}
		</tr>
	</thead>
	<tbody>
		{#each rows as row, rowIndex (rowIndex)}
			<tr class="table__body-row" data-testid="table__row">
				{#each columns as column (column.key + rowIndex)}
					<td class="table__body-cell" data-testid={`table__cell-${column.key}-${rowIndex}`}>
						{#if !!column.renderKey}
							{row[column.renderKey]}
						{:else if !!column.renderTemplate}
							<column.renderTemplate columnDefinition={column} {row} {rowIndex} />
						{/if}
					</td>
				{/each}
			</tr>
		{/each}
	</tbody>
</table>

<style>
	.table-base {
		/** border styles */
		--border-color: var(--color-font);
		--border-width: 2px;

		/** cell styles */
		--cell-padding: 12px;

		/**header styles */
		--header-background-color: var(--color-primary-background);
		--header-color: var(--color-primary-font);
	}

	.table {
		width: 100%;
		border: var(--border-width) solid var(--border-color);
	}

	.table__header-row {
		background-color: var(--header-background-color);
		color: var(--header-color);
	}

	.table__header-row,
	.table__body-row:not(:last-child) {
		border-bottom: var(--border-width) solid var(--border-color);
	}

	.table__body-cell,
	.table__header-cell {
		padding: var(--cell-padding);
		text-align: center;
	}
</style>
