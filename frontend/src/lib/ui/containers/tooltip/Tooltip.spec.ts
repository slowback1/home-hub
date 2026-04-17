import { act, fireEvent, render, type RenderResult, waitFor } from '@testing-library/svelte';
import Tooltip from '$lib/ui/containers/tooltip/Tooltip.svelte';

describe('Tooltip', () => {
	let result: RenderResult<Tooltip>;

	beforeEach(() => {
		result = render(Tooltip, {
			props: {
				text: 'Hello, World!'
			}
		});
	});

	it('renders the slot content', () => {
		const { getByText } = result;

		expect(getByText('Slot Content')).toBeInTheDocument();
	});

	it('hovering over the content shows the tooltip', async () => {
		const { getByTestId, queryByTestId } = result;

		const content = getByTestId('tooltip-content');
		let tooltip = queryByTestId('tooltip');

		expect(tooltip).not.toBeInTheDocument();

		await act(async () => {
			await fireEvent.mouseEnter(content);
		});

		await waitFor(() => {
			tooltip = queryByTestId('tooltip');
			expect(tooltip).toBeVisible();
		});
	});

	it('hovering over the content and then leaving hides the tooltip', async () => {
		const { getByTestId, queryByTestId } = result;

		const content = getByTestId('tooltip-content');

		await act(async () => {
			await fireEvent.mouseEnter(content);
		});

		let tooltip = queryByTestId('tooltip');

		expect(tooltip).toBeVisible();

		await act(async () => {
			await fireEvent.mouseLeave(content);
		});

		await waitFor(() => {
			tooltip = queryByTestId('tooltip');
			expect(tooltip).not.toBeInTheDocument();
		});
	});
});
