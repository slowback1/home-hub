import TableFilterService from '$lib/ui/table/tableFilter/tableFilterService.svelte';
import { beforeEach, type Mock } from 'vitest';
import { getTestTableFilterField } from '$lib/testHelpers/testData/testTableFilterField';

describe('TableFilterService', () => {
	let service: TableFilterService;
	let onFilter: Mock;

	beforeEach(() => {
		onFilter = vi.fn();

		service = new TableFilterService({
			fields: [getTestTableFilterField()],
			onFilter: onFilter
		});
	});

	it('can get the input props for each filter', () => {
		const inputProps = service.getInputProps();

		expect(inputProps).toHaveLength(1);
		expect(inputProps[0].field.label).toBe('Label');
		expect(inputProps[0].field.type).toBe('text');
		expect(inputProps[0].field.value).toEqual(undefined);
		expect(inputProps[0].onChange).toBeInstanceOf(Function);
	});

	it('the onFilter function is called when calling the onChange function', () => {
		const inputProps = service.getInputProps();
		inputProps[0].onChange('value');

		expect(onFilter).toHaveBeenCalledWith({ id: 'value' });
	});

	it("the onChange function updates the field's value", () => {
		const inputProps = service.getInputProps();
		inputProps[0].onChange('value');

		expect(inputProps[0].field.value).toBe('value');
	});

	it('can reset the fields', () => {
		const inputProps = service.getInputProps();
		inputProps[0].onChange('value');
		service.resetFields();
		expect(inputProps[0].field.value).toBe(undefined);
	});

	it('resetting the fields calls the onFilter function', () => {
		const inputProps = service.getInputProps();
		inputProps[0].onChange('value');
		service.resetFields();
		expect(onFilter).toHaveBeenCalledWith({ id: undefined });
	});
});
