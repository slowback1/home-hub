import Header from '$lib/components/navigation/Header.svelte';
import type { RenderResult } from '@testing-library/svelte';
import { render } from '@testing-library/svelte';
import { beforeEach } from 'vitest';

describe('Header', () => {
	let result: RenderResult<Header>;

	function renderComponent() {
		if (result) result.unmount();

		result = render(Header);
	}

	beforeEach(() => {
		renderComponent();
	});

	it('renders the nav', () => {
		const nav = result.getByTestId('header');

		expect(nav).toBeInTheDocument();
	});

	it('contains a skip to content link', () => {
		expect(result.container.querySelector("[href='#content']")).toBeInTheDocument();
	});

	it('contains a link to the home page', () => {
		expect(result.container.querySelector("[href='/']")).toBeInTheDocument();
	});
});
