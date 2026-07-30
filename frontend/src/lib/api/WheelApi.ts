import BaseApi from './baseApi';

export type Wheel = {
	id: string;
	name: string;
	items: string;
	createdAt: string;
};

export default class WheelApi extends BaseApi {
	constructor() {
		super();
	}

	async getAll(): Promise<Wheel[]> {
		return this.Get<Wheel[]>('/api/wheels');
	}

	async create(name: string, items: string): Promise<Wheel> {
		return this.Post<Wheel>('/api/wheels', { name, items });
	}

	async update(id: string, name: string, items: string): Promise<Wheel> {
		return this.Put<Wheel>(`/api/wheels/${id}`, { name, items });
	}

	async delete(id: string): Promise<void> {
		return this.Delete<void>(`/api/wheels/${id}`);
	}
}
