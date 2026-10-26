import { beforeEach } from 'vitest';
import { mockApi } from '$lib/testHelpers/getFetchMock';
import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';
import UserService from './UserService';
import getLocalStorageMock from '$lib/testHelpers/localStorageMock';

const mockUsers = [
	{ id: 'u1', name: 'Alex' },
	{ id: 'u2', name: 'Sam' }
];

describe('UserService', () => {
	beforeEach(() => {
		MessageBus.clearAll();
	});

	it('returns null when no user is selected', () => {
		const service = new UserService();

		expect(service.getCurrentUserId()).toBeNull();
	});

	it('returns the selected user id after a selection is sent', () => {
		mockApi({ '/api/users': mockUsers });

		return UserService.initialize().then(() => {
			MessageBus.sendMessage(Messages.CurrentUser, 'u2');

			const service = new UserService();

			expect(service.getCurrentUserId()).toEqual('u2');
		});
	});

	it('returns null when the stored id is not in the users list', () => {
		MessageBus.sendMessage(Messages.CurrentUser, 'bogus-id');
		mockApi({ '/api/users': mockUsers });

		return UserService.initialize().then(() => {
			const service = new UserService();

			expect(service.getCurrentUserId()).toBeNull();
		});
	});

	it('persists the selection to the storage provider', async () => {
		const storage = getLocalStorageMock();
		MessageBus.initialize(storage);
		mockApi({ '/api/users': mockUsers });

		await UserService.initialize();

		MessageBus.sendMessage(Messages.CurrentUser, 'u1');

		expect(storage.setItem).toHaveBeenCalledWith(Messages.CurrentUser, 'u1');

		const service = new UserService();
		expect(service.getCurrentUserId()).toEqual('u1');
	});

	it('validates a numeric-string id rehydrated from storage as a number', async () => {
		const storage = getLocalStorageMock();
		storage.setItem(Messages.CurrentUser, '2');
		MessageBus.initialize(storage);
		mockApi({ '/api/users': [{ id: '2', name: 'Sam' }] });

		await UserService.initialize();

		const service = new UserService();
		expect(service.getCurrentUserId()).toEqual('2');
	});
});
