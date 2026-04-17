import type { APIRequest, IRequestMiddleware } from '$lib/api/middleware/IRequestMiddleware';
import ConfigService from '$lib/services/Config/ConfigService';

export default class UrlMiddleware implements IRequestMiddleware {
	async transformRequest(request: APIRequest): Promise<APIRequest> {
		request.url = this.getFullUrl(request.url);

		return request;
	}

	private getFullUrl(urlSuffix: string): string {
		const prefix = this.getBaseUrl();

		return `${this.stripTrailingSlash(prefix)}/${this.stripLeadingSlash(urlSuffix)}`;
	}

	private stripTrailingSlash(url: string): string {
		if (url.endsWith('/')) {
			return url.slice(0, url.length - 1);
		}

		return url;
	}

	private stripLeadingSlash(url: string): string {
		if (url.startsWith('/')) {
			return url.slice(1);
		}

		return url;
	}

	private getBaseUrl(): string {
		const service = new ConfigService();

		const baseUrl = service.getConfig<string>('baseUrl');

		if (!baseUrl || typeof baseUrl !== 'string') return '/';

		return baseUrl;
	}
}
