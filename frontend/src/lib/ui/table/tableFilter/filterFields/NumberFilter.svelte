<script lang="ts">
	import type { TableFilterFieldProps } from '$lib/ui/table/tableFilter/tableFilterTypes';
	import TextBox from '$lib/ui/inputs/TextBox/TextBox.svelte';

	const { onChange, field }: TableFilterFieldProps = $props();

	function handleChange(event: Event) {
		onChange((event.target as HTMLInputElement).value);
	}
</script>

<div class="number-filter">
	<TextBox
		label={field.label}
		value={field.value}
		id={field.id}
		onChange={(e) => handleChange(e)}
		type="number"
	/>
</div>

<style>
	.number-filter {
		display: flex;
		flex-direction: column;
	}
</style>
