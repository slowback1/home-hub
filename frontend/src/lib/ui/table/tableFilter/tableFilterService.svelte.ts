import type {
	TableFilterField,
	TableFilterFieldProps,
	TableFilterProps
} from '$lib/ui/table/tableFilter/tableFilterTypes';

export default class TableFilterService {
	private fields: TableFilterField[] = $state([]);
	private readonly onFilter: (filters: Record<string, string | undefined>) => void;

	constructor(props: TableFilterProps) {
		this.fields = props.fields;
		this.onFilter = props.onFilter;
	}

	public getInputProps(): TableFilterFieldProps[] {
		return this.fields.map((field) => {
			return {
				field,
				onChange: this.getFilterCallback(field)
			};
		});
	}

	public resetFields() {
		this.fields = this.fields.map((field) => {
			field.value = undefined;
			return field;
		});

		this.filterFields();
	}

	private getFilterCallback(field: TableFilterField) {
		return (value: string) => {
			this.fields[this.fields.findIndex((f) => f.id === field.id)].value = value;
			this.filterFields();
		};
	}

	private filterFields() {
		const fieldMap = this.buildFieldMap();
		this.onFilter(fieldMap);
	}

	private buildFieldMap() {
		const fieldMap: Record<string, string | undefined> = {};

		this.fields.forEach((field) => {
			fieldMap[field.id] = field.value;
		});
		return fieldMap;
	}
}
