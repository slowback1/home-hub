import { act, fireEvent, render, type RenderResult, waitFor } from '@testing-library/svelte';
import ComboBox from '$lib/ui/inputs/ComboBox/ComboBox.svelte';
import type { ComponentProps } from 'svelte';
import { beforeEach } from 'vitest';
import type { ComboBoxOption } from '$lib/ui/inputs/ComboBox/ComboBoxService';
import { slugify } from '$lib/utils/stringUtils';
import { KeyboardKeys } from '$lib/utils/KeyboardKeys';
import ComboBoxTestHelpers from './ComboBoxTestHelpers';

describe('ComboBox', () => {
	let result: RenderResult<ComboBox>;

	const testId = 'test-id';

	const testOptionInput: ComboBoxOption<string>[] = [
		{
			label: 'test 1',
			value: 'value 1'
		},
		{
			label: 'test 2',
			value: 'value 2'
		},
		{
			label: 'orange',
			value: '3'
		},
		{
			label: 'apple',
			value: '4'
		},
		{
			label: 'anthill',
			value: '5'
		},
		{
			label: 'aardvark',
			value: '6'
		}
	];
	function renderComponent(overrides: Partial<ComponentProps<ComboBox>> = {}) {
		const props = {
			onSelect: vi.fn(),
			label: 'label',
			options: testOptionInput,
			testId: 'test-id',
			...overrides
		} as never;

		if (result) result.unmount();

		result = render(ComboBox, { props });
	}

	beforeEach(() => {
		renderComponent();
	});

	it('renders an element that is accessible via a test id', () => {
		renderComponent({ testId: 'my-custom-test-id' });

		const comboBox = result.getByTestId('my-custom-test-id');

		expect(comboBox).toBeInTheDocument();
	});

	it('renders the label in a way that is accessible via a test id', () => {
		renderComponent({ label: 'my custom label' });

		ComboBoxTestHelpers.assertThatComboBoxHasLabel(testId, 'my custom label');
	});

	it('renders an input for the combobox', () => {
		ComboBoxTestHelpers.assertThatComboBoxHasValue(testId, '');
	});

	it("the input's id and label are correctly hooked up", () => {
		const label = result.getByTestId(`${testId}__label`);
		const input = result.getByTestId(`${testId}__input`);

		const expectedIdForPair = `${testId}__input`;

		expect(input.id).toEqual(expectedIdForPair);
		expect(label).toHaveAttribute('for', expectedIdForPair);
	});

	it('contains a button to toggle the combobox', () => {
		const button = result.getByTestId(`${testId}__toggle`);

		expect(button).toBeInTheDocument();
	});

	it('initially does not have a list for options', () => {
		const options = result.queryByTestId(`${testId}__options`);

		expect(options).not.toBeInTheDocument();
	});

	async function waitForOptionsToLoad() {
		await waitFor(() => {
			const options = result.queryByTestId(`${testId}__options`);

			expect(options).toBeInTheDocument();
		});
	}

	async function clickTheToggleButtonAndWaitForOptionsToLoad() {
		const button = result.getByTestId(`${testId}__toggle`);

		act(() => {
			fireEvent.click(button);
		});

		await waitForOptionsToLoad();
	}

	it('clicking the toggle reveals the options', async () => {
		await clickTheToggleButtonAndWaitForOptionsToLoad();
	});

	it('the options list has all of the options', async () => {
		await clickTheToggleButtonAndWaitForOptionsToLoad();

		const options = result
			.getByTestId(`${testId}__options`)
			.querySelectorAll(`[data-testid="${testId}__option"]`);

		expect(options.length).toEqual(testOptionInput.length);

		options.forEach((option) => {
			const labelIsInOptionList = testOptionInput.some((opt) =>
				option.textContent.includes(opt.label)
			);

			expect(labelIsInOptionList).toBeTruthy();
		});
	});

	it('typing into the input opens up the options list with a filtered list of the options', async () => {
		const input = result.getByTestId(`${testId}__input`);

		fireEvent.input(input, { target: { value: 'aard' } });

		await waitForOptionsToLoad();

		const options = result.getAllByTestId(`${testId}__option`);

		expect(options.length).toEqual(1);

		expect(options[0]).toHaveTextContent('aardvark');
	});

	it('the options list has the correct roles and aria-labeling on each of the elements', async () => {
		await clickTheToggleButtonAndWaitForOptionsToLoad();

		const list = result.getByTestId(`${testId}__options`);

		expect(list).toHaveAttribute('role', 'listbox');
		expect(list).toHaveAttribute('aria-label', 'label');
		expect(list).toHaveAttribute('id', `${testId}__listbox`);

		const items = result.getAllByTestId(`${testId}__option`);

		items.forEach((item, index) => {
			expect(item).toHaveAttribute('role', 'option');
			expect(item).toHaveAttribute('id', `${testId}-${slugify(testOptionInput[index].value)}`);
		});
	});

	it('the toggle button has the correct aria attributes on it', () => {
		const toggle = result.getByTestId(`${testId}__toggle`);

		expect(toggle).toHaveAttribute('aria-label', 'label');
		expect(toggle).toHaveAttribute('aria-controls', `${testId}__listbox`);
		expect(toggle).toHaveAttribute('aria-expanded', 'false');
		expect(toggle).toHaveAttribute('type', 'button');
	});

	it("the button's aria-expanded is true when the listbox is open", async () => {
		await clickTheToggleButtonAndWaitForOptionsToLoad();

		const toggle = result.getByTestId(`${testId}__toggle`);

		expect(toggle).toHaveAttribute('aria-expanded', 'true');
	});

	it('the first option is the one  that  has focus by default when the option list is opened', async () => {
		await clickTheToggleButtonAndWaitForOptionsToLoad();

		const [focused, ...rest] = result.getAllByTestId(`${testId}__option`);

		expect(focused).toHaveClass('combo-box__option-focused');

		rest.forEach((option) => {
			expect(option).not.toHaveClass('combo-box__option-focused');
		});
	});

	it.each([
		`${testId}`,
		`${testId}__input`,
		`${testId}__label`,
		`${testId}__toggle`,
		`${testId}__options`
	])(
		'arrowing down will shift the focus to the second option when focused on the %s',
		async (elementToFocus) => {
			await clickTheToggleButtonAndWaitForOptionsToLoad();

			fireEvent.keyUp(result.getByTestId(elementToFocus), { key: KeyboardKeys.ArrowDown });

			await waitFor(() => {
				const focused = result.getAllByTestId(`${testId}__option`)[1];

				expect(focused).toHaveClass('combo-box__option-focused');
			});
		}
	);

	it('the input indicates the aria-activedescendent value when the options are visible', async () => {
		await clickTheToggleButtonAndWaitForOptionsToLoad();

		const input = result.getByTestId(`${testId}__input`);

		expect(input).toHaveAttribute(
			'aria-activedescendant',
			`${testId}-${slugify(testOptionInput[0].value)}`
		);
	});

	it("pressing 'enter' will select the currently focused option", async () => {
		await clickTheToggleButtonAndWaitForOptionsToLoad();

		fireEvent.keyUp(result.getByTestId(testId), { key: KeyboardKeys.Enter });

		await waitFor(() => {
			const input = result.getByTestId(`${testId}__input`);

			expect(input).toHaveValue(testOptionInput[0].label);
		});
	});

	it('clicking an option value selects that value', async () => {
		await clickTheToggleButtonAndWaitForOptionsToLoad();

		const option = result.getAllByTestId(`${testId}__option`)[1];

		fireEvent.click(option);

		await waitFor(() => {
			const input = result.getByTestId(`${testId}__input`);

			expect(input).toHaveValue(testOptionInput[1].label);
		});
	});

	it('can conmbine typing and clicking to select an option', async () => {
		const label = testOptionInput[2].label;
		await ComboBoxTestHelpers.selectComboBoxOption(testId, label);

		ComboBoxTestHelpers.assertThatComboBoxHasValue(testId, label);
	});

	it('hovering over an option value switches focus to that option', async () => {
		await clickTheToggleButtonAndWaitForOptionsToLoad();

		const option = result.getAllByTestId(`${testId}__option`)[1];

		fireEvent.mouseEnter(option);

		await waitFor(() => {
			const option = result.getAllByTestId(`${testId}__option`)[1];

			expect(option).toHaveClass('combo-box__option-focused');
		});
	});
});
