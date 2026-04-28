import Badge from './Badge.svelte';
import type { Meta } from '@storybook/svelte';

const meta: Meta = {
	title: 'Feedback/Badge',
	component: Badge,
	argTypes: {
		variant: { control: { type: 'select' }, options: ['default', 'success', 'warning', 'error'] }
	}
};

export default meta;

export const Default = { args: { text: 'Default', variant: 'default' } };
export const Success = { args: { text: 'Success', variant: 'success' } };
export const Warning = { args: { text: 'Warning', variant: 'warning' } };
export const Error = { args: { text: 'Error', variant: 'error' } };
