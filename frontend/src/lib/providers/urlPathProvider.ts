export interface IUrlPathProvider {
	path: string;
}

export default class UrlPathProvider {
	private static urlProvider: IUrlPathProvider;

	static initialize(provider: IUrlPathProvider) {
		this.urlProvider = provider;
	}

	static matchesPath(path: string): boolean {
		if (!this.urlProvider) return false;

		const trimmedPath = path.substring(1);

		const providerPath = this.urlProvider.path.substring(1);

		if (providerPath === '') return trimmedPath === '';

		return trimmedPath.includes(providerPath);
	}
}

export class TestUrlProvider implements IUrlPathProvider {
	path: string;

	constructor(path: string) {
		this.path = path;
	}
}

export class RealUrlProvider implements IUrlPathProvider {
	constructor() {
		this.path = window.location.pathname;
	}

	path: string;
}
