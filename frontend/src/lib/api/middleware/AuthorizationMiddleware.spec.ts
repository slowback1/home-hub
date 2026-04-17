import AuthorizationMiddleware from '$lib/api/middleware/AuthorizationMiddleware';
import { getTestAPIRequest } from '$lib/testHelpers/testData/testAPIRequest';

describe('AuthenticationMiddleware', () => {
	it("appends a bearer token to the request's headers", async () => {
		const middleware = new AuthorizationMiddleware();

		let request = getTestAPIRequest();

		request = await middleware.transformRequest(request);

		expect(request.headers['Authorization']).toEqual('Bearer ');
	});
});
