import type IStorageProvider from './IStorageProvider';
import LocalStorageProvider from '$lib/bus/providers/localStorageProvider';

describe('getLocalStorageProvider', () => {
	let storageProvider: IStorageProvider;

	beforeEach(() => {
		localStorage.clear();
		storageProvider = new LocalStorageProvider();
	});

	test('should get an item from localStorage', () => {
		localStorage.setItem('key1', 'value1');
		expect(storageProvider.getItem('key1')).toBe('value1');
	});

	test('should return undefined for non-existing item', () => {
		expect(storageProvider.getItem('nonExistingKey')).toBeNull();
	});

	test('should set an item in localStorage', () => {
		storageProvider.setItem('key2', 'value2');
		expect(localStorage.getItem('key2')).toBe('value2');
	});

	test('should get the entire store from localStorage', () => {
		localStorage.setItem('key1', 'value1');
		localStorage.setItem('key2', 'value2');
		const store = storageProvider.getStore();
		expect(store).toEqual({
			key1: 'value1',
			key2: 'value2'
		});
	});
});
