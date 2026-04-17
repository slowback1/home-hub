import CheckboxGroup from '$lib/ui/inputs/Checkbox/CheckboxGroup.svelte';

const meta = {
	title: 'UI/CheckboxGroup',
	component: CheckboxGroup
};

export default meta;

export const Default = {
	args: {
		groupLabel: 'Group Label'
	}
};

export const Row = {
	args: {
		groupLabel: 'Group Label',
		align: 'row'
	}
};
