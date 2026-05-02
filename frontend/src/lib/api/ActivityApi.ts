import BaseApi from './baseApi';

export type Activity = {
	id: string;
	name: string;
	weight: number;
};

export default class ActivityApi extends BaseApi {
	constructor() {
		super();
	}

	async getAll(): Promise<Activity[]> {
		return this.Get<Activity[]>('/api/activities');
	}

	async create(name: string, weight: number): Promise<Activity> {
		return this.Post<Activity>('/api/activities', { name, weight });
	}

	async update(id: string, name: string, weight: number): Promise<Activity> {
		return this.Put<Activity>(`/api/activities/${id}`, { name, weight });
	}

	async delete(id: string): Promise<void> {
		return this.Delete<void>(`/api/activities/${id}`);
	}
}
