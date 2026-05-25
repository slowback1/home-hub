import { Page } from '@playwright/test';

export class BookmarksPage {
	constructor(readonly page: Page) {}

	async goto(): Promise<void> {
		await this.page.goto('/bookmarks');
		await this.page.waitForSelector('[data-testid="bookmarks-page"]');
	}

	async isCardVisible(name: string): Promise<boolean> {
		const card = this.page.locator(`[data-testid="bookmark-card"][data-bookmark-name="${name}"]`);
		return await card.isVisible();
	}

	async openAddModal(): Promise<void> {
		await this.page.click('[data-testid="add-bookmark-button"]');
		await this.page.waitForSelector('[data-testid="bookmark-modal"]');
	}

	async fillAndSubmitBookmark(url: string, name: string, description: string): Promise<void> {
		await this.page.fill('[data-testid="bookmark-url-input"]', url);
		if (name) await this.page.fill('[data-testid="bookmark-name-input"]', name);
		if (description) await this.page.fill('[data-testid="bookmark-description-input"]', description);
		await this.page.click('[data-testid="submit-bookmark-button"]');
		await this.page.waitForSelector('[data-testid="bookmark-modal"]', { state: 'detached' });
	}

	async addBookmarkByUrl(url: string): Promise<void> {
		await this.page.fill('[data-testid="bookmark-url-input"]', url);
		await this.page.click('[data-testid="submit-bookmark-button"]');
		await this.page.waitForSelector('[data-testid="bookmark-modal"]', { state: 'detached' });
	}

	async getCardUrl(name: string): Promise<string> {
		const card = this.page.locator(`[data-testid="bookmark-card"][data-bookmark-name="${name}"]`);
		return (await card.getAttribute('href')) ?? '';
	}

	async openEditModal(name: string): Promise<void> {
		const card = this.page.locator(`[data-testid="bookmark-card"][data-bookmark-name="${name}"]`);
		await card.locator('[data-testid="edit-bookmark-button"]').click();
		await this.page.waitForSelector('[data-testid="bookmark-modal"]');
	}

	async fillEditForm(name: string, description: string): Promise<void> {
		const nameInput = this.page.locator('[data-testid="bookmark-name-input"]');
		await nameInput.fill(name);
		const descInput = this.page.locator('[data-testid="bookmark-description-input"]');
		await descInput.fill(description);
	}

	async submitEditForm(): Promise<void> {
		await this.page.click('[data-testid="submit-bookmark-button"]');
		await this.page.waitForSelector('[data-testid="bookmark-modal"]', { state: 'detached' });
	}

	async clickDelete(name: string): Promise<void> {
		const card = this.page.locator(`[data-testid="bookmark-card"][data-bookmark-name="${name}"]`);
		await card.locator('[data-testid="delete-bookmark-button"]').click();
		await this.page.waitForSelector('[data-testid="delete-confirm-dialog"]');
	}

	async confirmDelete(): Promise<void> {
		await this.page.click('[data-testid="confirm-delete-button"]');
		await this.page.waitForSelector('[data-testid="delete-confirm-dialog"]', { state: 'detached' });
	}

	async cancelDelete(): Promise<void> {
		await this.page.click('[data-testid="cancel-delete-button"]');
		await this.page.waitForSelector('[data-testid="delete-confirm-dialog"]', { state: 'detached' });
	}

	async clickStar(name: string): Promise<void> {
		const card = this.page.locator(`[data-testid="bookmark-card"][data-bookmark-name="${name}"]`);
		await card.locator('[data-testid="star-button"]').click();
		await this.page.waitForTimeout(300);
	}

	async isCardStarred(name: string): Promise<boolean> {
		const card = this.page.locator(`[data-testid="bookmark-card"][data-bookmark-name="${name}"]`);
		const starBtn = card.locator('[data-testid="star-button"]');
		const label = await starBtn.getAttribute('aria-label');
		return label === 'Unstar';
	}

	async search(query: string): Promise<void> {
		await this.page.fill('[data-testid="bookmark-search"]', query);
	}

	async visibleCardCount(): Promise<number> {
		return await this.page.locator('[data-testid="bookmark-card"]').count();
	}

	async isEmptyStateVisible(): Promise<boolean> {
		return await this.page.locator('[data-testid="bookmarks-empty-state"]').isVisible();
	}
}
