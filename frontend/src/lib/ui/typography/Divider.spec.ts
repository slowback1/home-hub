import { render } from '@testing-library/svelte';
import Divider from './Divider.svelte';

describe('Divider', () => {
	it('renders a divider element', () => {
		const { getByRole } = render(Divider);
		expect(getByRole('separator')).toBeInTheDocument();
	});

	it('has the divider class', () => {
		const { getByRole } = render(Divider);
		expect(getByRole('separator')).toHaveClass('divider');
	});
});
