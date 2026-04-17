<script lang="ts">
	import {
		type TableFilterFieldProps,
		TableFilterFieldType
	} from '$lib/ui/table/tableFilter/tableFilterTypes';
	import TextFilter from '$lib/ui/table/tableFilter/filterFields/TextFilter.svelte';
	import NumberFilter from '$lib/ui/table/tableFilter/filterFields/NumberFilter.svelte';

	let props: TableFilterFieldProps = $props();
</script>

{#if props.field.type === TableFilterFieldType.Text}
	<TextFilter {...props} />
{/if}

{#if props.field.type === TableFilterFieldType.Number}
	<NumberFilter {...props} />
{/if}
