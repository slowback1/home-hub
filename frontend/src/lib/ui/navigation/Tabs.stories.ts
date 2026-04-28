import Tabs from './Tabs.svelte';
import type { Meta } from '@storybook/svelte';

const meta: Meta = {
	title: 'Navigation/Tabs',
	component: Tabs
};

export default meta;

export const Default = {
	args: {
		tabs: [
			{ id: 'overview', label: 'Overview' },
			{ id: 'details', label: 'Details' },
			{ id: 'history', label: 'History' }
		],
		activeTab: 'overview'
	}
};
