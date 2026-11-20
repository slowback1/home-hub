import { render, waitFor } from '@testing-library/svelte';
import { vi } from 'vitest';
import NbaPage from './+page.svelte';
import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
import type { NbaGame } from '$lib/api/NbaApi';

vi.mock('$lib/api/NbaApi', () => ({ default: vi.fn() }));

import NbaApi from '$lib/api/NbaApi';

function enableNbaFlag() {
	FeatureFlagService.featureFlags = [{ name: 'NBA_SCHEDULE_ENABLED', isEnabled: true }];
}

function disableNbaFlag() {
	FeatureFlagService.featureFlags = [{ name: 'NBA_SCHEDULE_ENABLED', isEnabled: false }];
}

function makeGame(overrides: Partial<NbaGame> = {}): NbaGame {
	return {
		id: 'g1',
		sourceGameId: 'src-1',
		startAt: new Date(Date.now() + 2 * 3_600_000).toISOString(),
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

describe('NBA page', () => {
	beforeEach(() => {
		vi.mocked(NbaApi).mockClear();
		vi.useFakeTimers();
		vi.setSystemTime(new Date('2026-11-20T15:00:00Z'));
		enableNbaFlag();
	});

	afterEach(() => {
		vi.useRealTimers();
	});

	it('shows disabled state when NBA_SCHEDULE_ENABLED flag is off', () => {
		disableNbaFlag();
		mockApi([]);
		const { getByTestId } = render(NbaPage);

		expect(getByTestId('nba-disabled')).toBeInTheDocument();
	});

	it('lists non-disappeared games including future days', async () => {
		const now = Date.now();
		mockApi([
			makeGame({
				id: 'today',
				homeTeam: 'Lakers',
				awayTeam: 'Celtics',
				startAt: new Date(now + 2 * 3_600_000).toISOString()
			}),
			makeGame({
				id: 'tomorrow',
				homeTeam: 'Bulls',
				awayTeam: 'Heat',
				startAt: new Date(now + 26 * 3_600_000).toISOString()
			})
		]);
		const { getAllByTestId } = render(NbaPage);

		await waitFor(() => expect(getAllByTestId('nba-game-row')).toHaveLength(2));
		const rows = getAllByTestId('nba-game-row').map((r) => r.textContent ?? '');
		expect(rows.join(' ')).toContain('Lakers @ Celtics');
		expect(rows.join(' ')).toContain('Bulls @ Heat');
	});

	it('hides games more than 3h past their scheduled start', async () => {
		const now = Date.now();
		mockApi([
			makeGame({
				id: 'ongoing',
				homeTeam: 'Bucks',
				awayTeam: 'Hawks',
				startAt: new Date(now - 2 * 3_600_000).toISOString()
			}),
			makeGame({
				id: 'gone',
				homeTeam: 'Suns',
				awayTeam: 'Mavs',
				startAt: new Date(now - 4 * 3_600_000).toISOString()
			})
		]);
		const { getAllByTestId, queryByText } = render(NbaPage);

		await waitFor(() => expect(getAllByTestId('nba-game-row')).toHaveLength(1));
		expect(queryByText(/Suns/)).not.toBeInTheDocument();
	});

	it('orders games chronologically regardless of input order', async () => {
		const now = Date.now();
		mockApi([
			makeGame({
				id: 'late',
				homeTeam: 'Bulls',
				awayTeam: 'Heat',
				startAt: new Date(now + 26 * 3_600_000).toISOString()
			}),
			makeGame({
				id: 'soon',
				homeTeam: 'Knicks',
				awayTeam: 'Nets',
				startAt: new Date(now + 1 * 3_600_000).toISOString()
			})
		]);
		const { getAllByTestId } = render(NbaPage);

		await waitFor(() => expect(getAllByTestId('nba-game-row')).toHaveLength(2));
		const rows = getAllByTestId('nba-game-row').map((r) => r.textContent ?? '');
		expect(rows[0]).toContain('Knicks @ Nets');
		expect(rows[1]).toContain('Bulls @ Heat');
	});

	it('shows an empty state when no games are visible', async () => {
		mockApi([]);
		const { queryByText } = render(NbaPage);

		await waitFor(() => expect(queryByText('No games')).toBeInTheDocument());
	});

	it('fetches a range covering the retention window', async () => {
		const mockGetSchedule = vi.fn().mockResolvedValue([]);
		vi.mocked(NbaApi).mockImplementation(() => ({ getSchedule: mockGetSchedule }) as never);

		render(NbaPage);

		await waitFor(() => expect(mockGetSchedule).toHaveBeenCalled());
		const [from, to] = mockGetSchedule.mock.calls[0];
		expect(from.getTime()).toBeLessThanOrEqual(Date.now() - 2 * 24 * 3_600_000);
		expect(to.getTime()).toBeGreaterThanOrEqual(Date.now() + 2 * 24 * 3_600_000);
	});
});
