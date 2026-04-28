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
	.table {
		width: 100%;
		border-collapse: collapse;
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md);
		overflow: hidden;
		font-size: var(--font-size-md);
	}

	.table__header-row {
		background-color: var(--color-surface-overlay);
		color: var(--color-text-primary);
		border-bottom: 1px solid var(--color-border-default);
	}

	.table__header-cell {
		padding: var(--space-3) var(--space-4);
		text-align: center;
		font-weight: var(--font-weight-semibold);
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
		letter-spacing: 0.05em;
		text-transform: uppercase;
	}

	.table__body-row {
		border-bottom: 1px solid var(--color-border-subtle);
		transition: background-color 0.1s ease;
	}

	.table__body-row:last-child {
		border-bottom: none;
	}

	.table__body-row:hover {
		background-color: var(--color-surface-raised);
	}

	.table__body-cell {
		padding: var(--space-3) var(--space-4);
		text-align: center;
		color: var(--color-text-primary);
	}
</style>
