import { mockApi } from '$lib/testHelpers/getFetchMock';
import ActivityPickApi from './ActivityPickApi';

const mockPick = { id: '1', activityName: 'Play Chess', pickedAt: '2026-05-02T14:00:00Z' };

describe('ActivityPickApi', () => {
	it('getCurrent calls GET /api/activity/current and returns ActivityPick', async () => {
		const mockFetch = mockApi({ '/api/activity/current': mockPick });
		const api = new ActivityPickApi();

		const result = await api.getCurrent();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/activity/current');
		expect(options.method).toEqual('GET');
		expect(result).toEqual(mockPick);
	});

	it('getHistory calls GET /api/activity/history and returns ActivityPick[]', async () => {
		const mockFetch = mockApi({ '/api/activity/history': [mockPick] });
		const api = new ActivityPickApi();

		const result = await api.getHistory();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/activity/history');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([mockPick]);
	});
});
