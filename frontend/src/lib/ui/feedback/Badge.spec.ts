import { render } from '@testing-library/svelte';
import Badge from './Badge.svelte';

describe('Badge', () => {
	it('renders a badge element', () => {
		const { getByTestId } = render(Badge, { props: { text: 'New' } });
		expect(getByTestId('badge')).toBeInTheDocument();
	});

	it('displays the provided text', () => {
		const { getByText } = render(Badge, { props: { text: 'New' } });
		expect(getByText('New')).toBeInTheDocument();
	});

	it('accepts a custom testId', () => {
		const { getByTestId } = render(Badge, { props: { text: 'Hi', testId: 'my-badge' } });
		expect(getByTestId('my-badge')).toBeInTheDocument();
	});

	it('applies the default variant class', () => {
		const { getByTestId } = render(Badge, { props: { text: 'Test' } });
		expect(getByTestId('badge')).toHaveClass('badge--default');
	});

	it('applies the success variant class', () => {
		const { getByTestId } = render(Badge, { props: { text: 'OK', variant: 'success' } });
		expect(getByTestId('badge')).toHaveClass('badge--success');
	});

	it('applies the warning variant class', () => {
		const { getByTestId } = render(Badge, { props: { text: 'Warn', variant: 'warning' } });
		expect(getByTestId('badge')).toHaveClass('badge--warning');
	});

	it('applies the error variant class', () => {
		const { getByTestId } = render(Badge, { props: { text: 'Err', variant: 'error' } });
		expect(getByTestId('badge')).toHaveClass('badge--error');
	});
});
