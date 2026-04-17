import Select from '$lib/ui/inputs/Select/Select.svelte';
import type { SelectOption } from '$lib/ui/inputs/Select/SelectTypes';

export default {
	title: 'UI/Select',
	component: Select
};

const options: SelectOption[] = [
	{
		label: 'Option 1',
		value: 'option1'
	},
	{
		label: 'Option 2',
		value: 'option2'
	},
	{
		label: 'Option 3',
		value: 'option3'
	}
];

export const Default = {
	args: {
		options,
		label: 'Select an option',
		id: 'test'
	}
};

export const WithALongLabel = {
	args: {
		options,
		label: 'Select an option with a really really really long long label',
		id: 'test'
	}
};

export const WithADefaultValue = {
	args: {
		options,
		label: 'Select an option',
		value: options[1].value,
		id: 'test'
	}
};
