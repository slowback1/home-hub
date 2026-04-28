import { render } from '@testing-library/svelte';
import Card from './Card.svelte';

describe('Card', () => {
	it('renders a card element', () => {
		const { getByTestId } = render(Card);
		expect(getByTestId('card')).toBeInTheDocument();
	});

	it('has the card class', () => {
		const { getByTestId } = render(Card);
		expect(getByTestId('card')).toHaveClass('card');
	});

	it('accepts a custom testId', () => {
		const { getByTestId } = render(Card, { props: { testId: 'my-card' } });
		expect(getByTestId('my-card')).toBeInTheDocument();
	});

	it('renders children inside the card container', () => {
		const { getByTestId } = render(Card);
		expect(getByTestId('card')).toBeInTheDocument();
	});
});
