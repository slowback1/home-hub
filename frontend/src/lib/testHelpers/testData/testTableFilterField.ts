import {
	type TableFilterField,
	TableFilterFieldType
} from '$lib/ui/table/tableFilter/tableFilterTypes';

export function getTestTableFilterField(
	overrides: Partial<TableFilterField> = {}
): TableFilterField {
	return {
		id: 'id',
		label: 'Label',
		type: TableFilterFieldType.Text,
		value: undefined,
		...overrides
	};
}
