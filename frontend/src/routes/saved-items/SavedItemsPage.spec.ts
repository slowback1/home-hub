import { render, waitFor, fireEvent } from '@testing-library/svelte';
import { vi } from 'vitest';
import SavedItemsPage from './+page.svelte';
import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
import type { SavedItem } from '$lib/api/SavedItemsApi';

vi.mock('$lib/api/SavedItemsApi', () => ({ default: vi.fn() }));

import SavedItemsApi from '$lib/api/SavedItemsApi';

function makeItem(overrides: Partial<SavedItem> = {}): SavedItem {
	return {
		id: 's1',
		userId: 'u1',
		name: '5lb sack of potatoes',
		createdAt: '2026-05-25T00:00:00Z',
		...overrides
	};
}

function mockApi(items: SavedItem[]) {
	vi.mocked(SavedItemsApi).mockImplementation(
		() =>
			({
				list: vi.fn().mockResolvedValue(items),
				create: vi
					.fn()
					.mockImplementation((name: string) => Promise.resolve(makeItem({ id: 'new-1', name }))),
				update: vi
					.fn()
					.mockImplementation((id: string, name: string) =>
						Promise.resolve(makeItem({ id, name }))
					),
				delete: vi.fn()
			}) as never
	);
}

function getApiMock() {
	return vi.mocked(SavedItemsApi).mock.results[0].value as {
		list: ReturnType<typeof vi.fn>;
		create: ReturnType<typeof vi.fn>;
		update: ReturnType<typeof vi.fn>;
	};
}

function enableSavedItemsFlag() {
	FeatureFlagService.featureFlags = [{ name: 'SAVED_ITEMS_ENABLED', isEnabled: true }];
}

function disableSavedItemsFlag() {
	FeatureFlagService.featureFlags = [{ name: 'SAVED_ITEMS_ENABLED', isEnabled: false }];
}

describe('Saved items page', () => {
	beforeEach(() => {
		vi.mocked(SavedItemsApi).mockClear();
		enableSavedItemsFlag();
	});

	it('shows disabled state when SAVED_ITEMS_ENABLED flag is off', () => {
		disableSavedItemsFlag();
		mockApi([]);
		const { getByTestId } = render(SavedItemsPage);

		expect(getByTestId('saved-items-disabled')).toBeInTheDocument();
	});

	it('shows the empty state when there are no saved items', async () => {
		mockApi([]);
		const { getByTestId } = render(SavedItemsPage);

		await waitFor(() => expect(getByTestId('saved-items-empty-state')).toBeInTheDocument());
	});

	it('renders the saved items', async () => {
		mockApi([
			makeItem({ id: 's1', name: '5lb sack of potatoes' }),
			makeItem({ id: 's2', name: 'Sourdough loaf' })
		]);
		const { getByTestId } = render(SavedItemsPage);

		await waitFor(() =>
			expect(getByTestId('saved-item-row-5lb sack of potatoes')).toBeInTheDocument()
		);
		expect(getByTestId('saved-item-row-Sourdough loaf')).toBeInTheDocument();
	});

	it('creates a saved item', async () => {
		mockApi([]);
		const { getByTestId } = render(SavedItemsPage);
		await waitFor(() => expect(getByTestId('saved-items-empty-state')).toBeInTheDocument());

		const input = getByTestId('saved-item-form-name');
		await fireEvent.input(input, { target: { value: '5lb sack of potatoes' } });
		await fireEvent.click(getByTestId('saved-item-form-add'));

		await waitFor(() => expect(getApiMock().create).toHaveBeenCalledWith('5lb sack of potatoes'));
		await waitFor(() =>
			expect(getByTestId('saved-item-row-5lb sack of potatoes')).toBeInTheDocument()
		);
	});

	it('edits a saved item name', async () => {
		mockApi([makeItem({ id: 's1', name: '5lb sack of potatoes' })]);
		const { getByTestId } = render(SavedItemsPage);
		await waitFor(() =>
			expect(getByTestId('saved-item-row-5lb sack of potatoes')).toBeInTheDocument()
		);

		await fireEvent.click(getByTestId('saved-item-edit-5lb sack of potatoes'));

		const input = getByTestId('saved-item-edit-input-5lb sack of potatoes');
		await fireEvent.input(input, { target: { value: '10lb sack of potatoes' } });
		await fireEvent.click(getByTestId('saved-item-save-5lb sack of potatoes'));

		await waitFor(() =>
			expect(getApiMock().update).toHaveBeenCalledWith('s1', '10lb sack of potatoes')
		);
		await waitFor(() =>
			expect(getByTestId('saved-item-row-10lb sack of potatoes')).toBeInTheDocument()
		);
	});
});
