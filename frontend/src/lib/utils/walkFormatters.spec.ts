import { fmtDuration, relTime, weekdayTime } from './walkFormatters';

describe('fmtDuration', () => {
	it('returns "Ym" for durations under one hour', () => {
		expect(fmtDuration(0)).toBe('0m');
		expect(fmtDuration(59)).toBe('0m');
		expect(fmtDuration(60)).toBe('1m');
		expect(fmtDuration(2280)).toBe('38m');
		expect(fmtDuration(3599)).toBe('59m');
	});

	it('returns "Xh Ym" for durations of one hour or more', () => {
		expect(fmtDuration(3600)).toBe('1h 0m');
		expect(fmtDuration(3725)).toBe('1h 2m');
		expect(fmtDuration(7320)).toBe('2h 2m');
	});
});

describe('relTime', () => {
	const today = new Date('2026-05-30T12:00:00Z');

	it('returns "Today" for same calendar day', () => {
		expect(relTime('2026-05-30T07:42:00Z', today)).toBe('Today');
		expect(relTime('2026-05-30T23:59:00Z', today)).toBe('Today');
	});

	it('returns "Yesterday" for the previous calendar day', () => {
		expect(relTime('2026-05-29T18:10:00Z', today)).toBe('Yesterday');
	});

	it('returns "N days ago" for older dates', () => {
		expect(relTime('2026-05-28T08:05:00Z', today)).toBe('2 days ago');
		expect(relTime('2026-05-23T09:15:00Z', today)).toBe('7 days ago');
	});
});

describe('weekdayTime', () => {
	it('returns weekday abbreviation and time', () => {
		const result = weekdayTime('2026-05-30T07:42:00Z');
		expect(result).toMatch(/^[A-Za-z]{3} · /);
		expect(result).toMatch(/\d{1,2}:\d{2}/);
	});
});
