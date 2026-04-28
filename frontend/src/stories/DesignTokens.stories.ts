import DesignTokens from './DesignTokens.svelte';
import type { Meta } from '@storybook/svelte';

const meta: Meta = {
	title: 'Design Tokens',
	component: DesignTokens,
	parameters: {
		layout: 'fullscreen'
	}
};

export default meta;

export const Tokens = {};
