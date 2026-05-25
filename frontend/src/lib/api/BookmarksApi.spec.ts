import { mockApi } from '$lib/testHelpers/getFetchMock';
import BookmarksApi, { type Bookmark } from './BookmarksApi';

const mockBookmark: Bookmark = {
	id: 'b1',
	name: 'GitHub',
	url: 'https://github.com',
	description: 'Code hosting.',
	starred: false,
	createdAt: '2026-05-25T00:00:00Z'
};

describe('BookmarksApi', () => {
	it('listBookmarks calls GET /api/bookmarks', async () => {
		const mockFetch = mockApi({ '/api/bookmarks': [mockBookmark] });
		const api = new BookmarksApi();

		const result = await api.listBookmarks();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/bookmarks');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([mockBookmark]);
	});

	it('createBookmark calls POST /api/bookmarks with correct body', async () => {
		const mockFetch = mockApi({ '/api/bookmarks': mockBookmark });
		const api = new BookmarksApi();

		const result = await api.createBookmark({
			url: 'https://github.com',
			name: 'GitHub',
			description: 'Code hosting.'
		});

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/bookmarks');
		expect(options.method).toEqual('POST');
		expect(JSON.parse(options.body as string)).toEqual({
			url: 'https://github.com',
			name: 'GitHub',
			description: 'Code hosting.'
		});
		expect(result).toEqual(mockBookmark);
	});

	it('updateBookmark calls PUT /api/bookmarks/{id}', async () => {
		const updated = { ...mockBookmark, name: 'GitHub (updated)' };
		const mockFetch = mockApi({ '/api/bookmarks/b1': updated });
		const api = new BookmarksApi();

		const result = await api.updateBookmark('b1', {
			url: 'https://github.com',
			name: 'GitHub (updated)',
			description: null
		});

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/bookmarks/b1');
		expect(options.method).toEqual('PUT');
		expect(result).toEqual(updated);
	});

	it('deleteBookmark calls DELETE /api/bookmarks/{id}', async () => {
		const mockFetch = mockApi({ '/api/bookmarks/b1': {} });
		const api = new BookmarksApi();

		await api.deleteBookmark('b1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/bookmarks/b1');
		expect(options.method).toEqual('DELETE');
	});

	it('toggleStar calls PATCH /api/bookmarks/{id}/star', async () => {
		const starred = { ...mockBookmark, starred: true };
		const mockFetch = mockApi({ '/api/bookmarks/b1/star': starred });
		const api = new BookmarksApi();

		const result = await api.toggleStar('b1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/bookmarks/b1/star');
		expect(options.method).toEqual('PATCH');
		expect(result).toEqual(starred);
	});
});
