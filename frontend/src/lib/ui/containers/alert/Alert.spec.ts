import { type AlertProps, AlertType } from '$lib/ui/containers/alert/alertTypes';
import { fireEvent, render } from '@testing-library/svelte';
import Alert from '$lib/ui/containers/alert/Alert.svelte';
import { beforeEach } from 'vitest';

describe('Alert', () => {
	let result: ReturnType<typeof render>;

	function renderComponent(overrides: Partial<AlertProps> = {}) {
		const props: AlertProps = {
			message: 'Test Message',
			...overrides
		};

		if (result) result.unmount();

		result = render(Alert, { props: props as never }) as never;
	}

	beforeEach(() => {
		renderComponent();
	});

	it("should render something with a data-testid of 'alert'", () => {
		expect(result.getByTestId('alert')).toBeInTheDocument();
	});

	it('can override the testId', () => {
		renderComponent({ testId: 'test-alert' });
		expect(result.getByTestId('test-alert')).toBeInTheDocument();
	});

	it('should render the message', () => {
		renderComponent({ message: 'My Custom Alert Message' });

		expect(result.getByText('My Custom Alert Message')).toBeInTheDocument();
	});

	it.each([
		[AlertType.Info, 'alert__info'],
		[AlertType.Warning, 'alert__warning'],
		[AlertType.Error, 'alert__error']
	])("should render the correct class for type '%s'", (type, expectedClass) => {
		renderComponent({ type });

		expect(result.getByTestId('alert')).toHaveClass(expectedClass);
	});

	it('has a close button', () => {
		expect(result.getByTestId('alert-close')).toBeInTheDocument();
	});

	it('clicking the close button should call the onClose function', () => {
		const onClose = vi.fn();
		renderComponent({ onClose });

		fireEvent.click(result.getByTestId('alert-close'));

		expect(onClose).toHaveBeenCalled();
	});

	it('should have a role of "alert"', () => {
		const alert = result.getByRole('alert');

		expect(alert).toBeInTheDocument();
	});
});
