import { act, fireEvent, render, type RenderResult, waitFor } from '@testing-library/svelte';
import ToastWrapper from '$lib/ui/containers/toast/ToastWrapper.svelte';
import { beforeEach } from 'vitest';
import ToastService, { ToastVariant } from '$lib/ui/containers/toast/ToastService';
import MessageBus from '$lib/bus/MessageBus';

describe('ToastWrapper', () => {
	let result: RenderResult<ToastWrapper>;

	beforeEach(() => {
		MessageBus.clearAll();

		if (result) result.unmount();

		result = render(ToastWrapper);
	});

	function addTestToast(message: string, variant: ToastVariant = ToastVariant.info) {
		act(() => {
			const service = new ToastService();

			service.AddToast({ message, variant });
		});
	}

	it('renders a container to wrap around the toasts', () => {
		expect(result.getByTestId('toast-wrapper')).toBeInTheDocument();
	});

	it('contains the toasts that are in the message bus', async () => {
		addTestToast('hi');
		addTestToast('hi 2');
		addTestToast('hi 3');

		await waitFor(() => {
			const items = result.getAllByTestId('toast-item');

			expect(items.length).toEqual(3);
			expect(items[0]).toHaveTextContent('hi');
			expect(items[1]).toHaveTextContent('hi 2');
			expect(items[2]).toHaveTextContent('hi 3');
		});
	});

	it('clicking the remove button on one of the toasts removes the toast', async () => {
		addTestToast('hi 1');

		await waitFor(() => {
			const items = result.getAllByTestId('toast-item');

			expect(items.length).toEqual(1);
			expect(items[0]).toHaveTextContent('hi 1');
		});

		const closeButton = result.getByTestId('toast-item__close');

		await fireEvent.click(closeButton);

		await waitFor(() => {
			const items = result.queryAllByTestId('toast-item');

			expect(items.length).toEqual(0);
		});
	});

	it('has an invisible aria-live region for announcing the most recent toast', () => {
		const live = result.getByTestId('toast-wrapper__live');

		expect(live).toHaveAttribute('aria-live', 'polite');
		expect(live).toHaveClass('screen-reader-only');
	});

	it('the most recent toast is displayed in the aria-live region', async () => {
		addTestToast('hello world');

		await waitFor(() => {
			const live = result.getByTestId('toast-wrapper__live');

			expect(live).toHaveTextContent('hello world');
		});
	});

	it('shows 3 toasts max', async () => {
		addTestToast('hi');
		addTestToast('hi 2');
		addTestToast('hi 3');
		addTestToast('hi 4');

		await waitFor(() => {
			const items = result.getAllByTestId('toast-item');

			expect(items.length).toEqual(3);
			expect(items[0]).toHaveTextContent('hi');
			expect(items[1]).toHaveTextContent('hi 2');
			expect(items[2]).toHaveTextContent('hi 3');
		});
	});
});
