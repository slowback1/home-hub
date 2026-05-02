import { mockApi } from '$lib/testHelpers/getFetchMock';
import ActivityApi from './ActivityApi';

const mockActivity = { id: '1', name: 'Play Chess', weight: 3 };

describe('ActivityApi', () => {
	it('getAll calls GET /api/activities and returns Activity[]', async () => {
		const mockFetch = mockApi({ '/api/activities': [mockActivity] });
		const api = new ActivityApi();

		const result = await api.getAll();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/activities');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([mockActivity]);
	});

	it('create calls POST /api/activities with name and weight', async () => {
		const mockFetch = mockApi({ '/api/activities': mockActivity });
		const api = new ActivityApi();

		const result = await api.create('Play Chess', 3);

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/activities');
		expect(options.method).toEqual('POST');
		expect(JSON.parse(options.body as string)).toEqual({ name: 'Play Chess', weight: 3 });
		expect(result).toEqual(mockActivity);
	});

	it('updateWeight calls PUT /api/activities/{id} with name and weight', async () => {
		const updated = { ...mockActivity, weight: 5 };
		const mockFetch = mockApi({ '/api/activities/1': updated });
		const api = new ActivityApi();

		const result = await api.update('1', 'Play Chess', 5);

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/activities/1');
		expect(options.method).toEqual('PUT');
		expect(JSON.parse(options.body as string)).toEqual({ name: 'Play Chess', weight: 5 });
		expect(result).toEqual(updated);
	});

	it('delete calls DELETE /api/activities/{id}', async () => {
		const mockFetch = mockApi({ '/api/activities/1': {} });
		const api = new ActivityApi();

		await api.delete('1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/activities/1');
		expect(options.method).toEqual('DELETE');
	});
});
