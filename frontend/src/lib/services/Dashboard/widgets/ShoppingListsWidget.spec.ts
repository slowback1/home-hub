import { render, waitFor } from '@testing-library/svelte';
import { vi } from 'vitest';
import ShoppingListsWidget from './ShoppingListsWidget.svelte';

vi.mock('$lib/api/ShoppingListApi', () => ({ default: vi.fn() }));

import ShoppingListApi, { type ShoppingList } from '$lib/api/ShoppingListApi';

function makeList(overrides: Partial<ShoppingList> = {}): ShoppingList {
	return {
		id: '1',
		userId: 'u1',
		name: 'Groceries',
		status: 'active',
		archivedAt: null,
		createdAt: '2026-07-29T12:00:00Z',
		items: [],
		...overrides
	};
}

function mockApi(lists: ShoppingList[]) {
	vi.mocked(ShoppingListApi).mockImplementation(
		() => ({ listActive: vi.fn().mockResolvedValue(lists) }) as never
	);
}

describe('ShoppingListsWidget', () => {
	beforeEach(() => {
		vi.mocked(ShoppingListApi).mockClear();
	});

	it('shows a loading state while fetching', async () => {
		vi.mocked(ShoppingListApi).mockImplementation(
			() => ({ listActive: () => new Promise(() => {}) }) as never
		);
		const { getByRole } = render(ShoppingListsWidget);

		expect(getByRole('status').textContent).toContain('Loading…');
	});

	it('shows an Unavailable error state when the fetch fails', async () => {
		vi.mocked(ShoppingListApi).mockImplementation(
			() => ({ listActive: vi.fn().mockRejectedValue(new Error('boom')) }) as never
		);
		const { getByRole } = render(ShoppingListsWidget);

		await waitFor(() => expect(getByRole('alert').textContent).toContain('Unavailable'));
	});

	it('shows an empty state when there are no active lists', async () => {
		mockApi([]);
		const { queryByText } = render(ShoppingListsWidget);

		await waitFor(() => expect(queryByText('No active lists')).toBeInTheDocument());
	});

	it('renders each list as a link to its fullscreen route', async () => {
		mockApi([
			makeList({ id: 'l1', name: 'Groceries', createdAt: '2026-08-01T12:00:00Z' }),
			makeList({ id: 'l2', name: 'Hardware', createdAt: '2026-07-29T12:00:00Z' })
		]);
		const { getAllByTestId } = render(ShoppingListsWidget);

		await waitFor(() => expect(getAllByTestId('shopping-lists-widget-link')).toHaveLength(2));
		const links = getAllByTestId('shopping-lists-widget-link');
		expect(links.map((l) => l.getAttribute('href'))).toEqual([
			'/shopping-lists?fullscreen=l1',
			'/shopping-lists?fullscreen=l2'
		]);
		expect(links.map((l) => l.textContent?.replace(/\s+/g, ''))).toEqual([
			'Groceries›',
			'Hardware›'
		]);
	});

	it('orders lists newest-created first', async () => {
		mockApi([
			makeList({ id: 'old', name: 'Old', createdAt: '2026-06-01T12:00:00Z' }),
			makeList({ id: 'new', name: 'New', createdAt: '2026-08-01T12:00:00Z' }),
			makeList({ id: 'mid', name: 'Mid', createdAt: '2026-07-01T12:00:00Z' })
		]);
		const { getAllByTestId } = render(ShoppingListsWidget);

		await waitFor(() => expect(getAllByTestId('shopping-lists-widget-link')).toHaveLength(3));
		expect(getAllByTestId('shopping-lists-widget-link').map((l) => l.getAttribute('href'))).toEqual(
			[
				'/shopping-lists?fullscreen=new',
				'/shopping-lists?fullscreen=mid',
				'/shopping-lists?fullscreen=old'
			]
		);
	});

	it('caps at 5 rows and shows an overflow count for the rest', async () => {
		mockApi(
			Array.from({ length: 7 }, (_, i) =>
				makeList({ id: `l${i}`, name: `List ${i}`, createdAt: `2026-08-0${i + 1}T12:00:00Z` })
			)
		);
		const { getAllByTestId, getByTestId } = render(ShoppingListsWidget);

		await waitFor(() => expect(getAllByTestId('shopping-lists-widget-link')).toHaveLength(5));
		expect(getByTestId('shopping-lists-widget-overflow').textContent).toContain('and 2 more…');
	});
});
