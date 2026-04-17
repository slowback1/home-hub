import type { RenderResult } from '@testing-library/svelte';
import Button from './Button.svelte';
import { beforeEach } from 'vitest';
import { fireEvent, render } from '@testing-library/svelte';

describe('Button', () => {
	let result: RenderResult<Button>;

	function renderComponent(props: object = {}) {
		if (result) result.unmount();

		result = render(Button, { props });
	}

	beforeEach(() => {
		renderComponent();
	});

	it('renders a button', () => {
		const button = result.getByRole('button');

		expect(button).toBeInTheDocument();
	});

	it('clicking the button fires a click event', () => {
		const button = result.getByRole('button');

		const eventListener = vi.fn();

		button.addEventListener('click', eventListener);

		fireEvent.click(button);

		expect(eventListener).toHaveBeenCalled();
	});

	it("by default has a 'button-primary' class", () => {
		const button = result.getByRole('button');
		expect(button).toHaveClass('button-primary');
	});

	it("can pass in a 'secondary' variant to get a 'button-secondary' class", () => {
		renderComponent({ variant: 'secondary' });

		const button = result.getByRole('button');

		expect(button).toHaveClass('button-secondary');
	});

	it("can pass in a 'text' variant to get a 'button-text' class", () => {
		renderComponent({ variant: 'text' });

		const button = result.getByRole('button');

		expect(button).toHaveClass('button-text');
	});

	it("does not have a 'button-primary' class when the variant is 'secondary'", () => {
		renderComponent({ variant: 'secondary' });

		const button = result.getByRole('button');

		expect(button).not.toHaveClass('button-primary');
	});

	it("does not have a 'button-secondary' class when the variant is 'primary'", () => {
		const button = result.getByRole('button');

		expect(button).not.toHaveClass('button-secondary');
	});

	it('passes through the test-id attribute', () => {
		const testId = 'test-button';

		renderComponent({ testId: testId });
		const buttonWithTestId = result.getByTestId(testId);

		expect(buttonWithTestId).toBeInTheDocument();
	});

	it('passes through the href attribute to make the button a link', () => {
		renderComponent({ href: '/test' });

		const link = result.getByRole('link');

		expect(link).toBeInTheDocument();
	});

	it('calls the onClick handler when the button is clicked', () => {
		const onClick = vi.fn();

		renderComponent({ onClick });

		const button = result.getByRole('button');

		fireEvent.click(button);

		expect(onClick).toHaveBeenCalled();
	});

	it("has a 'small' size class when passed the small size prop", () => {
		renderComponent({ size: 'small' });

		const button = result.getByRole('button');

		expect(button).toHaveClass('button-small');
	});

	it("has a 'large' size clas when passed the large size prop", () => {
		renderComponent({ size: 'large' });

		const button = result.getByRole('button');

		expect(button).toHaveClass('button-large');
	});

	it('can disable the button', () => {
		renderComponent({ disabled: true });

		const button = result.getByRole('button');

		expect(button).toBeDisabled();
	});

	it('setting disabled to false means the button is enabled', () => {
		renderComponent({ disabled: false });

		const button = result.getByRole('button');

		expect(button).not.toBeDisabled();
	});

	it('has a tabindex of 0 by default', () => {
		renderComponent();

		const button = result.getByRole('button');

		expect(button.tabIndex).toEqual(0);
	});

	it('can update the tab index', () => {
		renderComponent({ tabIndex: 1 });

		const button = result.getByRole('button');

		expect(button.tabIndex).toEqual(1);
	});
});
