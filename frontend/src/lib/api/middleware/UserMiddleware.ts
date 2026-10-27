import type { APIRequest, IRequestMiddleware } from '$lib/api/middleware/IRequestMiddleware';
import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';

export default class UserMiddleware implements IRequestMiddleware {
	async transformRequest(request: APIRequest): Promise<APIRequest> {
		const userId = MessageBus.getLastMessage<string>(Messages.CurrentUser);

		if (userId) {
			request.headers['X-User-Id'] = userId;
		}

		return request;
	}
}
