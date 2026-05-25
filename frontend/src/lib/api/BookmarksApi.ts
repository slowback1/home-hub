import BaseApi from './baseApi';

export type Bookmark = {
	id: string;
	name: string;
	url: string;
	description: string | null;
	starred: boolean;
	createdAt: string;
};

export type CreateBookmarkRequest = {
	url: string;
	name: string;
	description: string | null;
};

export type UpdateBookmarkRequest = {
	url: string;
	name: string;
	description: string | null;
};

export default class BookmarksApi extends BaseApi {
	constructor() {
		super();
	}

	async listBookmarks(): Promise<Bookmark[]> {
		return this.Get<Bookmark[]>('/api/bookmarks');
	}

	async createBookmark(req: CreateBookmarkRequest): Promise<Bookmark> {
		return this.Post<Bookmark>('/api/bookmarks', req);
	}

	async updateBookmark(id: string, req: UpdateBookmarkRequest): Promise<Bookmark> {
		return this.Put<Bookmark>(`/api/bookmarks/${id}`, req);
	}

	async deleteBookmark(id: string): Promise<void> {
		return this.Delete<void>(`/api/bookmarks/${id}`);
	}

	async toggleStar(id: string): Promise<Bookmark> {
		return this.Patch<Bookmark>(`/api/bookmarks/${id}/star`, {});
	}
}
