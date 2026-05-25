import { Page } from '@playwright/test';

export class BookmarksPage {
	constructor(readonly page: Page) {}

	async goto(): Promise<void> {
		throw new Error('not implemented');
	}

	async isCardVisible(name: string): Promise<boolean> {
		throw new Error('not implemented');
	}

	async openAddModal(): Promise<void> {
		throw new Error('not implemented');
	}

	async fillAndSubmitBookmark(url: string, name: string, description: string): Promise<void> {
		throw new Error('not implemented');
	}

	async addBookmarkByUrl(url: string): Promise<void> {
		throw new Error('not implemented');
	}

	async getCardUrl(name: string): Promise<string> {
		throw new Error('not implemented');
	}

	async openEditModal(name: string): Promise<void> {
		throw new Error('not implemented');
	}

	async fillEditForm(name: string, description: string): Promise<void> {
		throw new Error('not implemented');
	}

	async submitEditForm(): Promise<void> {
		throw new Error('not implemented');
	}

	async clickDelete(name: string): Promise<void> {
		throw new Error('not implemented');
	}

	async confirmDelete(): Promise<void> {
		throw new Error('not implemented');
	}

	async cancelDelete(): Promise<void> {
		throw new Error('not implemented');
	}

	async clickStar(name: string): Promise<void> {
		throw new Error('not implemented');
	}

	async isCardStarred(name: string): Promise<boolean> {
		throw new Error('not implemented');
	}

	async search(query: string): Promise<void> {
		throw new Error('not implemented');
	}

	async visibleCardCount(): Promise<number> {
		throw new Error('not implemented');
	}

	async isEmptyStateVisible(): Promise<boolean> {
		throw new Error('not implemented');
	}
}
