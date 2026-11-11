import '@testing-library/jest-dom/vitest';

// Node >= 23 defines a `localStorage` key on globalThis, which makes vitest's
// jsdom environment skip copying jsdom's own localStorage. In CI that leaves
// `localStorage` undefined. Provide an in-memory fallback so tests behave the
// same on every Node version.
function ambientLocalStorageIsUsable(): boolean {
	try {
		return typeof globalThis.localStorage?.clear === 'function';
	} catch {
		return false;
	}
}

if (!ambientLocalStorageIsUsable()) {
	const storage: Record<string, string> = {};

	const hide = (name: string, value: unknown) =>
		Object.defineProperty(storage, name, {
			value,
			enumerable: false,
			configurable: true,
			writable: true
		});

	hide('getItem', (key: string) => (key in storage ? storage[key] : null));
	hide('setItem', (key: string, value: unknown) => {
		storage[key] = String(value);
	});
	hide('removeItem', (key: string) => {
		delete storage[key];
	});
	hide('clear', () => {
		for (const key of Object.keys(storage)) {
			delete storage[key];
		}
	});
	hide('key', (index: number) => Object.keys(storage)[index] ?? null);
	Object.defineProperty(storage, 'length', {
		get: () => Object.keys(storage).length,
		enumerable: false,
		configurable: true
	});

	Object.defineProperty(globalThis, 'localStorage', {
		value: storage,
		configurable: true,
		writable: true
	});
}
