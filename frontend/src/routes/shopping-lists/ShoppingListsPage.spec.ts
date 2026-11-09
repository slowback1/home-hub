import { render, waitFor, fireEvent } from '@testing-library/svelte';
import { vi } from 'vitest';
import ShoppingListsPage from './+page.svelte';
import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
import type { ShoppingList, ShoppingListItem } from '$lib/api/ShoppingListApi';

vi.mock('$lib/api/ShoppingListApi', () => ({ default: vi.fn() }));

import ShoppingListApi from '$lib/api/ShoppingListApi';

function makeItem(overrides: Partial<ShoppingListItem> = {}): ShoppingListItem {
	return {
		id: 'i1',
		listId: 'l1',
		text: 'Milk',
		isChecked: false,
		createdAt: '2026-05-25T00:00:00Z',
		...overrides
	};
}

function makeList(overrides: Partial<ShoppingList> = {}): ShoppingList {
	return {
		id: 'l1',
		userId: 'u1',
		name: 'Groceries',
		status: 'active',
		archivedAt: null,
		createdAt: '2026-05-25T00:00:00Z',
		items: [makeItem()],
		...overrides
	};
}

function mockApi(lists: ShoppingList[]) {
	vi.mocked(ShoppingListApi).mockImplementation(
		() =>
			({
				listActive: vi.fn().mockResolvedValue(lists),
				listArchived: vi.fn().mockResolvedValue([]),
				create: vi.fn().mockResolvedValue(makeList({ items: [] })),
				addItem: vi
					.fn()
					.mockImplementation((listId: string, text: string) =>
						Promise.resolve(makeItem({ listId, text }))
					),
				toggleItem: vi
					.fn()
					.mockImplementation((_listId: string, itemId: string) =>
						Promise.resolve(makeItem({ id: itemId, isChecked: true }))
					),
				complete: vi.fn().mockResolvedValue(makeList({ status: 'archived' })),
				deleteList: vi.fn()
			}) as never
	);
}

function getApiMock() {
	return vi.mocked(ShoppingListApi).mock.results[0].value as {
		listActive: ReturnType<typeof vi.fn>;
		create: ReturnType<typeof vi.fn>;
		addItem: ReturnType<typeof vi.fn>;
		toggleItem: ReturnType<typeof vi.fn>;
		complete: ReturnType<typeof vi.fn>;
	};
}

function enableShoppingListsFlag() {
	FeatureFlagService.featureFlags = [{ name: 'SHOPPING_LISTS_ENABLED', isEnabled: true }];
}

function disableShoppingListsFlag() {
	FeatureFlagService.featureFlags = [{ name: 'SHOPPING_LISTS_ENABLED', isEnabled: false }];
}

describe('Shopping lists page', () => {
	beforeEach(() => {
		vi.mocked(ShoppingListApi).mockClear();
		enableShoppingListsFlag();
	});

	it('shows disabled state when SHOPPING_LISTS_ENABLED flag is off', () => {
		disableShoppingListsFlag();
		mockApi([]);
		const { getByTestId } = render(ShoppingListsPage);

		expect(getByTestId('shopping-lists-disabled')).toBeInTheDocument();
	});

	it('shows the empty state when there are no lists', async () => {
		mockApi([]);
		const { getByTestId } = render(ShoppingListsPage);

		await waitFor(() => expect(getByTestId('shopping-lists-empty-state')).toBeInTheDocument());
	});

	it('renders active lists with their items', async () => {
		mockApi([
			makeList({ items: [makeItem({ text: 'Milk' }), makeItem({ id: 'i2', text: 'Eggs' })] })
		]);
		const { getByTestId } = render(ShoppingListsPage);

		await waitFor(() => expect(getByTestId('shopping-list-row-Groceries')).toBeInTheDocument());
		expect(getByTestId('shopping-item-Milk')).toBeInTheDocument();
		expect(getByTestId('shopping-item-Eggs')).toBeInTheDocument();
	});

	it('creates a named list', async () => {
		mockApi([]);
		const { getByTestId } = render(ShoppingListsPage);
		await waitFor(() => expect(getByTestId('shopping-lists-empty-state')).toBeInTheDocument());

		const input = getByTestId('shopping-list-form-name');
		await fireEvent.input(input, { target: { value: 'Groceries' } });
		await fireEvent.click(getByTestId('shopping-list-form-add'));

		await waitFor(() => expect(getApiMock().create).toHaveBeenCalledWith('Groceries'));
		await waitFor(() => expect(getByTestId('shopping-list-row-Groceries')).toBeInTheDocument());
	});

	it('adds a free-text item to a list, rendered unchecked', async () => {
		mockApi([makeList({ items: [] })]);
		const { getByTestId } = render(ShoppingListsPage);
		await waitFor(() => expect(getByTestId('shopping-list-row-Groceries')).toBeInTheDocument());

		const input = getByTestId('shopping-item-input-Groceries');
		await fireEvent.input(input, { target: { value: 'Milk' } });
		await fireEvent.click(getByTestId('shopping-item-add-Groceries'));

		await waitFor(() => expect(getApiMock().addItem).toHaveBeenCalledWith('l1', 'Milk'));
		await waitFor(() => expect(getByTestId('shopping-item-Milk')).toBeInTheDocument());
		expect(getByTestId('shopping-item-Milk')).not.toHaveClass('shopping-item--checked');
	});

	it('toggles an item checked and keeps it visible crossed out', async () => {
		mockApi([makeList()]);
		const { getByTestId } = render(ShoppingListsPage);
		await waitFor(() => expect(getByTestId('shopping-item-Milk')).toBeInTheDocument());

		await fireEvent.click(getByTestId('shopping-item-Milk'));

		await waitFor(() => expect(getApiMock().toggleItem).toHaveBeenCalledWith('l1', 'i1'));
		await waitFor(() =>
			expect(getByTestId('shopping-item-Milk')).toHaveClass('shopping-item--checked')
		);
	});

	it('completes a list, removing it from the active view', async () => {
		mockApi([makeList()]);
		const { getByTestId, queryByTestId } = render(ShoppingListsPage);
		await waitFor(() => expect(getByTestId('shopping-list-row-Groceries')).toBeInTheDocument());

		await fireEvent.click(getByTestId('shopping-list-complete-Groceries'));

		await waitFor(() => expect(getApiMock().complete).toHaveBeenCalledWith('l1'));
		await waitFor(() =>
			expect(queryByTestId('shopping-list-row-Groceries')).not.toBeInTheDocument()
		);
	});
});
