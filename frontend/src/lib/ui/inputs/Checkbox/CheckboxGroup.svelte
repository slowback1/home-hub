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
		--checkbox-group-spacing: 16px;
	}

	.checkbox-group__checkboxes {
		display: flex;
		flex-direction: column;
		flex-wrap: wrap;
		gap: var(--checkbox-group-spacing);
		margin-top: var(--checkbox-group-spacing);
	}
	.checkbox-group__checkboxes-row {
		flex-direction: row;
	}
</style>
