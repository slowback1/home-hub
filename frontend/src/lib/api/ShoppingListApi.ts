import BaseApi from './baseApi';

export type ShoppingListStatus = 'active' | 'archived';

export type ShoppingListItem = {
	id: string;
	listId: string;
	text: string;
	isChecked: boolean;
	createdAt: string;
};

export type ShoppingList = {
	id: string;
	userId: string;
	name: string;
	status: ShoppingListStatus;
	archivedAt: string | null;
	createdAt: string;
	items: ShoppingListItem[];
};

export default class ShoppingListApi extends BaseApi {
	constructor() {
		super();
	}

	async listActive(): Promise<ShoppingList[]> {
		return this.Get<ShoppingList[]>('/api/shopping-lists');
	}

	async listArchived(): Promise<ShoppingList[]> {
		return this.Get<ShoppingList[]>('/api/shopping-lists', { status: 'archived' });
	}

	async create(name: string): Promise<ShoppingList> {
		return this.Post<ShoppingList>('/api/shopping-lists', { name });
	}

	async addItem(listId: string, text: string): Promise<ShoppingListItem> {
		return this.Post<ShoppingListItem>(`/api/shopping-lists/${listId}/items`, { text });
	}

	async toggleItem(listId: string, itemId: string): Promise<ShoppingListItem> {
		return this.Patch<ShoppingListItem>(`/api/shopping-lists/${listId}/items/${itemId}`, {});
	}

	async complete(listId: string): Promise<ShoppingList> {
		return this.Post<ShoppingList>(`/api/shopping-lists/${listId}/complete`, {});
	}

	async deleteList(listId: string): Promise<void> {
		return this.Delete<void>(`/api/shopping-lists/${listId}`);
	}
}
