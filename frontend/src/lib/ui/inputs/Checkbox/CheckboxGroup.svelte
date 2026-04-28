<script lang="ts">
	import Checkbox from '$lib/ui/inputs/Checkbox/Checkbox.svelte';

	export let groupLabel: string = '';
	export let align: 'row' | 'column' = 'column';
</script>

<fieldset class="checkbox-group">
	<legend class="checkbox-group__label">{groupLabel}</legend>

	<div
		data-testid="checkbox-group__checkboxes"
		class="checkbox-group__checkboxes"
		class:checkbox-group__checkboxes-row={align === 'row'}
	>
		<slot>
			<Checkbox id="1" label="example 1" />
			<Checkbox id="2" label="example 2" />
			<Checkbox id="3" label="example 3" />
		</slot>
	</div>
</fieldset>

<style>
	.checkbox-group {
		border: none;
		padding: 0;
	}

	.checkbox-group__label {
		display: block;
		margin-bottom: var(--space-1);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-text-secondary);
	}

	.checkbox-group__checkboxes {
		display: flex;
		flex-direction: column;
		flex-wrap: wrap;
		gap: var(--space-3);
		margin-top: var(--space-2);
	}

	.checkbox-group__checkboxes-row {
		flex-direction: row;
	}
</style>
