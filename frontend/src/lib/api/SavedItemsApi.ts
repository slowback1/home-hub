import BaseApi from './baseApi';

export type SavedItem = {
	id: string;
	userId: string;
	name: string;
	createdAt: string;
};

export default class SavedItemsApi extends BaseApi {
	constructor() {
		super();
	}

	async list(): Promise<SavedItem[]> {
		return this.Get<SavedItem[]>('/api/saved-items');
	}

	async create(name: string): Promise<SavedItem> {
		return this.Post<SavedItem>('/api/saved-items', { name });
	}

	async update(id: string, name: string): Promise<SavedItem> {
		return this.Put<SavedItem>(`/api/saved-items/${id}`, { name });
	}

	async delete(id: string): Promise<void> {
		return this.Delete<void>(`/api/saved-items/${id}`);
	}
}
