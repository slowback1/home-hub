/* eslint-disable @typescript-eslint/no-explicit-any */
export function getFetchMock(response: any, status: number = 200) {
	const mock = vi.fn(() => {
		return Promise.resolve({
			json(): Promise<never> {
				return Promise.resolve(response) as any;
			},
			text(): Promise<string> {
				return Promise.resolve(response);
			},
			status: status
		} as never);
	});

	global.fetch = mock;

	return mock;
}

function normalizeRoute(route: string) {
	let normalized = route.toLowerCase();

	if (normalized.startsWith('/')) normalized = normalized.slice(1);
	if (normalized.endsWith('/')) normalized = normalized.slice(0, normalized.length - 1);

	return normalized;
}

function compareRoutes(sourceRoute: string, searchUrl: string) {
	const normalizedFirst = normalizeRoute(sourceRoute);
	const normalizedSecond = normalizeRoute(searchUrl);

	if (sourceRoute.includes('*')) return compareWildcards(sourceRoute, searchUrl);

	return normalizedFirst == normalizedSecond;
}

function compareWildcards(wildcard: string, search: string) {
	const beforeWildcard = wildcard.substring(0, wildcard.indexOf('*'));
	const equivalentSearchString = search.substring(0, wildcard.indexOf('*'));

	return compareRoutes(beforeWildcard, equivalentSearchString);
}

/* eslint-disable @typescript-eslint/no-explicit-any */
type MockApiMap = { [route: string]: any | { response: never; status: number } };

function getMatchedResponse(url: string, map: MockApiMap) {
	const lowerUrl = url.toLowerCase();

	const routes = Object.keys(map);

	const matchedRoute = routes.find((route) => compareRoutes(route, lowerUrl));
	return map[matchedRoute];
}

export function mockApi(map: MockApiMap) {
	const mock = vi.fn((url: string) => {
		const matchedResponse = getMatchedResponse(url, map);

		if (!matchedResponse) throw new Error(`Invalid URL: '${url}'`);

		let response = matchedResponse;
		let status = 200;

		const isStatusResponse = !!matchedResponse.status && !!matchedResponse.response;

		if (isStatusResponse) {
			response = matchedResponse.response;
			status = matchedResponse.status;
		}

		return Promise.resolve({
			json(): Promise<any> {
				return Promise.resolve(response);
			},
			text(): Promise<string> {
				return Promise.resolve(response);
			},
			status: status
		});
	});

	global.fetch = mock as any;

	return mock;
}
