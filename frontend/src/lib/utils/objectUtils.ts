export function clone<T>(obj: T): T {
	if (obj === undefined) return undefined;

	return JSON.parse(JSON.stringify(obj));
}
