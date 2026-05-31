const SECONDS_PER_MINUTE = 60;
const MINUTES_PER_HOUR = 60;
const MS_PER_DAY = 86_400_000;

export function fmtDuration(seconds: number): string {
	const totalMinutes = Math.floor(seconds / SECONDS_PER_MINUTE);
	const hours = Math.floor(totalMinutes / MINUTES_PER_HOUR);
	const minutes = totalMinutes % MINUTES_PER_HOUR;
	return hours > 0 ? `${hours}h ${minutes}m` : `${minutes}m`;
}

function startOfDay(d: Date): Date {
	return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

export function relTime(iso: string, now: Date = new Date()): string {
	const days = Math.round(
		(startOfDay(now).getTime() - startOfDay(new Date(iso)).getTime()) / MS_PER_DAY
	);
	if (days <= 0) return 'Today';
	if (days === 1) return 'Yesterday';
	return `${days} days ago`;
}

export function weekdayTime(iso: string): string {
	const d = new Date(iso);
	const wd = d.toLocaleDateString('en-US', { weekday: 'short' });
	const t = d.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit' });
	return `${wd} · ${t}`;
}
