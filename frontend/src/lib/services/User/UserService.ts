import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';
import UsersApi, { type AppUser } from '$lib/api/UsersApi';

export default class UserService {
	private static users: AppUser[] = [];
	private static currentUserId: string | null = null;

	static async initialize(): Promise<void> {
		this.users = await new UsersApi().getUsers();

		MessageBus.subscribe<string>(Messages.CurrentUser, (value) => {
			const normalized = value === null || value === undefined ? null : String(value);
			const isValid = !!normalized && this.users.some((user) => user.id === normalized);

			this.currentUserId = isValid ? normalized : null;
		});
	}

	getCurrentUserId(): string | null {
		return UserService.currentUserId;
	}

	getUsers(): AppUser[] {
		return UserService.users;
	}
}
