import { fireEvent, render, type RenderResult, waitFor } from '@testing-library/svelte';
import Select from '$lib/ui/inputs/Select/Select.svelte';
import { beforeEach } from 'vitest';

describe('Select', () => {
	let result: RenderResult<Select>;

	function renderComponent(props = {}) {
		if (result) result.unmount();

		result = render(Select, { props });
	}

	beforeEach(() => {
		renderComponent();
	});

	it('should render a select', () => {
		expect(result.container.querySelector('select')).toBeInTheDocument();
	});

	it('rendering options should render the same number of options as the options prop', () => {
		renderComponent({
			options: [
				{ label: 'Option 1', value: 'option1' },
				{ label: 'Option 2', value: 'option2' }
			]
		});
		expect(result.container.querySelectorAll('option')).toHaveLength(2);
	});

	it('rendering options should render the correct labels', () => {
		renderComponent({
			options: [
				{ label: 'Option 1', value: 'option1' },
				{ label: 'Option 2', value: 'option2' }
			]
		});
		expect(result.container.querySelectorAll('option')[0].textContent).toBe('Option 1');
		expect(result.container.querySelectorAll('option')[1].textContent).toBe('Option 2');
	});

	it('rendering options should render the correct values', () => {
		renderComponent({
			options: [
				{ label: 'Option 1', value: 'option1' },
				{ label: 'Option 2', value: 'option2' }
			]
		});
		expect(result.container.querySelectorAll('option')[0].value).toBe('option1');
		expect(result.container.querySelectorAll('option')[1].value).toBe('option2');
	});

	it('should call the onChange function when the select value changes', async () => {
		const onChange = vi.fn();
		renderComponent({ onChange, options: [{ label: 'Option 1', value: 'option1' }] });

		await fireEvent.change(result.container.querySelector('select'), {
			target: { value: 'option1' }
		});

		await waitFor(() => {
			expect(onChange).toHaveBeenCalledTimes(1);
			expect(onChange).toHaveBeenCalledWith('option1');
		});
	});

	it('should call the onChange function when the select value is changed via a select event', () => {
		const onChange = vi.fn();
		renderComponent({ onChange, options: [{ label: 'Option 1', value: 'option1' }] });

		fireEvent.select(result.container.querySelector('select'), { target: { value: 'option1' } });

		expect(onChange).toHaveBeenCalledTimes(1);
		expect(onChange).toHaveBeenCalledWith('option1');
	});

	it('can pass in a value to use as the default value', () => {
		renderComponent({
			options: [
				{ label: 'Option 1', value: 'option1' },
				{ label: 'Option 2', value: 'option2' }
			],
			value: 'option2'
		});
		expect(result.container.querySelector('select').value).toBe('option2');
	});

	it('should render a label', () => {
		renderComponent({ label: 'Test Label' });
		expect(result.getByText('Test Label')).toBeInTheDocument();
	});

	it('the label and the select are hooked up with the correct id / for attributes', () => {
		renderComponent({ label: 'Test Label', id: 'test-id' });
		const label = result.getByText('Test Label');
		const select = result.container.querySelector('select');

		expect(label).toHaveAttribute('for', 'test-id');
		expect(select).toHaveAttribute('id', 'test-id');
	});

	it('the component can render arbitrary html attributes on the select element', () => {
		renderComponent({ id: 'test-id', 'data-test': 'test' });
		expect(result.container.querySelector('select')).toHaveAttribute('data-test', 'test');
	});

	it("should set the 'selected' attribute on the option with the same value as the value prop", () => {
		renderComponent({
			options: [
				{ label: 'Option 1', value: 'option1' },
				{ label: 'Option 2', value: 'option2' }
			],
			value: 'option2'
		});

		expect(result.container.querySelectorAll('option')[0].selected).toBe(false);
		expect(result.container.querySelectorAll('option')[1].selected).toBe(true);
	});
});
