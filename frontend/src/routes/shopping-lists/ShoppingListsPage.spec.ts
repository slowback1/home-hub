import { render, waitFor, fireEvent } from '@testing-library/svelte';
import { vi } from 'vitest';
import ShoppingListsPage from './+page.svelte';
import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
import type { ShoppingList, ShoppingListItem } from '$lib/api/ShoppingListApi';
import type { SavedItem } from '$lib/api/SavedItemsApi';

vi.mock('$lib/api/ShoppingListApi', () => ({ default: vi.fn() }));
vi.mock('$lib/api/SavedItemsApi', () => ({ default: vi.fn() }));
vi.mock('$lib/api/PremadeListsApi', () => ({ default: vi.fn() }));

import ShoppingListApi from '$lib/api/ShoppingListApi';
import SavedItemsApi from '$lib/api/SavedItemsApi';
import PremadeListsApi, { type PremadeList } from '$lib/api/PremadeListsApi';

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

function mockApi(
	lists: ShoppingList[],
	archived: ShoppingList[] = [],
	created: ShoppingList = makeList({ items: [] })
) {
	vi.mocked(ShoppingListApi).mockImplementation(
		() =>
			({
				listActive: vi.fn().mockResolvedValue(lists),
				listArchived: vi.fn().mockResolvedValue(archived),
				create: vi.fn().mockResolvedValue(created),
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
	FeatureFlagService.featureFlags = [
		{ name: 'SHOPPING_LISTS_ENABLED', isEnabled: true },
		{ name: 'SAVED_ITEMS_ENABLED', isEnabled: true }
	];
}

function disableShoppingListsFlag() {
	FeatureFlagService.featureFlags = [
		{ name: 'SHOPPING_LISTS_ENABLED', isEnabled: false },
		{ name: 'SAVED_ITEMS_ENABLED', isEnabled: true }
	];
}

function disableSavedItemsFlag() {
	FeatureFlagService.featureFlags = [
		{ name: 'SHOPPING_LISTS_ENABLED', isEnabled: true },
		{ name: 'SAVED_ITEMS_ENABLED', isEnabled: false }
	];
}

function makeSavedItem(overrides: Partial<SavedItem> = {}): SavedItem {
	return {
		id: 's1',
		userId: 'u1',
		name: 'Milk',
		createdAt: '2026-05-25T00:00:00Z',
		...overrides
	};
}

function mockSavedItemsApi(items: SavedItem[]) {
	vi.mocked(SavedItemsApi).mockImplementation(
		() => ({ list: vi.fn().mockResolvedValue(items) }) as never
	);
}

function getSavedItemsApiMock() {
	return vi.mocked(SavedItemsApi).mock.results[0].value as {
		list: ReturnType<typeof vi.fn>;
	};
}

function makePremadeList(overrides: Partial<PremadeList> = {}): PremadeList {
	return {
		id: 'p1',
		userId: 'u1',
		name: 'Weekly groceries',
		createdAt: '2026-05-25T00:00:00Z',
		savedItemIds: ['s1', 's2'],
		...overrides
	};
}

function mockPremadeListsApi(premadeLists: PremadeList[]) {
	vi.mocked(PremadeListsApi).mockImplementation(
		() => ({ list: vi.fn().mockResolvedValue(premadeLists) }) as never
	);
}

function enablePremadeListsFlag() {
	FeatureFlagService.featureFlags = [
		{ name: 'SHOPPING_LISTS_ENABLED', isEnabled: true },
		{ name: 'SAVED_ITEMS_ENABLED', isEnabled: true },
		{ name: 'PREMADE_LISTS_ENABLED', isEnabled: true }
	];
}

describe('Shopping lists page', () => {
	beforeEach(() => {
		vi.mocked(ShoppingListApi).mockClear();
		vi.mocked(SavedItemsApi).mockClear();
		enableShoppingListsFlag();
		mockSavedItemsApi([]);
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

	it('renders a premade-list picker when PREMADE_LISTS_ENABLED is on', async () => {
		enablePremadeListsFlag();
		mockPremadeListsApi([makePremadeList()]);
		mockApi([]);
		const { getByTestId } = render(ShoppingListsPage);

		await waitFor(() => expect(getByTestId('shopping-list-premade-picker')).toBeInTheDocument());
		const picker = () => getByTestId('shopping-list-premade-picker') as HTMLSelectElement;
		await waitFor(() => expect(picker().options.length).toEqual(2));
		expect(picker().value).toEqual('');
		expect(picker().options[0].textContent).toContain('None');
		expect(picker().options[1].textContent).toContain('Weekly groceries');
	});

	it('styles the premade-list picker with the app standard select styling', async () => {
		enablePremadeListsFlag();
		mockPremadeListsApi([makePremadeList()]);
		mockApi([]);
		const { getByTestId } = render(ShoppingListsPage);

		await waitFor(() => expect(getByTestId('shopping-list-premade-picker')).toBeInTheDocument());
		expect(getByTestId('shopping-list-premade-picker')).toHaveClass('select__input');
	});

	it('creates a list expanded from a chosen premade list', async () => {
		enablePremadeListsFlag();
		mockPremadeListsApi([makePremadeList()]);
		mockApi(
			[],
			[],
			makeList({
				id: 'l2',
				name: 'Trip',
				items: [makeItem({ id: 'i1', text: 'Milk' }), makeItem({ id: 'i2', text: 'Eggs' })]
			})
		);
		const { getByTestId } = render(ShoppingListsPage);
		await waitFor(() =>
			expect(
				(getByTestId('shopping-list-premade-picker') as HTMLSelectElement).options.length
			).toEqual(2)
		);

		await fireEvent.change(getByTestId('shopping-list-premade-picker'), {
			target: { value: 'p1' }
		});
		await fireEvent.input(getByTestId('shopping-list-form-name'), { target: { value: 'Trip' } });
		await fireEvent.click(getByTestId('shopping-list-form-add'));

		await waitFor(() => expect(getApiMock().create).toHaveBeenCalledWith('Trip', 'p1'));
		await waitFor(() => expect(getByTestId('shopping-list-row-Trip')).toBeInTheDocument());
		expect(getByTestId('shopping-item-Milk')).toBeInTheDocument();
		expect(getByTestId('shopping-item-Eggs')).toBeInTheDocument();
	});

	it('hides the premade-list picker when PREMADE_LISTS_ENABLED is off', async () => {
		mockPremadeListsApi([]);
		mockApi([]);
		const { getByTestId, queryByTestId } = render(ShoppingListsPage);
		await waitFor(() => expect(getByTestId('shopping-lists-empty-state')).toBeInTheDocument());

		expect(queryByTestId('shopping-list-premade-picker')).not.toBeInTheDocument();

		await fireEvent.input(getByTestId('shopping-list-form-name'), {
			target: { value: 'Groceries' }
		});
		await fireEvent.click(getByTestId('shopping-list-form-add'));

		await waitFor(() => expect(getApiMock().create).toHaveBeenCalledWith('Groceries'));
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

	it('renders archived lists read-only with items and archive date', async () => {
		mockApi(
			[],
			[
				makeList({
					id: 'l2',
					name: 'Camping',
					status: 'archived',
					archivedAt: '2026-06-01T10:00:00Z',
					items: [
						makeItem({ id: 'i3', listId: 'l2', text: 'Tent', isChecked: true }),
						makeItem({ id: 'i4', listId: 'l2', text: 'Fuel', isChecked: false })
					]
				})
			]
		);
		const { getByTestId, queryByTestId } = render(ShoppingListsPage);

		await waitFor(() => expect(getByTestId('shopping-archived-row-Camping')).toBeInTheDocument());
		expect(getByTestId('shopping-archived-item-Tent')).toHaveClass('shopping-item--checked');
		expect(getByTestId('shopping-archived-item-Fuel')).not.toHaveClass('shopping-item--checked');
		expect(getByTestId('shopping-archived-date-Camping')).toBeInTheDocument();

		expect(queryByTestId('shopping-item-input-Camping')).not.toBeInTheDocument();
		expect(queryByTestId('shopping-item-add-Camping')).not.toBeInTheDocument();
		expect(queryByTestId('shopping-list-complete-Camping')).not.toBeInTheDocument();
	});

	it('shows matching saved item suggestions while typing in the add-item input', async () => {
		mockApi([makeList({ items: [] })]);
		mockSavedItemsApi([makeSavedItem({ name: 'Milk' }), makeSavedItem({ id: 's2', name: 'Eggs' })]);
		const { getByTestId, queryByTestId } = render(ShoppingListsPage);
		await waitFor(() => expect(getByTestId('shopping-list-row-Groceries')).toBeInTheDocument());

		const input = getByTestId('shopping-item-input-Groceries');
		await fireEvent.input(input, { target: { value: 'mi' } });

		await waitFor(() => expect(getByTestId('saved-item-suggestion-Milk')).toBeInTheDocument());
		expect(queryByTestId('saved-item-suggestion-Eggs')).not.toBeInTheDocument();
	});

	it('adds a picked saved item to the list as a normal unchecked item', async () => {
		mockApi([makeList({ items: [] })]);
		mockSavedItemsApi([makeSavedItem({ name: 'Milk' })]);
		const { getByTestId } = render(ShoppingListsPage);
		await waitFor(() => expect(getByTestId('shopping-list-row-Groceries')).toBeInTheDocument());

		const input = getByTestId('shopping-item-input-Groceries');
		await fireEvent.input(input, { target: { value: 'mi' } });
		await waitFor(() => expect(getByTestId('saved-item-suggestion-Milk')).toBeInTheDocument());

		await fireEvent.click(getByTestId('saved-item-suggestion-Milk'));

		await waitFor(() => expect(getApiMock().addItem).toHaveBeenCalledWith('l1', 'Milk'));
		await waitFor(() => expect(getByTestId('shopping-item-Milk')).toBeInTheDocument());
		expect(getByTestId('shopping-item-Milk')).not.toHaveClass('shopping-item--checked');
	});

	it('renders a Saved Items link on the page when SAVED_ITEMS_ENABLED is on', async () => {
		mockApi([]);
		const { getByTestId } = render(ShoppingListsPage);
		await waitFor(() => expect(getByTestId('shopping-lists-empty-state')).toBeInTheDocument());

		const link = getByTestId('shopping-lists-saved-items-link');
		expect(link).toHaveAttribute('href', '/saved-items');
	});

	it('hides the Saved Items link when SAVED_ITEMS_ENABLED is off', async () => {
		disableSavedItemsFlag();
		mockApi([]);
		const { getByTestId, queryByTestId } = render(ShoppingListsPage);
		await waitFor(() => expect(getByTestId('shopping-lists-empty-state')).toBeInTheDocument());

		expect(queryByTestId('shopping-lists-saved-items-link')).not.toBeInTheDocument();
	});

	it('renders a Premade Lists link on the page when PREMADE_LISTS_ENABLED is on', async () => {
		enablePremadeListsFlag();
		mockPremadeListsApi([]);
		mockApi([]);
		const { getByTestId } = render(ShoppingListsPage);
		await waitFor(() => expect(getByTestId('shopping-lists-empty-state')).toBeInTheDocument());

		const link = getByTestId('shopping-lists-premade-lists-link');
		expect(link).toHaveAttribute('href', '/premade-lists');
	});

	it('hides the Premade Lists link when PREMADE_LISTS_ENABLED is off', async () => {
		mockApi([]);
		const { getByTestId, queryByTestId } = render(ShoppingListsPage);
		await waitFor(() => expect(getByTestId('shopping-lists-empty-state')).toBeInTheDocument());

		expect(queryByTestId('shopping-lists-premade-lists-link')).not.toBeInTheDocument();
	});

	it('does not fetch saved items or show suggestions when SAVED_ITEMS_ENABLED is off', async () => {
		disableSavedItemsFlag();
		mockApi([makeList({ items: [] })]);
		mockSavedItemsApi([makeSavedItem({ name: 'Milk' })]);
		const { getByTestId, queryByTestId } = render(ShoppingListsPage);
		await waitFor(() => expect(getByTestId('shopping-list-row-Groceries')).toBeInTheDocument());

		const input = getByTestId('shopping-item-input-Groceries');
		await fireEvent.input(input, { target: { value: 'mi' } });

		expect(getSavedItemsApiMock().list).not.toHaveBeenCalled();
		expect(queryByTestId('saved-item-suggestion-Milk')).not.toBeInTheDocument();

		await fireEvent.click(getByTestId('shopping-item-add-Groceries'));
		await waitFor(() => expect(getApiMock().addItem).toHaveBeenCalledWith('l1', 'mi'));
	});
});
