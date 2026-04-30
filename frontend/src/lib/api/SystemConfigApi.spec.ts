import { mockApi } from '$lib/testHelpers/getFetchMock';
import SystemConfigApi from './SystemConfigApi';

const mockEntry = {
	id: 'weather::zip_code',
	namespace: 'weather',
	key: 'zip_code',
	value: '10001',
	type: '',
	isSecret: false
};

describe('SystemConfigApi', () => {
	it('getAll calls GET /api/system-config and returns SystemConfig[]', async () => {
		const mockFetch = mockApi({ '/api/system-config': [mockEntry] });
		const api = new SystemConfigApi();

		const result = await api.getAll();

		expect(mockFetch).toHaveBeenCalled();
		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/system-config');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([mockEntry]);
	});

	it('update calls PUT /api/system-config/{namespace}/{key} with value body', async () => {
		const updated = { ...mockEntry, value: '90210' };
		const mockFetch = mockApi({ '/api/system-config/weather/zip_code': updated });
		const api = new SystemConfigApi();

		const result = await api.update('weather', 'zip_code', '90210');

		expect(mockFetch).toHaveBeenCalled();
		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/system-config/weather/zip_code');
		expect(options.method).toEqual('PUT');
		expect(JSON.parse(options.body as string)).toEqual({ value: '90210' });
		expect(result).toEqual(updated);
	});
});
