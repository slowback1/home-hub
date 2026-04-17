import ComboBox from '$lib/ui/inputs/ComboBox/ComboBox.svelte';
import type { ComboBoxOption } from '$lib/ui/inputs/ComboBox/ComboBoxService';

export default {
	title: 'UI/ComboBox',
	component: ComboBox
};

const options: ComboBoxOption<string>[] = [
	{
		label: 'Option 1',
		value: '1'
	},
	{
		label: 'Option 2',
		value: '2'
	},
	{
		label: 'Option 3',
		value: '3'
	},
	{
		label: 'Chicken',
		value: 'yummy'
	},
	{
		label: 'A really really really long option',
		value: 'long'
	}
];

export const Default = {
	args: {
		label: 'Combo Box Example',
		options: options,
		onSelect: () => {},
		testId: 'storybook-example-combobox'
	}
};
