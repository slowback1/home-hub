import { fireEvent, render, type RenderResult } from '@testing-library/svelte';
import ToastItem from '$lib/ui/containers/toast/ToastItem.svelte';
import { type ToastConfig, ToastVariant } from '$lib/ui/containers/toast/ToastService';
import type { Mock } from '@storybook/test';
import { beforeEach } from 'vitest';

describe('ToastItem', () => {
	let result: RenderResult<ToastItem>;
	let onCloseMock: Mock;

	function renderComponent(config: ToastConfig) {
		if (result) result.unmount();

		onCloseMock = vi.fn();
		const props = { config, onClose: onCloseMock };

		result = render(ToastItem, { props });
	}

	beforeEach(() => {
		renderComponent({ message: 'hello world', variant: ToastVariant.info });
	});

	it('renders a wrapper for the toast item', () => {
		const wrapper = result.getByTestId('toast-item');

		expect(wrapper).toBeInTheDocument();
	});

	it.each([
		[ToastVariant.info, 'toast-item__info'],
		[ToastVariant.warning, 'toast-item__warning'],
		[ToastVariant.success, 'toast-item__success'],
		[ToastVariant.error, 'toast-item__error']
	])('when given a variant %s, has a class named %s', (variant, expectedClass) => {
		renderComponent({ message: 'hello world', variant });

		const wrapper = result.getByTestId('toast-item');

		expect(wrapper).toHaveClass(expectedClass);
	});

	it('contains the text content of the message', () => {
		const wrapper = result.getByTestId('toast-item');

		expect(wrapper).toHaveTextContent('hello world');
	});

	it("coerces the variant to 'info' if none is given", () => {
		renderComponent({ message: 'hi' });

		const wrapper = result.getByTestId('toast-item');

		expect(wrapper).toHaveClass('toast-item__info');
	});

	it('has a close button', () => {
		const closeButton = result.getByTestId('toast-item__close');

		expect(closeButton).toBeInTheDocument();
	});

	it('clicking the close button calls the onClose mock', () => {
		const closeButton = result.getByTestId('toast-item__close');

		fireEvent.click(closeButton);

		expect(onCloseMock).toHaveBeenCalled();
	});
});
