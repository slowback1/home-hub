export type TableFilterFieldProps = {
	onChange: (value: string) => void;
	field: TableFilterField;
};

export type TableFilterField = {
	id: string;
	label: string;
	value?: string;
	type: TableFilterFieldType;
};

export enum TableFilterFieldType {
	Text = 'text',
	Number = 'number'
}

export type TableFilterProps = {
	fields: TableFilterField[];
	onFilter: (filters: Record<string, string | undefined>) => void;
};
