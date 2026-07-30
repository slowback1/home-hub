/**
 * Parse a newline-delimited wheel `items` string into a clean list:
 * each line trimmed, blank lines dropped. Duplicates are preserved
 * (they legitimately bias the odds).
 */
export function parseItems(items: string): string[] {
	return items
		.split(/\r?\n/)
		.map((item) => item.trim())
		.filter((item) => item.length > 0);
}

/**
 * Pick one element from a list uniformly at random. Returns null for an
 * empty list. The random source is injectable for testability.
 */
export function pickRandom<T>(list: T[], random: () => number = Math.random): T | null {
	if (list.length === 0) return null;
	const index = Math.floor(random() * list.length);
	return list[Math.min(index, list.length - 1)];
}
