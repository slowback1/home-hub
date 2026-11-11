describe('test environment localStorage', () => {
	it('provides a working localStorage', () => {
		expect(localStorage).toBeDefined();
		localStorage.clear();
		localStorage.setItem('env-key', 'env-value');
		expect(localStorage.getItem('env-key')).toBe('env-value');
		localStorage.removeItem('env-key');
		expect(localStorage.getItem('env-key')).toBeNull();
	});

	it('exposes stored keys via Object.keys', () => {
		localStorage.clear();
		localStorage.setItem('a', '1');
		localStorage.setItem('b', '2');
		expect(Object.keys(localStorage)).toEqual(['a', 'b']);
		localStorage.clear();
		expect(Object.keys(localStorage)).toEqual([]);
	});
});
