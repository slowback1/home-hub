import { mockApi } from '$lib/testHelpers/getFetchMock';
import PremadeListsApi, { type PremadeList } from './PremadeListsApi';

const mockList: PremadeList = {
	id: 'p1',
	userId: 'u1',
	name: 'Weekly groceries',
	createdAt: '2026-05-25T00:00:00Z',
	savedItemIds: ['s1', 's2']
};

describe('PremadeListsApi', () => {
	it('list calls GET /api/premade-lists', async () => {
		const mockFetch = mockApi({ '/api/premade-lists': [mockList] });
		const api = new PremadeListsApi();

		const result = await api.list();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/premade-lists');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([mockList]);
	});

	it('create calls POST /api/premade-lists with correct body', async () => {
		const created = { ...mockList, savedItemIds: [] };
		const mockFetch = mockApi({ '/api/premade-lists': created });
		const api = new PremadeListsApi();

		const result = await api.create('Weekly groceries');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/premade-lists');
		expect(options.method).toEqual('POST');
		expect(JSON.parse(options.body as string)).toEqual({ name: 'Weekly groceries' });
		expect(result).toEqual(created);
	});

	it('delete calls DELETE /api/premade-lists/{id}', async () => {
		const mockFetch = mockApi({ '/api/premade-lists/p1': {} });
		const api = new PremadeListsApi();

		await api.delete('p1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/premade-lists/p1');
		expect(options.method).toEqual('DELETE');
	});

	it('addItem calls POST /api/premade-lists/{id}/items with correct body', async () => {
		const updated = { ...mockList, savedItemIds: ['s1', 's2', 's3'] };
		const mockFetch = mockApi({ '/api/premade-lists/p1/items': updated });
		const api = new PremadeListsApi();

		const result = await api.addItem('p1', 's3');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/premade-lists/p1/items');
		expect(options.method).toEqual('POST');
		expect(JSON.parse(options.body as string)).toEqual({ savedItemId: 's3' });
		expect(result).toEqual(updated);
	});

	it('removeItem calls DELETE /api/premade-lists/{id}/items/{savedItemId}', async () => {
		const mockFetch = mockApi({ '/api/premade-lists/p1/items/s1': {} });
		const api = new PremadeListsApi();

		await api.removeItem('p1', 's1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/premade-lists/p1/items/s1');
		expect(options.method).toEqual('DELETE');
	});
});
