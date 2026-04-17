import Tooltip from './Tooltip.svelte';

export default {
	title: 'UI/Tooltip',
	component: Tooltip
};

export const Default = {
	args: {
		text: 'tooltip'
	}
};

export const WithALongTooltip = {
	args: {
		text: 'This is a long tooltip'
	}
};
