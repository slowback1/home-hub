import { mockApi } from '$lib/testHelpers/getFetchMock';
import WalkSessionApi, { type WalkSession } from './WalkSessionApi';

const mockSession: WalkSession = {
	id: 'abc-123',
	clientId: 'device-1',
	startedAt: '2026-05-30T07:42:00Z',
	durationSeconds: 2280,
	stepCount: 4820,
	syncedAt: '2026-05-30T08:00:00Z'
};

describe('WalkSessionApi', () => {
	it('listSessions calls GET /api/walk-sessions and returns WalkSession[]', async () => {
		const mockFetch = mockApi({ '/api/walk-sessions': [mockSession] });
		const api = new WalkSessionApi();

		const result = await api.listSessions();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/walk-sessions');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([mockSession]);
	});

	it('listSessions returns empty array when no sessions exist', async () => {
		mockApi({ '/api/walk-sessions': [] });
		const api = new WalkSessionApi();

		const result = await api.listSessions();

		expect(result).toEqual([]);
	});
});
