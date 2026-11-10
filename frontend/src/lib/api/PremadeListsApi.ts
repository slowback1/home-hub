import BaseApi from './baseApi';

export type PremadeList = {
	id: string;
	userId: string;
	name: string;
	createdAt: string;
	savedItemIds: string[];
};

export default class PremadeListsApi extends BaseApi {
	constructor() {
		super();
	}

	async list(): Promise<PremadeList[]> {
		return this.Get<PremadeList[]>('/api/premade-lists');
	}

	async create(name: string): Promise<PremadeList> {
		return this.Post<PremadeList>('/api/premade-lists', { name });
	}

	async delete(id: string): Promise<void> {
		return this.Delete<void>(`/api/premade-lists/${id}`);
	}

	async addItem(id: string, savedItemId: string): Promise<PremadeList> {
		return this.Post<PremadeList>(`/api/premade-lists/${id}/items`, { savedItemId });
	}

	async removeItem(id: string, savedItemId: string): Promise<void> {
		return this.Delete<void>(`/api/premade-lists/${id}/items/${savedItemId}`);
	}
}
