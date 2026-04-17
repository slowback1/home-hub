import type { Meta } from '@storybook/svelte';
import TableFilter from '$lib/ui/table/tableFilter/TableFilter.svelte';
import {
	type TableFilterField,
	TableFilterFieldType,
	type TableFilterProps
} from '$lib/ui/table/tableFilter/tableFilterTypes';
import { getTestTableFilterField } from '$lib/testHelpers/testData/testTableFilterField';

const meta: Meta = {
	title: 'UI/Table/Table Filter',
	component: TableFilter as never
};

export default meta;

const makeProps = (fields: TableFilterField[] = [getTestTableFilterField()]): TableFilterProps => {
	return {
		fields,
		onFilter: () => {}
	};
};

export const WithOneTextFilter = {
	args: makeProps([getTestTableFilterField({ type: TableFilterFieldType.Text })])
};

export const WithFiveTextFilters = {
	args: makeProps([
		getTestTableFilterField({ id: '1', label: 'Name', type: TableFilterFieldType.Text }),
		getTestTableFilterField({ id: '2', label: 'City', type: TableFilterFieldType.Text }),
		getTestTableFilterField({ id: '3', label: 'State', type: TableFilterFieldType.Text }),
		getTestTableFilterField({ id: '4', label: 'Country', type: TableFilterFieldType.Text }),
		getTestTableFilterField({
			id: '5',
			label: 'Favorite Video Game',
			type: TableFilterFieldType.Text
		})
	])
};

export const WithOneNumberFilter = {
	args: makeProps([getTestTableFilterField({ type: TableFilterFieldType.Number })])
};
