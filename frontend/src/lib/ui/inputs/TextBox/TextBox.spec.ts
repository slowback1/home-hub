import type { RenderResult } from '@testing-library/svelte';
import TextBox from './TextBox.svelte';
import { beforeEach } from 'vitest';
import { fireEvent, render, waitFor } from '@testing-library/svelte';
import TextBoxTestHelpers from './TextBoxTestHelpers';

describe('TextBox', () => {
	let result: RenderResult<TextBox>;

	function renderComponent(props = {}) {
		if (result) result.unmount();

		result = render(TextBox, props);
	}

	beforeEach(() => {
		renderComponent();
	});

	it('renders a text input', () => {
		expect(result.getByRole('textbox')).toBeDefined();
	});

	it('can update the input type', () => {
		renderComponent({ type: 'password' });

		const input = result.container.querySelector('input');

		expect(input.type).toEqual('password');
	});

	it('calls the onChange handler when changing the input', () => {
		const onChange = vi.fn();

		renderComponent({ onChange });

		const input = result.container.querySelector('input');

		fireEvent.change(input, { target: { value: 'test' } });

		expect(onChange).toHaveBeenCalled();
	});

	it('passes the label to the textbox', () => {
		renderComponent({ label: 'test label' });

		TextBoxTestHelpers.assertHasLabel('test-label', 'test label');
	});

	it('generates an id based off of the label', () => {
		renderComponent({ label: 'test label' });

		const input = result.container.querySelector('input');

		expect(input.id).toEqual('test-label');
	});

	it('sets the given id', () => {
		renderComponent({ label: 'test label', id: 'some-other-id' });

		const input = result.container.querySelector('input');

		expect(input.id).toEqual('some-other-id');
	});

	it('sets the value', () => {
		renderComponent({ value: 'value', id: 'test' });

		TextBoxTestHelpers.assertHasValue('test', 'value');
	});

	it("the label has the given test id with '-label' appended to the end of it", () => {
		renderComponent({ id: 'test-id', label: 'my cool label' });

		const label = result.getByTestId('test-id-label');

		expect(label).toBeInTheDocument();
		expect(label).toHaveTextContent('my cool label');
	});

	it('can change the input value', async () => {
		renderComponent({ id: 'test' });

		TextBoxTestHelpers.enterTextIntoTextbox('test', 'Rathalos');

		await waitFor(() => {
			TextBoxTestHelpers.assertHasValue('test', 'Rathalos');
		});
	});

	it.each(['minlength', 'maxlength'])(
		'can pass other input props to the input (%s)',
		(attribute) => {
			renderComponent({ id: 'test', [attribute]: 'test' });

			const input = result.getByTestId('test');

			expect(input).toHaveAttribute(attribute, 'test');
		}
	);
});
