import Spinner from './Spinner.svelte';
import type { Meta } from '@storybook/svelte';

const meta: Meta = {
	title: 'Feedback/Spinner',
	component: Spinner,
	argTypes: {
		size: { control: { type: 'select' }, options: ['small', 'medium', 'large'] }
	}
};

export default meta;

export const Small = { args: { size: 'small' } };
export const Medium = { args: { size: 'medium' } };
export const Large = { args: { size: 'large' } };
