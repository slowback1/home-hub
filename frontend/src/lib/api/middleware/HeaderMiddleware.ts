import type { APIRequest, IRequestMiddleware } from '$lib/api/middleware/IRequestMiddleware';

export default class HeaderMiddleware implements IRequestMiddleware {
	async transformRequest(request: APIRequest): Promise<APIRequest> {
		request.headers['Content-Type'] = 'application/json';

		return request;
	}
}
