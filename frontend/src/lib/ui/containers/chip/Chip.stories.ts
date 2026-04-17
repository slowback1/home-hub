import type { Meta } from '@storybook/svelte';
import Chip from '$lib/ui/containers/chip/Chip.svelte';
import { type ChipProps, ChipSize, ChipVariant } from '$lib/ui/containers/chip/chipTypes';

const meta: Meta = {
	title: 'UI/Chip',
	component: Chip as never
};

export default meta;

function makeProps(overrides: Partial<ChipProps> = {}): ChipProps {
	return {
		text: 'chip text',
		...overrides
	};
}

export const Default = {
	args: makeProps()
};

export const Secondary = {
	args: makeProps({ variant: ChipVariant.Secondary })
};

export const Outline = {
	args: makeProps({ variant: ChipVariant.Outline })
};

export const Success = {
	args: makeProps({ variant: ChipVariant.Success })
};

export const Error = {
	args: makeProps({ variant: ChipVariant.Error })
};

export const Small = {
	args: makeProps({ size: ChipSize.Small })
};

export const Large = {
	args: makeProps({ size: ChipSize.Large })
};

export const TinyText = {
	args: makeProps({ text: '5' })
};

export const LongText = {
	args: makeProps({ text: 'This is a really long text that should keep going' })
};
