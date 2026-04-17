import { render, type RenderResult } from '@testing-library/svelte';
import Heading from '$lib/ui/typography/Heading/Heading.svelte';
import { beforeEach } from 'vitest';

describe('Heading', () => {
	let result: RenderResult<typeof Heading>;

	function renderComponent(overrides: { level?: number; fontWeight?: 'bold' | 'normal' } = {}) {
		if (result) result.unmount();

		const props = { ...overrides };

		result = render(Heading, { props });
	}

	beforeEach(() => {
		renderComponent();
	});

	it.each([
		[1, 'h1'],
		[2, 'h2'],
		[3, 'h3'],
		[4, 'h4'],
		[5, 'h5'],
		[6, 'h6']
	])('the level %s renders a %s', (level, selector) => {
		renderComponent({ level });

		const heading = result.container.querySelector(selector);

		expect(heading).toBeInTheDocument();
	});

	it("the heading has a 'heading__bold' class when passed a font weight prop of 'bold'", () => {
		renderComponent({ fontWeight: 'bold' });

		const heading = result.container.querySelector('h1');

		expect(heading).toHaveClass('heading__bold');
	});
});
