import Button from './Button.svelte';
import type { Meta } from '@storybook/svelte';

const meta: Meta = {
	title: 'UI/Button',
	component: Button,
	argTypes: {
		variant: {
			control: { type: 'select' },
			options: ['primary', 'secondary', 'text']
		},
		size: {
			control: { type: 'select' },
			options: ['small', 'medium', 'large']
		}
	}
};

export default meta;

export const Default = {};

export const Secondary = {
	args: {
		variant: 'secondary'
	}
};

export const Text = {
	args: {
		variant: 'text'
	}
};

export const Small = {
	args: {
		size: 'small'
	}
};

export const Medium = {
	args: {
		size: 'medium'
	}
};

export const Large = {
	args: {
		size: 'large'
	}
};

export const Disabled = {
	args: {
		disabled: true
	}
};
