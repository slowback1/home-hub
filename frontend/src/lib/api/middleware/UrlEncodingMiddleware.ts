import type { APIRequest, IRequestMiddleware } from '$lib/api/middleware/IRequestMiddleware';

export default class UrlEncodingMiddleware implements IRequestMiddleware {
	transformRequest(request: APIRequest): Promise<APIRequest> {
		if (this.hasQueryParameters(request)) {
			request.url = this.appendQueryParametersToUrl(request);
		}

		return Promise.resolve(request);
	}

	private hasQueryParameters(request: APIRequest): boolean {
		return !!request.queryParameters && Object.keys(request.queryParameters).length > 0;
	}

	private appendQueryParametersToUrl(request: APIRequest): string {
		const queryString = new URLSearchParams(request.queryParameters).toString();
		return `${request.url}?${queryString}`;
	}
}
