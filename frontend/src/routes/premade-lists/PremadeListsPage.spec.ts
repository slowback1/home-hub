import { render, waitFor, fireEvent } from '@testing-library/svelte';
import { vi } from 'vitest';
import PremadeListsPage from './+page.svelte';
import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
import type { PremadeList } from '$lib/api/PremadeListsApi';
import type { SavedItem } from '$lib/api/SavedItemsApi';

vi.mock('$lib/api/PremadeListsApi', () => ({ default: vi.fn() }));
vi.mock('$lib/api/SavedItemsApi', () => ({ default: vi.fn() }));

import PremadeListsApi from '$lib/api/PremadeListsApi';
import SavedItemsApi from '$lib/api/SavedItemsApi';

function makeList(overrides: Partial<PremadeList> = {}): PremadeList {
	return {
		id: 'p1',
		userId: 'u1',
		name: 'Weekly groceries',
		createdAt: '2026-05-25T00:00:00Z',
		savedItemIds: [],
		...overrides
	};
}

function makeSavedItem(id: string, name: string): SavedItem {
	return { id, userId: 'u1', name, createdAt: '2026-05-25T00:00:00Z' };
}

const savedItems = [makeSavedItem('s1', 'Milk'), makeSavedItem('s2', 'Eggs')];

function moveSavedItemIds(ids: string[], savedItemId: string, direction: string): string[] {
	const result = [...ids];
	const index = result.indexOf(savedItemId);
	const target = direction === 'up' ? index - 1 : index + 1;
	if (index < 0 || target < 0 || target >= result.length) return result;
	[result[index], result[target]] = [result[target], result[index]];
	return result;
}

function mockApis(lists: PremadeList[]) {
	vi.mocked(SavedItemsApi).mockImplementation(
		() => ({ list: vi.fn().mockResolvedValue(savedItems) }) as never
	);
	vi.mocked(PremadeListsApi).mockImplementation(
		() =>
			({
				list: vi.fn().mockResolvedValue(lists),
				create: vi
					.fn()
					.mockImplementation((name: string) => Promise.resolve(makeList({ id: 'new-1', name }))),
				delete: vi.fn().mockResolvedValue(undefined),
				addItem: vi.fn().mockImplementation((id: string, savedItemId: string) =>
					Promise.resolve(
						makeList({
							id,
							name: lists.find((l) => l.id === id)?.name ?? '',
							savedItemIds: [...(lists.find((l) => l.id === id)?.savedItemIds ?? []), savedItemId]
						})
					)
				),
				removeItem: vi.fn().mockResolvedValue(undefined),
				moveItem: vi
					.fn()
					.mockImplementation((id: string, savedItemId: string, direction: string) => {
						const source = lists.find((l) => l.id === id);
						const ids = moveSavedItemIds(source?.savedItemIds ?? [], savedItemId, direction);
						return Promise.resolve(makeList({ id, name: source?.name ?? '', savedItemIds: ids }));
					})
			}) as never
	);
}

function getApiMock() {
	return vi.mocked(PremadeListsApi).mock.results[0].value as {
		list: ReturnType<typeof vi.fn>;
		create: ReturnType<typeof vi.fn>;
		delete: ReturnType<typeof vi.fn>;
		addItem: ReturnType<typeof vi.fn>;
		removeItem: ReturnType<typeof vi.fn>;
		moveItem: ReturnType<typeof vi.fn>;
	};
}

function enableFlags() {
	FeatureFlagService.featureFlags = [
		{ name: 'PREMADE_LISTS_ENABLED', isEnabled: true },
		{ name: 'SAVED_ITEMS_ENABLED', isEnabled: true }
	];
}

describe('Premade lists page', () => {
	beforeEach(() => {
		vi.mocked(PremadeListsApi).mockClear();
		vi.mocked(SavedItemsApi).mockClear();
		enableFlags();
	});

	it('shows disabled state when PREMADE_LISTS_ENABLED flag is off', () => {
		FeatureFlagService.featureFlags = [{ name: 'PREMADE_LISTS_ENABLED', isEnabled: false }];
		mockApis([]);
		const { getByTestId } = render(PremadeListsPage);

		expect(getByTestId('premade-lists-disabled')).toBeInTheDocument();
	});

	it('shows the empty state when there are no premade lists', async () => {
		mockApis([]);
		const { getByTestId } = render(PremadeListsPage);

		await waitFor(() => expect(getByTestId('premade-lists-empty-state')).toBeInTheDocument());
	});

	it('creates a premade list', async () => {
		mockApis([]);
		const { getByTestId } = render(PremadeListsPage);
		await waitFor(() => expect(getByTestId('premade-lists-empty-state')).toBeInTheDocument());

		const input = getByTestId('premade-list-form-name');
		await fireEvent.input(input, { target: { value: 'Weekly groceries' } });
		await fireEvent.click(getByTestId('premade-list-form-add'));

		await waitFor(() => expect(getApiMock().create).toHaveBeenCalledWith('Weekly groceries'));
		await waitFor(() =>
			expect(getByTestId('premade-list-row-Weekly groceries')).toBeInTheDocument()
		);
	});

	it('adds a saved item to a premade list', async () => {
		mockApis([makeList({ id: 'p1', name: 'Weekly groceries' })]);
		const { getByTestId } = render(PremadeListsPage);
		await waitFor(() =>
			expect(getByTestId('premade-list-row-Weekly groceries')).toBeInTheDocument()
		);

		const select = getByTestId('premade-list-pick-Weekly groceries');
		await fireEvent.change(select, { target: { value: 's1' } });
		await fireEvent.click(getByTestId('premade-list-add-item-Weekly groceries'));

		await waitFor(() => expect(getApiMock().addItem).toHaveBeenCalledWith('p1', 's1'));
		await waitFor(() =>
			expect(getByTestId('premade-list-item-Weekly groceries-Milk')).toBeInTheDocument()
		);
	});

	it('styles the saved-item picker with the app standard select styling', async () => {
		mockApis([makeList({ id: 'p1', name: 'Weekly groceries' })]);
		const { getByTestId } = render(PremadeListsPage);
		await waitFor(() =>
			expect(getByTestId('premade-list-pick-Weekly groceries')).toBeInTheDocument()
		);

		expect(getByTestId('premade-list-pick-Weekly groceries')).toHaveClass('select__input');
	});

	it('removes a saved item from a premade list', async () => {
		mockApis([makeList({ id: 'p1', name: 'Weekly groceries', savedItemIds: ['s1'] })]);
		const { getByTestId, queryByTestId } = render(PremadeListsPage);
		await waitFor(() =>
			expect(getByTestId('premade-list-item-Weekly groceries-Milk')).toBeInTheDocument()
		);

		await fireEvent.click(getByTestId('premade-list-remove-item-Weekly groceries-Milk'));

		await waitFor(() => expect(getApiMock().removeItem).toHaveBeenCalledWith('p1', 's1'));
		await waitFor(() =>
			expect(queryByTestId('premade-list-item-Weekly groceries-Milk')).not.toBeInTheDocument()
		);
	});

	it('deletes a premade list', async () => {
		mockApis([makeList({ id: 'p1', name: 'Weekly groceries' })]);
		const { getByTestId, queryByTestId } = render(PremadeListsPage);
		await waitFor(() =>
			expect(getByTestId('premade-list-row-Weekly groceries')).toBeInTheDocument()
		);

		await fireEvent.click(getByTestId('premade-list-delete-Weekly groceries'));

		await waitFor(() => expect(getApiMock().delete).toHaveBeenCalledWith('p1'));
		await waitFor(() =>
			expect(queryByTestId('premade-list-row-Weekly groceries')).not.toBeInTheDocument()
		);
	});

	it('renders move buttons hiding up on the first item and down on the last', async () => {
		mockApis([makeList({ id: 'p1', name: 'Weekly groceries', savedItemIds: ['s1', 's2'] })]);
		const { getByTestId, queryByTestId } = render(PremadeListsPage);
		await waitFor(() =>
			expect(getByTestId('premade-list-item-Weekly groceries-Milk')).toBeInTheDocument()
		);

		expect(queryByTestId('premade-list-move-up-Weekly groceries-Milk')).not.toBeInTheDocument();
		expect(getByTestId('premade-list-move-down-Weekly groceries-Milk')).toBeInTheDocument();
		expect(getByTestId('premade-list-move-up-Weekly groceries-Eggs')).toBeInTheDocument();
		expect(queryByTestId('premade-list-move-down-Weekly groceries-Eggs')).not.toBeInTheDocument();
	});

	it('moves a saved item down and reorders the list', async () => {
		mockApis([makeList({ id: 'p1', name: 'Weekly groceries', savedItemIds: ['s1', 's2'] })]);
		const { getByTestId } = render(PremadeListsPage);
		await waitFor(() =>
			expect(getByTestId('premade-list-item-Weekly groceries-Milk')).toBeInTheDocument()
		);

		await fireEvent.click(getByTestId('premade-list-move-down-Weekly groceries-Milk'));

		await waitFor(() => expect(getApiMock().moveItem).toHaveBeenCalledWith('p1', 's1', 'down'));
		await waitFor(() => {
			const eggs = getByTestId('premade-list-item-Weekly groceries-Eggs');
			const milk = getByTestId('premade-list-item-Weekly groceries-Milk');
			expect(eggs.compareDocumentPosition(milk) & Node.DOCUMENT_POSITION_FOLLOWING).toBeTruthy();
		});
	});
});
