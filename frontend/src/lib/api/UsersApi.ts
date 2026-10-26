import BaseApi from './baseApi';

export type AppUser = {
	id: string;
	name: string;
};

export default class UsersApi extends BaseApi {
	constructor() {
		super();
	}

	async getUsers(): Promise<AppUser[]> {
		return this.Get<AppUser[]>('/api/users');
	}
}
