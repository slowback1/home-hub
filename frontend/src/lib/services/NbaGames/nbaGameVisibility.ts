import type { NbaGame } from '$lib/api/NbaApi';

export const DISAPPEARANCE_HOURS = 3;
const MS_PER_HOUR = 3_600_000;

export function isDisappeared(startAt: string, now: Date): boolean {
	const cutoff = new Date(new Date(startAt).getTime() + DISAPPEARANCE_HOURS * MS_PER_HOUR);
	return cutoff <= now;
}

export function isTodayLocal(startAt: string, now: Date): boolean {
	const start = new Date(startAt);
	return (
		start.getFullYear() === now.getFullYear() &&
		start.getMonth() === now.getMonth() &&
		start.getDate() === now.getDate()
	);
}

export function visibleGames(
	games: NbaGame[],
	now: Date,
	options: { todayOnly?: boolean } = {}
): NbaGame[] {
	return games
		.filter((g) => !isDisappeared(g.startAt, now))
		.filter((g) => !options.todayOnly || isTodayLocal(g.startAt, now))
		.sort((a, b) => new Date(a.startAt).getTime() - new Date(b.startAt).getTime());
}

const timeFormatter = new Intl.DateTimeFormat(undefined, { timeStyle: 'short' });

export function formatStartLocal(startAt: string): string {
	return timeFormatter.format(new Date(startAt));
}
