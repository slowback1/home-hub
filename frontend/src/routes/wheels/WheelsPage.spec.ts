import { render, waitFor, fireEvent } from '@testing-library/svelte';
import { vi } from 'vitest';
import WheelsPage from './+page.svelte';
import type { Wheel } from '$lib/api/WheelApi';

vi.mock('$lib/api/WheelApi', () => ({ default: vi.fn() }));

import WheelApi from '$lib/api/WheelApi';

function makeWheel(overrides: Partial<Wheel> = {}): Wheel {
	return {
		id: '1',
		name: 'Dinner',
		items: 'Pizza\nTacos\nSushi',
		createdAt: '2026-07-29T12:00:00Z',
		...overrides
	};
}

function mockApi(wheels: Wheel[]) {
	vi.mocked(WheelApi).mockImplementation(
		() =>
			({
				getAll: vi.fn().mockResolvedValue(wheels),
				create: vi.fn(),
				update: vi.fn(),
				delete: vi.fn()
			}) as never
	);
}

describe('Wheels page', () => {
	beforeEach(() => {
		vi.mocked(WheelApi).mockClear();
	});

	it('shows the empty state when there are no wheels', async () => {
		mockApi([]);
		const { getByTestId } = render(WheelsPage);

		await waitFor(() => expect(getByTestId('wheels-empty-state')).toBeInTheDocument());
	});

	it('lists saved wheels with their item counts', async () => {
		mockApi([
			makeWheel({ id: '1', name: 'Dinner', items: 'Pizza\nTacos\nSushi' }),
			makeWheel({ id: '2', name: 'Movies', items: 'Alien\nHeat' })
		]);
		const { getByTestId } = render(WheelsPage);

		await waitFor(() => expect(getByTestId('wheel-row-Dinner')).toBeInTheDocument());
		expect(getByTestId('wheel-item-count-Dinner').textContent).toContain('3');
		expect(getByTestId('wheel-item-count-Movies').textContent).toContain('2');
	});

	it('spins the selected wheel and shows a result from its items', async () => {
		mockApi([makeWheel({ id: '1', name: 'Dinner', items: 'Pizza\nTacos\nSushi' })]);
		const { getByTestId } = render(WheelsPage);

		await waitFor(() => expect(getByTestId('spin-btn')).toBeInTheDocument());
		await fireEvent.click(getByTestId('spin-btn'));

		await waitFor(() => expect(getByTestId('wheel-spin-result')).toBeInTheDocument());
		expect(['Pizza', 'Tacos', 'Sushi']).toContain(
			getByTestId('wheel-spin-result').textContent?.trim()
		);
	});

	it('disables Spin when the selected wheel has no items', async () => {
		mockApi([makeWheel({ id: '1', name: 'Empty', items: '' })]);
		const { getByTestId } = render(WheelsPage);

		await waitFor(() => expect(getByTestId('spin-btn')).toBeInTheDocument());
		expect(getByTestId('spin-btn')).toBeDisabled();
	});
});
