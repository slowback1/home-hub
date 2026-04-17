import type { APIRequest, IRequestMiddleware } from '$lib/api/middleware/IRequestMiddleware';

export default class AuthorizationMiddleware implements IRequestMiddleware {
	async transformRequest(request: APIRequest): Promise<APIRequest> {
		request.headers['Authorization'] = this.getBearerToken();

		return request;
	}

	private getBearerToken() {
		//TO-DO: get your token from wherever you get your token, probably the message bus
		const token = '';

		return `Bearer ${token}`;
	}
}
