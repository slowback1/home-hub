import { mockApi } from '$lib/testHelpers/getFetchMock';
import SavedItemsApi, { type SavedItem } from './SavedItemsApi';

const mockItem: SavedItem = {
	id: 's1',
	userId: 'u1',
	name: '5lb sack of potatoes',
	createdAt: '2026-05-25T00:00:00Z'
};

describe('SavedItemsApi', () => {
	it('list calls GET /api/saved-items', async () => {
		const mockFetch = mockApi({ '/api/saved-items': [mockItem] });
		const api = new SavedItemsApi();

		const result = await api.list();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/saved-items');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([mockItem]);
	});

	it('create calls POST /api/saved-items with correct body', async () => {
		const mockFetch = mockApi({ '/api/saved-items': mockItem });
		const api = new SavedItemsApi();

		const result = await api.create('5lb sack of potatoes');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/saved-items');
		expect(options.method).toEqual('POST');
		expect(JSON.parse(options.body as string)).toEqual({ name: '5lb sack of potatoes' });
		expect(result).toEqual(mockItem);
	});

	it('update calls PUT /api/saved-items/{id} with correct body', async () => {
		const updated = { ...mockItem, name: '10lb sack of potatoes' };
		const mockFetch = mockApi({ '/api/saved-items/s1': updated });
		const api = new SavedItemsApi();

		const result = await api.update('s1', '10lb sack of potatoes');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/saved-items/s1');
		expect(options.method).toEqual('PUT');
		expect(JSON.parse(options.body as string)).toEqual({ name: '10lb sack of potatoes' });
		expect(result).toEqual(updated);
	});

	it('delete calls DELETE /api/saved-items/{id}', async () => {
		const mockFetch = mockApi({ '/api/saved-items/s1': {} });
		const api = new SavedItemsApi();

		await api.delete('s1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/saved-items/s1');
		expect(options.method).toEqual('DELETE');
	});
});
