import { render, waitFor } from '@testing-library/svelte';
import { vi } from 'vitest';
import NbaGamesWidget from './NbaGamesWidget.svelte';

vi.mock('$lib/api/NbaApi', () => ({ default: vi.fn() }));

import NbaApi, { type NbaGame } from '$lib/api/NbaApi';

function localToday(hour: number, minute = 0): string {
	const now = new Date();
	return new Date(now.getFullYear(), now.getMonth(), now.getDate(), hour, minute, 0).toISOString();
}

function hoursAgo(hours: number): string {
	return new Date(Date.now() - hours * 3_600_000).toISOString();
}

function hoursFromNow(hours: number): string {
	return new Date(Date.now() + hours * 3_600_000).toISOString();
}

function makeGame(overrides: Partial<NbaGame> = {}): NbaGame {
	return {
		id: 'g1',
		sourceGameId: 'src-1',
		startAt: localToday(19, 30),
		homeTeam: 'Lakers',
		awayTeam: 'Celtics',
		...overrides
	};
}

function mockApi(games: NbaGame[]) {
	vi.mocked(NbaApi).mockImplementation(
		() => ({ getSchedule: vi.fn().mockResolvedValue(games) }) as never
	);
}

describe('NbaGamesWidget', () => {
	beforeEach(() => {
		vi.mocked(NbaApi).mockClear();
		vi.useFakeTimers();
		vi.setSystemTime(new Date('2026-11-20T15:00:00Z'));
	});

	afterEach(() => {
		vi.useRealTimers();
	});

	it('shows a loading state while fetching', async () => {
		vi.mocked(NbaApi).mockImplementation(
			() => ({ getSchedule: () => new Promise(() => {}) }) as never
		);
		const { getByRole } = render(NbaGamesWidget);

		expect(getByRole('status').textContent).toContain('Loading…');
	});

	it('shows an Unavailable error state when the fetch fails', async () => {
		vi.mocked(NbaApi).mockImplementation(
			() => ({ getSchedule: vi.fn().mockRejectedValue(new Error('boom')) }) as never
		);
		const { getByRole } = render(NbaGamesWidget);

		await waitFor(() => expect(getByRole('alert').textContent).toContain('Unavailable'));
	});

	it('shows a No games today empty state when no games are visible', async () => {
		mockApi([]);
		const { queryByText } = render(NbaGamesWidget);

		await waitFor(() => expect(queryByText('No games today')).toBeInTheDocument());
	});

	it('shows a No games today empty state when only non-today games are returned', async () => {
		mockApi([makeGame({ startAt: hoursFromNow(30) })]);
		const { queryByText, queryAllByTestId } = render(NbaGamesWidget);

		await waitFor(() => expect(queryByText('No games today')).toBeInTheDocument());
		expect(queryAllByTestId('nba-game-row')).toHaveLength(0);
	});

	it('renders today games as team names with a local start time', async () => {
		mockApi([
			makeGame({ id: 'g1', homeTeam: 'Lakers', awayTeam: 'Celtics', startAt: localToday(19, 30) })
		]);
		const { getAllByTestId } = render(NbaGamesWidget);

		await waitFor(() => expect(getAllByTestId('nba-game-row')).toHaveLength(1));
		const row = getAllByTestId('nba-game-row')[0].textContent ?? '';
		expect(row).toContain('Lakers @ Celtics');
		expect(row).toContain('7:30');
	});

	it('orders games chronologically by start time', async () => {
		mockApi([
			makeGame({ id: 'late', startAt: localToday(22, 0), homeTeam: 'Bulls', awayTeam: 'Heat' }),
			makeGame({ id: 'early', startAt: localToday(17, 0), homeTeam: 'Knicks', awayTeam: 'Nets' })
		]);
		const { getAllByTestId } = render(NbaGamesWidget);

		await waitFor(() => expect(getAllByTestId('nba-game-row')).toHaveLength(2));
		const rows = getAllByTestId('nba-game-row').map((r) => r.textContent ?? '');
		expect(rows[0]).toContain('Knicks @ Nets');
		expect(rows[1]).toContain('Bulls @ Heat');
	});

	it('hides games more than 3h past their scheduled start', async () => {
		mockApi([
			makeGame({ id: 'ongoing', startAt: hoursAgo(2), homeTeam: 'Bucks', awayTeam: 'Hawks' }),
			makeGame({ id: 'gone', startAt: hoursAgo(4), homeTeam: 'Suns', awayTeam: 'Mavs' })
		]);
		const { getAllByTestId, queryByText } = render(NbaGamesWidget);

		await waitFor(() => expect(getAllByTestId('nba-game-row')).toHaveLength(1));
		expect(queryByText(/Suns/)).not.toBeInTheDocument();
	});

	it('hides games outside the local calendar day', async () => {
		const now = new Date();
		mockApi([
			makeGame({
				id: 'tonight',
				startAt: localToday(21, 0),
				homeTeam: 'Warriors',
				awayTeam: 'Kings'
			}),
			makeGame({
				id: 'tomorrow',
				startAt: new Date(
					now.getFullYear(),
					now.getMonth(),
					now.getDate() + 1,
					19,
					30,
					0
				).toISOString(),
				homeTeam: 'Nuggets',
				awayTeam: 'Timberwolves'
			})
		]);
		const { getAllByTestId, queryByText } = render(NbaGamesWidget);

		await waitFor(() => expect(getAllByTestId('nba-game-row')).toHaveLength(1));
		expect(queryByText(/Nuggets/)).not.toBeInTheDocument();
	});

	it('wraps the game list in a scroll container', async () => {
		mockApi([makeGame()]);
		const { getByTestId } = render(NbaGamesWidget);

		await waitFor(() => expect(getByTestId('nba-games-scroll')).toBeInTheDocument());
	});
});
