import type { RenderResult } from '@testing-library/svelte';
import HeaderLink from '$lib/components/navigation/HeaderLink.svelte';
import { render } from '@testing-library/svelte';
import { beforeEach } from 'vitest';
import UrlPathProvider, { TestUrlProvider } from '$lib/providers/urlPathProvider';

describe('HeaderLink', () => {
	let result: RenderResult<HeaderLink>;

	function renderComponent(overrides: object = {}) {
		if (result) result.unmount();

		const props = {
			href: '/',
			label: 'label',
			...overrides
		};

		result = render(HeaderLink, { props });
	}

	beforeEach(() => {
		UrlPathProvider.initialize(new TestUrlProvider('/'));

		renderComponent();
	});

	it('renders a link with the given href and label', () => {
		const link = result.getByRole('link');

		expect(link).toHaveTextContent('label');
		expect(link).toHaveAttribute('href', '/');
	});

	it("the link does not have an 'active' class when the href does not match with the current window location", () => {
		renderComponent({ href: '/not-matching' });

		const link = result.getByRole('link');

		expect(link).not.toHaveClass('header-link__active');
	});

	it("the link has an 'active' class when the href matches with the current window location", () => {
		UrlPathProvider.initialize(new TestUrlProvider('/test'));

		renderComponent({ href: '/test' });

		const link = result.getByRole('link');

		expect(link).toHaveClass('header-link__active');
	});
});
