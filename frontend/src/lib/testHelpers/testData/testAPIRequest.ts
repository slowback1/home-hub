import type { APIRequest } from '$lib/api/middleware/IRequestMiddleware';

export function getTestAPIRequest(overrides: Partial<APIRequest> = {}): APIRequest {
	return {
		url: 'url',
		method: 'GET',
		headers: {},
		body: undefined,
		...overrides
	};
}
