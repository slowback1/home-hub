import { render, waitFor } from '@testing-library/svelte';
import { afterEach, beforeEach } from 'vitest';
import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';
import { mockApi } from '$lib/testHelpers/getFetchMock';
import UserService from '$lib/services/User/UserService';
import Page from './+page.svelte';

const emptyLayout = {
	layoutFormat: '3x2',
	slots: [0, 1, 2, 3, 4, 5].map((slotIndex) => ({ slotIndex, widgetType: null }))
};

const mockUsers = [
	{ id: 'user-a', name: 'Alex' },
	{ id: 'user-b', name: 'Sam' }
];

async function resolveUser(userIds: string) {
	mockApi({ '/api/users': mockUsers });
	await UserService.initialize();
	MessageBus.sendMessage(Messages.CurrentUser, userIds);
}

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
		await resolveUser('user-a');
		const mockFetch = mockApi({ '/api/dashboard/layout': emptyLayout });

		const { getByTestId } = render(Page);

		await waitFor(() => expect(getByTestId('dashboard-page')).toBeInTheDocument());

		const calls = layoutGetCalls(mockFetch);
		expect(calls.length).toBe(1);
		expect(calls[0][1].headers).toMatchObject({ 'X-User-Id': 'user-a' });
	});

	it('re-fetches the layout when the current user changes', async () => {
		await resolveUser('user-a');
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
		mockApi({ '/api/users': mockUsers });
		await UserService.initialize();
		const mockFetch = mockApi({ '/api/dashboard/layout': emptyLayout });

		render(Page);

		await new Promise((resolve) => setTimeout(resolve, 50));

		expect(layoutGetCalls(mockFetch).length).toBe(0);
	});

	it('does not fetch the layout or render widgets until the user id is resolved', async () => {
		mockApi({ '/api/users': mockUsers });
		await UserService.initialize();
		MessageBus.sendMessage(Messages.CurrentUser, 'bogus-user');
		const mockFetch = mockApi({ '/api/dashboard/layout': emptyLayout });

		const { container } = render(Page);

		await new Promise((resolve) => setTimeout(resolve, 50));

		expect(layoutGetCalls(mockFetch).length).toBe(0);
		expect(container.querySelector('[data-testid="dashboard-page"]')).toBeNull();
	});

	it('stops re-fetching after the page is destroyed', async () => {
		await resolveUser('user-a');
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
