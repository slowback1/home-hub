import { render } from '@testing-library/svelte';
import Spinner from './Spinner.svelte';

describe('Spinner', () => {
	it('renders a spinner element', () => {
		const { getByTestId } = render(Spinner);
		expect(getByTestId('spinner')).toBeInTheDocument();
	});

	it('has an aria-label for accessibility', () => {
		const { getByRole } = render(Spinner);
		expect(getByRole('status')).toBeInTheDocument();
	});

	it('applies the small size class', () => {
		const { getByTestId } = render(Spinner, { props: { size: 'small' } });
		expect(getByTestId('spinner')).toHaveClass('spinner--small');
	});

	it('applies the large size class', () => {
		const { getByTestId } = render(Spinner, { props: { size: 'large' } });
		expect(getByTestId('spinner')).toHaveClass('spinner--large');
	});

	it('defaults to medium size', () => {
		const { getByTestId } = render(Spinner);
		expect(getByTestId('spinner')).toHaveClass('spinner--medium');
	});
});
