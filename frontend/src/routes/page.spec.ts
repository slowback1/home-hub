import { render, waitFor } from '@testing-library/svelte';
import { afterEach, beforeEach } from 'vitest';
import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';
import { mockApi } from '$lib/testHelpers/getFetchMock';
import Page from './+page.svelte';

const emptyLayout = {
	layoutFormat: '3x2',
	slots: [0, 1, 2, 3, 4, 5].map((slotIndex) => ({ slotIndex, widgetType: null }))
};

type FetchMock = ReturnType<typeof mockApi>;

function layoutGetCalls(mockFetch: FetchMock) {
	const calls = mockFetch.mock.calls as unknown as [string, RequestInit][];
	return calls.filter(
		([url, options]) => url.includes('/api/dashboard/layout') && (options.method ?? 'GET') === 'GET'
	);
}

describe('Dashboard page layout re-fetch', () => {
	beforeEach(() => {
		localStorage.clear();
		MessageBus.clearAll();
	});

	afterEach(() => {
		localStorage.clear();
		MessageBus.clearAll();
	});

	it('loads the layout for the current user on mount', async () => {
		MessageBus.sendMessage(Messages.CurrentUser, 'user-a');
		const mockFetch = mockApi({ '/api/dashboard/layout': emptyLayout });

		const { getByTestId } = render(Page);

		await waitFor(() => expect(getByTestId('dashboard-page')).toBeInTheDocument());

		const calls = layoutGetCalls(mockFetch);
		expect(calls.length).toBe(1);
		expect(calls[0][1].headers).toMatchObject({ 'X-User-Id': 'user-a' });
	});

	it('re-fetches the layout when the current user changes', async () => {
		MessageBus.sendMessage(Messages.CurrentUser, 'user-a');
		const mockFetch = mockApi({ '/api/dashboard/layout': emptyLayout });

		const { getByTestId } = render(Page);

		await waitFor(() => expect(getByTestId('dashboard-page')).toBeInTheDocument());
		expect(layoutGetCalls(mockFetch).length).toBe(1);

		MessageBus.sendMessage(Messages.CurrentUser, 'user-b');

		await waitFor(() => expect(layoutGetCalls(mockFetch).length).toBe(2));
		const secondCall = layoutGetCalls(mockFetch)[1];
		expect(secondCall[1].headers).toMatchObject({ 'X-User-Id': 'user-b' });
	});

	it('does not fetch the layout when no current user is set', async () => {
		const mockFetch = mockApi({ '/api/dashboard/layout': emptyLayout });

		render(Page);

		await new Promise((resolve) => setTimeout(resolve, 50));

		expect(layoutGetCalls(mockFetch).length).toBe(0);
	});

	it('stops re-fetching after the page is destroyed', async () => {
		MessageBus.sendMessage(Messages.CurrentUser, 'user-a');
		const mockFetch = mockApi({ '/api/dashboard/layout': emptyLayout });

		const { getByTestId, unmount } = render(Page);

		await waitFor(() => expect(getByTestId('dashboard-page')).toBeInTheDocument());
		expect(layoutGetCalls(mockFetch).length).toBe(1);

		unmount();

		MessageBus.sendMessage(Messages.CurrentUser, 'user-b');

		await new Promise((resolve) => setTimeout(resolve, 50));

		expect(layoutGetCalls(mockFetch).length).toBe(1);
	});
});
