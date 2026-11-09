import { mockApi } from '$lib/testHelpers/getFetchMock';
import ShoppingListApi, { type ShoppingList, type ShoppingListItem } from './ShoppingListApi';

const mockItem: ShoppingListItem = {
	id: 'i1',
	listId: 'l1',
	text: 'Milk',
	isChecked: false,
	createdAt: '2026-05-25T00:00:00Z'
};

const mockList: ShoppingList = {
	id: 'l1',
	userId: 'u1',
	name: 'Groceries',
	status: 'active',
	archivedAt: null,
	createdAt: '2026-05-25T00:00:00Z',
	items: [mockItem]
};

describe('ShoppingListApi', () => {
	it('listActive calls GET /api/shopping-lists', async () => {
		const mockFetch = mockApi({ '/api/shopping-lists': [mockList] });
		const api = new ShoppingListApi();

		const result = await api.listActive();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/shopping-lists');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([mockList]);
	});

	it('listArchived calls GET /api/shopping-lists?status=archived', async () => {
		const archived = {
			...mockList,
			status: 'archived' as const,
			archivedAt: '2026-05-26T00:00:00Z'
		};
		const mockFetch = mockApi({ '/api/shopping-lists?status=archived': [archived] });
		const api = new ShoppingListApi();

		const result = await api.listArchived();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/shopping-lists?status=archived');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([archived]);
	});

	it('create calls POST /api/shopping-lists with correct body', async () => {
		const mockFetch = mockApi({ '/api/shopping-lists': mockList });
		const api = new ShoppingListApi();

		const result = await api.create('Groceries');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/shopping-lists');
		expect(options.method).toEqual('POST');
		expect(JSON.parse(options.body as string)).toEqual({ name: 'Groceries' });
		expect(result).toEqual(mockList);
	});

	it('addItem calls POST /api/shopping-lists/{id}/items with correct body', async () => {
		const mockFetch = mockApi({ '/api/shopping-lists/l1/items': mockItem });
		const api = new ShoppingListApi();

		const result = await api.addItem('l1', 'Milk');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/shopping-lists/l1/items');
		expect(options.method).toEqual('POST');
		expect(JSON.parse(options.body as string)).toEqual({ text: 'Milk' });
		expect(result).toEqual(mockItem);
	});

	it('toggleItem calls PATCH /api/shopping-lists/{id}/items/{itemId}', async () => {
		const checked = { ...mockItem, isChecked: true };
		const mockFetch = mockApi({ '/api/shopping-lists/l1/items/i1': checked });
		const api = new ShoppingListApi();

		const result = await api.toggleItem('l1', 'i1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/shopping-lists/l1/items/i1');
		expect(options.method).toEqual('PATCH');
		expect(result).toEqual(checked);
	});

	it('complete calls POST /api/shopping-lists/{id}/complete', async () => {
		const archived = {
			...mockList,
			status: 'archived' as const,
			archivedAt: '2026-05-26T00:00:00Z'
		};
		const mockFetch = mockApi({ '/api/shopping-lists/l1/complete': archived });
		const api = new ShoppingListApi();

		const result = await api.complete('l1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/shopping-lists/l1/complete');
		expect(options.method).toEqual('POST');
		expect(result).toEqual(archived);
	});

	it('deleteList calls DELETE /api/shopping-lists/{id}', async () => {
		const mockFetch = mockApi({ '/api/shopping-lists/l1': {} });
		const api = new ShoppingListApi();

		await api.deleteList('l1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/shopping-lists/l1');
		expect(options.method).toEqual('DELETE');
	});
});
