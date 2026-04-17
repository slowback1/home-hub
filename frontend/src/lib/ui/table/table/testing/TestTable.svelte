<script lang="ts">
	import {
		getTestTableData,
		getTestTableDefinitions
	} from '$lib/ui/table/table/testing/testTableDefinitions';
	import Table from '$lib/ui/table/table/Table.svelte';

	const columns = getTestTableDefinitions();
	const rows = getTestTableData();
</script>

<Table {columns} {rows} />
