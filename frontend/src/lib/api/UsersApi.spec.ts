import { mockApi } from '$lib/testHelpers/getFetchMock';
import UsersApi, { type AppUser } from './UsersApi';

const mockUsers: AppUser[] = [
	{ id: 'u1', name: 'Alex' },
	{ id: 'u2', name: 'Sam' }
];

describe('UsersApi', () => {
	it('getUsers calls GET /api/users and returns the users', async () => {
		const mockFetch = mockApi({ '/api/users': mockUsers });
		const api = new UsersApi();

		const result = await api.getUsers();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/users');
		expect(options.method).toEqual('GET');
		expect(result).toEqual(mockUsers);
	});

	it('getUsers returns empty array when no users exist', async () => {
		const mockFetch = mockApi({ '/api/users': [] });
		const api = new UsersApi();

		const result = await api.getUsers();

		expect(result).toEqual([]);
		expect(mockFetch).toHaveBeenCalledTimes(1);
	});
});
