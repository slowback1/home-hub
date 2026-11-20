import type { NbaGame } from '$lib/api/NbaApi';
import { formatStartLocal, isDisappeared, isTodayLocal, visibleGames } from './nbaGameVisibility';

function game(id: string, startAt: Date, homeTeam = 'Lakers', awayTeam = 'Celtics'): NbaGame {
	return {
		id,
		sourceGameId: `src-${id}`,
		startAt: startAt.toISOString(),
		homeTeam,
		awayTeam
	};
}

function localNoonToday(now: Date): string {
	return new Date(now.getFullYear(), now.getMonth(), now.getDate(), 12, 0, 0).toISOString();
}

describe('isTodayLocal', () => {
	it('is true for a game starting at local noon today', () => {
		const now = new Date();
		expect(isTodayLocal(localNoonToday(now), now)).toBe(true);
	});

	it('is true for a game late tonight even when its UTC date differs from the local date', () => {
		const now = new Date();
		const lateTonight = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 23, 30, 0);
		expect(isTodayLocal(lateTonight.toISOString(), now)).toBe(true);
	});

	it('is true for a game just after local midnight today', () => {
		const now = new Date();
		const justAfterMidnight = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 30, 0);
		expect(isTodayLocal(justAfterMidnight.toISOString(), now)).toBe(true);
	});

	it('is false for a game late last night', () => {
		const now = new Date();
		const lastNight = new Date(now.getFullYear(), now.getMonth(), now.getDate() - 1, 23, 30, 0);
		expect(isTodayLocal(lastNight.toISOString(), now)).toBe(false);
	});

	it('is false for a game tomorrow', () => {
		const now = new Date();
		const tomorrow = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1, 19, 30, 0);
		expect(isTodayLocal(tomorrow.toISOString(), now)).toBe(false);
	});
});

describe('isDisappeared', () => {
	it('is false for a game 2h past its scheduled start', () => {
		const now = new Date();
		const start = new Date(now.getTime() - 2 * 60 * 60_000).toISOString();
		expect(isDisappeared(start, now)).toBe(false);
	});

	it('is true for a game exactly 3h past its scheduled start', () => {
		const now = new Date();
		const start = new Date(now.getTime() - 3 * 60 * 60_000).toISOString();
		expect(isDisappeared(start, now)).toBe(true);
	});

	it('is true for a game 4h past its scheduled start', () => {
		const now = new Date();
		const start = new Date(now.getTime() - 4 * 60 * 60_000).toISOString();
		expect(isDisappeared(start, now)).toBe(true);
	});

	it('is false for a game that has not started yet', () => {
		const now = new Date();
		const start = new Date(now.getTime() + 2 * 60 * 60_000).toISOString();
		expect(isDisappeared(start, now)).toBe(false);
	});
});

describe('visibleGames', () => {
	it('drops disappeared games and keeps the rest', () => {
		const now = new Date();
		const games = [
			game('old', new Date(now.getTime() - 4 * 60 * 60_000)),
			game('recent', new Date(now.getTime() - 2 * 60 * 60_000)),
			game('future', new Date(now.getTime() + 3 * 60 * 60_000))
		];

		const result = visibleGames(games, now);

		expect(result.map((g) => g.id)).toEqual(['recent', 'future']);
	});

	it('with todayOnly keeps only games in the local calendar day', () => {
		const now = new Date();
		const games = [
			game('tonight', new Date(now.getFullYear(), now.getMonth(), now.getDate(), 23, 30, 0)),
			game('lastnight', new Date(now.getFullYear(), now.getMonth(), now.getDate() - 1, 20, 0, 0)),
			game('tomorrow', new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1, 19, 30, 0))
		];

		const result = visibleGames(games, now, { todayOnly: true });

		expect(result.map((g) => g.id)).toEqual(['tonight']);
	});

	it('sorts chronologically regardless of input order', () => {
		const now = new Date();
		const games = [
			game('late', new Date(now.getTime() + 5 * 60 * 60_000)),
			game('soon', new Date(now.getTime() + 1 * 60 * 60_000)),
			game('ongoing', new Date(now.getTime() - 1 * 60 * 60_000))
		];

		const result = visibleGames(games, now);

		expect(result.map((g) => g.id)).toEqual(['ongoing', 'soon', 'late']);
	});
});

describe('formatStartLocal', () => {
	it('formats a start time in the browser-local timezone', () => {
		const now = new Date();
		const local730pm = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 19, 30, 0);

		const formatted = formatStartLocal(local730pm.toISOString());

		expect(formatted).toContain('7:30');
		expect(formatted).toMatch(/AM|PM/);
	});
});
