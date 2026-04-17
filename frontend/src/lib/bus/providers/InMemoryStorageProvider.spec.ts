import InMemoryStorageProvider from '$lib/bus/providers/inMemoryStorageProvider';
import { beforeEach } from 'vitest';

describe('InMemoryStorageProvider', () => {
	let storageProvider: InMemoryStorageProvider;

	beforeEach(() => {
		storageProvider = new InMemoryStorageProvider();
	});

	it('should set an item', () => {
		storageProvider.setItem('key', 'value');
		expect(storageProvider.getItem('key')).toBe('value');
	});

	it("should return undefined if the item doesn't exist", () => {
		expect(storageProvider.getItem('key')).toBeUndefined();
	});

	it('should return the store', () => {
		storageProvider.setItem('key', 'value');
		expect(storageProvider.getStore()).toEqual({ key: 'value' });
	});

	it('can store multiple items', () => {
		storageProvider.setItem('key1', 'value1');
		storageProvider.setItem('key2', 'value2');

		expect(storageProvider.getStore()).toEqual({ key1: 'value1', key2: 'value2' });
	});
});
