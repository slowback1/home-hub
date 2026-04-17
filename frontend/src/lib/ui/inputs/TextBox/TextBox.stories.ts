import TextBox from '$lib/ui/inputs/TextBox/TextBox.svelte';

export default {
	title: 'UI/TextBox',
	component: TextBox,
	argTypes: {
		label: {
			control: { type: 'text' },
			defaultValue: 'Label'
		},
		value: {
			control: { type: 'text' },
			defaultValue: 'value'
		}
	}
};

export const Default = {
	args: {
		label: 'Label'
	}
};

export const WithValue = {
	args: {
		label: 'Label',
		value: 'value'
	}
};

export const Number = {
	args: {
		label: 'Label',
		type: 'number'
	}
};
