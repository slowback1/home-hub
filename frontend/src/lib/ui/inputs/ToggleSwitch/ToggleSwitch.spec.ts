import type { RenderResult } from '@testing-library/svelte';
import ToggleSwitch from '$lib/ui/inputs/ToggleSwitch/ToggleSwitch.svelte';
import { fireEvent, render, waitFor } from '@testing-library/svelte';
import { beforeEach } from 'vitest';
import ToggleSwitchTestHelpers from './ToggleSwitchTestHelpers';

describe('ToggleSwitch', () => {
	let result: RenderResult<ToggleSwitch>;

	function renderComponent(overrides: object = {}) {
		if (result) result.unmount();

		const props = { id: 'id', ...overrides };

		result = render(ToggleSwitch, { props });
	}

	beforeEach(() => {
		renderComponent();
	});

	it('renders a thing that is detected as a switch', () => {
		const checkbox = result.getByRole('switch');

		expect(checkbox).toBeInTheDocument();
	});

	it('the switch is checked off by default', () => {
		ToggleSwitchTestHelpers.assertToggleState('id', false);
	});

	it('when the switch is clicked it is checked on', async () => {
		ToggleSwitchTestHelpers.toggleSwitch('id');

		await waitFor(() => {
			ToggleSwitchTestHelpers.assertToggleState('id', true);
		});
	});

	it('component has a label that is correctly hooked up via aria-labelledby with the  given id', () => {
		renderComponent({ id: 'test', label: 'hello' });

		const toggle = result.getByRole('switch');

		expect(toggle).toHaveAttribute('aria-labelledby', 'test');

		ToggleSwitchTestHelpers.assertHasLabel('test', 'hello');
	});

	it('by default the wrapper has a test id of the given id', () => {
		renderComponent({ id: 'test' });

		const wrapper = result.getByTestId('test');

		expect(wrapper).toBeInTheDocument();
	});

	it('can override the test id with a custom one', () => {
		renderComponent({ testId: 'something-else' });

		const wrapper = result.getByTestId('something-else');

		expect(wrapper).toBeInTheDocument();
	});

	it('can override whether or not toggle is checked by default', () => {
		renderComponent({ checked: true });

		const toggle = result.getByRole('switch');

		expect(toggle).toHaveAttribute('aria-checked', 'true');
	});

	it('calls back onClick when the toggle is clicked', () => {
		const onClick = vi.fn();

		renderComponent({ onClick });

		const toggle = result.getByRole('switch');

		fireEvent.click(toggle);

		expect(onClick).toHaveBeenCalledWith(true);
	});

	it('clicking twice will correctly call onClick the second time with the toggled value', () => {
		const onClick = vi.fn();

		renderComponent({ onClick });

		const toggle = result.getByRole('switch');

		fireEvent.click(toggle);
		fireEvent.click(toggle);

		expect(onClick).toHaveBeenCalledTimes(2);
	});
});
