import { beforeEach } from 'vitest';
import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';
import UserMiddleware from './UserMiddleware';
import { getTestAPIRequest } from '$lib/testHelpers/testData/testAPIRequest';

describe('UserMiddleware', () => {
	beforeEach(() => {
		MessageBus.clearAll();
	});

	it('injects the X-User-Id header when a user is selected', async () => {
		MessageBus.sendMessage(Messages.CurrentUser, 'u2');

		const middleware = new UserMiddleware();
		const request = await middleware.transformRequest(getTestAPIRequest());

		expect(request.headers['X-User-Id']).toEqual('u2');
	});

	it('omits the X-User-Id header when no user is selected', async () => {
		const middleware = new UserMiddleware();
		const request = await middleware.transformRequest(getTestAPIRequest());

		expect(request.headers['X-User-Id']).toBeUndefined();
	});
});
