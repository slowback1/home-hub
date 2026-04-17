import { render, type RenderResult, waitFor } from '@testing-library/svelte';
import Checkbox from '$lib/ui/inputs/Checkbox/Checkbox.svelte';
import type { ComponentProps } from 'svelte';
import { beforeEach, expect } from 'vitest';
import CheckboxTestHelpers from './CheckboxTestHelpers';

describe('Checkbox', () => {
	let result: RenderResult<Checkbox>;

	function renderComponent(overrides: Partial<ComponentProps<Checkbox>> = {}) {
		if (result) result.unmount();

		const props = {
			id: 'test',
			label: 'test',
			...overrides
		};

		result = render(Checkbox, { props });
	}

	beforeEach(() => {
		renderComponent();
	});

	it('renders a checkbox', () => {
		const checkbox = result.container.querySelector('input[type="checkbox"]');

		expect(checkbox).toBeInTheDocument();
	});

	it('renders the given label', () => {
		renderComponent({ label: 'custom label' });
		CheckboxTestHelpers.assertHasLabel('test', 'custom label');
	});

	it('ties the input and id together with the given id', () => {
		renderComponent({ id: 'test' });
		const label = result.container.querySelector('label');
		const checkbox = result.container.querySelector('input[type="checkbox"]');

		expect(checkbox).toHaveAttribute('id', 'test');
		expect(label).toHaveAttribute('for', 'test');
	});

	it('will set the data-testid attribute to the id if given no override', () => {
		renderComponent({ id: 'test' });
		const label = result.container.querySelector('label');
		const checkbox = result.container.querySelector('input[type="checkbox"]');

		expect(checkbox).toHaveAttribute('data-testid', 'test');
		expect(label).toHaveAttribute('data-testid', 'test-label');
	});

	it('can overwrite the testid attribute', () => {
		renderComponent({ testId: 'something-different' });

		const label = result.container.querySelector('label');
		const checkbox = result.container.querySelector('input[type="checkbox"]');

		expect(checkbox).toHaveAttribute('data-testid', 'something-different');
		expect(label).toHaveAttribute('data-testid', 'something-different-label');
	});

	it('is not checked by default when not overridden', () => {
		CheckboxTestHelpers.assertCheckboxISNotChecked('test');
	});

	it('can be checked by default', () => {
		renderComponent({ checked: true });

		CheckboxTestHelpers.assertCheckboxIsChecked('test');
	});

	it('clicking the checkbox checks the checkbox', async () => {
		CheckboxTestHelpers.checkCheckbox('test');

		await waitFor(() => {
			CheckboxTestHelpers.assertCheckboxIsChecked('test');
		});
	});
});
