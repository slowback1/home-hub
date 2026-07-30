import { mockApi } from '$lib/testHelpers/getFetchMock';
import WheelApi, { type Wheel } from './WheelApi';

const mockWheel: Wheel = {
	id: '1',
	name: 'Dinner',
	items: 'Pizza\nTacos\nSushi',
	createdAt: '2026-07-29T12:00:00Z'
};

describe('WheelApi', () => {
	it('getAll calls GET /api/wheels and returns Wheel[]', async () => {
		const mockFetch = mockApi({ '/api/wheels': [mockWheel] });
		const api = new WheelApi();

		const result = await api.getAll();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/wheels');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([mockWheel]);
	});

	it('getAll returns empty array when no wheels exist', async () => {
		mockApi({ '/api/wheels': [] });
		const api = new WheelApi();

		const result = await api.getAll();

		expect(result).toEqual([]);
	});

	it('create calls POST /api/wheels with name and items', async () => {
		const mockFetch = mockApi({ '/api/wheels': mockWheel });
		const api = new WheelApi();

		const result = await api.create('Dinner', 'Pizza\nTacos\nSushi');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/wheels');
		expect(options.method).toEqual('POST');
		expect(JSON.parse(options.body as string)).toEqual({
			name: 'Dinner',
			items: 'Pizza\nTacos\nSushi'
		});
		expect(result).toEqual(mockWheel);
	});

	it('update calls PUT /api/wheels/{id} with name and items', async () => {
		const updated = { ...mockWheel, name: 'Dinner Options', items: 'Pizza\nRamen' };
		const mockFetch = mockApi({ '/api/wheels/1': updated });
		const api = new WheelApi();

		const result = await api.update('1', 'Dinner Options', 'Pizza\nRamen');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/wheels/1');
		expect(options.method).toEqual('PUT');
		expect(JSON.parse(options.body as string)).toEqual({
			name: 'Dinner Options',
			items: 'Pizza\nRamen'
		});
		expect(result).toEqual(updated);
	});

	it('delete calls DELETE /api/wheels/{id}', async () => {
		const mockFetch = mockApi({ '/api/wheels/1': {} });
		const api = new WheelApi();

		await api.delete('1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/wheels/1');
		expect(options.method).toEqual('DELETE');
	});
});
