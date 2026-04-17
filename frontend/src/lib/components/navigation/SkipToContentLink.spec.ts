import type { RenderResult } from '@testing-library/svelte';
import SkipToContentLink from '$lib/components/navigation/SkipToContentLink.svelte';
import { beforeEach } from 'vitest';
import { render } from '@testing-library/svelte';

describe('SkipToContentLink', () => {
	let result: RenderResult<SkipToContentLink>;

	function renderComponent() {
		if (result) result.unmount();

		result = render(SkipToContentLink);
	}

	beforeEach(() => {
		renderComponent();
	});

	it("renders a link with an href of '#content'", () => {
		const link = result.getByRole('link');

		expect(link).toHaveAttribute('href', '#content');
	});

	it("the link text is 'Skip to Content'", () => {
		const link = result.getByRole('link');

		expect(link).toHaveTextContent('Skip to Content');
	});
});
