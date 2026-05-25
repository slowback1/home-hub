import { mockApi } from '$lib/testHelpers/getFetchMock';
import DashboardApi, { type LayoutResponse } from './DashboardApi';

const mockLayout: LayoutResponse = {
	layoutFormat: '3x2',
	slots: [
		{ slotIndex: 0, widgetType: 'tasks' },
		{ slotIndex: 1, widgetType: null },
		{ slotIndex: 2, widgetType: 'weather' },
		{ slotIndex: 3, widgetType: null },
		{ slotIndex: 4, widgetType: null },
		{ slotIndex: 5, widgetType: null }
	]
};

describe('DashboardApi', () => {
	it('getLayout calls GET /api/dashboard/layout', async () => {
		const mockFetch = mockApi({ '/api/dashboard/layout': mockLayout });
		const api = new DashboardApi();

		const result = await api.getLayout();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/dashboard/layout');
		expect(options.method).toEqual('GET');
		expect(result).toEqual(mockLayout);
	});

	it('saveLayout calls PUT /api/dashboard/layout with correct body', async () => {
		const mockFetch = mockApi({ '/api/dashboard/layout': {} });
		const api = new DashboardApi();

		const slots = [
			{ slotIndex: 0, widgetType: 'tasks' },
			{ slotIndex: 1, widgetType: null }
		];
		await api.saveLayout(slots);

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/dashboard/layout');
		expect(options.method).toEqual('PUT');
		expect(JSON.parse(options.body as string)).toEqual({
			layoutFormat: '3x2',
			slots
		});
	});
});
