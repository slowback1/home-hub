import { render } from '@testing-library/svelte';
import Chip from '$lib/ui/containers/chip/Chip.svelte';
import { type ChipProps, ChipSize, ChipVariant } from '$lib/ui/containers/chip/chipTypes';
import { beforeEach } from 'vitest';

describe('Chip', () => {
	let result: ReturnType<typeof render>;

	function renderComponent(overrides: Partial<ChipProps> = {}) {
		if (result) result.unmount();

		const props: ChipProps = { text: 'chip text', ...overrides };

		result = render(Chip, { props: props as never }) as never;
	}

	beforeEach(() => {
		renderComponent();
	});

	it("should render something with a data-testid of 'chip'", () => {
		expect(result.getByTestId('chip')).toBeInTheDocument();
	});

	it('can override the data-testid', () => {
		renderComponent({ testId: 'custom-chip' });

		expect(result.getByTestId('custom-chip')).toBeInTheDocument();
	});

	it('renders the text', () => {
		renderComponent({ text: 'custom text' });

		const chip = result.getByTestId('chip');

		expect(chip).toHaveTextContent('custom text');
	});

	it("by default renders the chip with the class 'chip__primary'", () => {
		expect(result.getByTestId('chip')).toHaveClass('chip__primary');
	});

	it.each([
		[ChipVariant.Primary, 'chip__primary'],
		[ChipVariant.Secondary, 'chip__secondary'],
		[ChipVariant.Outline, 'chip__outline'],
		[ChipVariant.Success, 'chip__success'],
		[ChipVariant.Error, 'chip__error']
	])(
		'when the chip is variant %s, it renders the chip with the class %s',
		(variant, expectedClass) => {
			renderComponent({ variant });

			expect(result.getByTestId('chip')).toHaveClass(expectedClass);
		}
	);

	it('by default renders the chip as a medium size', () => {
		expect(result.getByTestId('chip')).toHaveClass('chip__medium');
	});

	it.each([
		[ChipSize.Small, 'chip__small'],
		[ChipSize.Medium, 'chip__medium'],
		[ChipSize.Large, 'chip__large']
	])('when the chip is size %s, it renders the chip with the class %s', (size, expectedClass) => {
		renderComponent({ size });

		expect(result.getByTestId('chip')).toHaveClass(expectedClass);
	});
});
