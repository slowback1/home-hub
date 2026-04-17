import { act, fireEvent, render, type RenderResult, waitFor } from '@testing-library/svelte';
import ThemeToggle from '$lib/components/navigation/ThemeToggle.svelte';
import { beforeEach } from 'vitest';
import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';
import { ColorTheme } from '$lib/services/Theme/ThemeService';

describe('ThemeToggle', () => {
	let result: RenderResult<ThemeToggle>;

	function renderComponent() {
		if (result) result.unmount();

		result = render(ThemeToggle);
	}

	beforeEach(() => {
		renderComponent();
	});

	it('renders a toggle', () => {
		const toggle = result.getByRole('switch');

		expect(toggle).toBeInTheDocument();
	});

	it('The toggle is unchecked by default when the message bus is null', () => {
		const toggle = result.getByRole('switch');

		expect(toggle).toHaveAttribute('aria-checked', 'false');
	});

	it('the toggle is checked when the message bus value was dark theme', () => {
		MessageBus.sendMessage(Messages.CurrentTheme, ColorTheme.Dark);

		renderComponent();

		const toggle = result.getByRole('switch');

		expect(toggle).toHaveAttribute('aria-checked', 'true');
	});

	it.each([
		[ColorTheme.Dark, 'Turn on the Lights'],
		[ColorTheme.Light, 'Turn off the Lights']
	])('when the theme is %s, the label is %s', (theme, expectedLabel) => {
		MessageBus.sendMessage(Messages.CurrentTheme, theme);
		renderComponent();

		const label = result.container.querySelector('#theme-toggle');

		expect(label).toHaveTextContent(expectedLabel);
	});

	function toggleSwitch() {
		const toggle = result.getByRole('switch');

		act(() => {
			fireEvent.click(toggle);
		});
	}

	it('Clicking the toggle will switch the color theme to dark', async () => {
		toggleSwitch();

		await waitFor(() => {
			const currentTheme = MessageBus.getLastMessage(Messages.CurrentTheme);

			expect(currentTheme).toEqual(ColorTheme.Dark);
		});
	});
});
