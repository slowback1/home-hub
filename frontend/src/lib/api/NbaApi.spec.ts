import { mockApi } from '$lib/testHelpers/getFetchMock';
import NbaApi, { type NbaGame } from './NbaApi';

const mockGame: NbaGame = {
	id: 'g1',
	sourceGameId: 'src-1',
	startAt: '2026-11-20T19:30:00Z',
	homeTeam: 'Lakers',
	awayTeam: 'Celtics'
};

describe('NbaApi', () => {
	it('getSchedule with no range calls GET /api/nba/schedule and returns games', async () => {
		const mockFetch = mockApi({ '/api/nba/schedule': [mockGame] });
		const api = new NbaApi();

		const result = await api.getSchedule();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/nba/schedule');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([mockGame]);
	});

	it('getSchedule serializes from and to as UTC ISO strings in the query string', async () => {
		const from = new Date('2026-11-20T05:00:00Z');
		const to = new Date('2026-11-21T05:00:00Z');
		const mockFetch = mockApi({ '/api/nba/schedule*': [mockGame] });
		const api = new NbaApi();

		await api.getSchedule(from, to);

		const [url] = mockFetch.mock.calls[0] as never as [string];
		const query = new URL(url, 'http://localhost').searchParams;
		expect(query.get('from')).toEqual('2026-11-20T05:00:00.000Z');
		expect(query.get('to')).toEqual('2026-11-21T05:00:00.000Z');
	});

	it('getSchedule with only from sends only the from parameter', async () => {
		const from = new Date('2026-11-19T05:00:00Z');
		const mockFetch = mockApi({ '/api/nba/schedule*': [mockGame] });
		const api = new NbaApi();

		await api.getSchedule(from);

		const [url] = mockFetch.mock.calls[0] as never as [string];
		const query = new URL(url, 'http://localhost').searchParams;
		expect(query.get('from')).toEqual('2026-11-19T05:00:00.000Z');
		expect(query.has('to')).toBe(false);
	});
});
