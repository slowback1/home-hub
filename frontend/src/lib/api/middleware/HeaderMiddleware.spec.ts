import HeaderMiddleware from '$lib/api/middleware/HeaderMiddleware';
import { getTestAPIRequest } from '$lib/testHelpers/testData/testAPIRequest';

describe('HeaderMiddleware', () => {
	it("should inject a 'Content-Type' header", async () => {
		const middleware = new HeaderMiddleware();

		let request = getTestAPIRequest();

		request = await middleware.transformRequest(request);

		expect(request.headers['Content-Type']).toEqual('application/json');
	});
});
