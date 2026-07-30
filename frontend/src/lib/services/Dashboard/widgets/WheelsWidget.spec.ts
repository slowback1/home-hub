import { render, waitFor, fireEvent } from '@testing-library/svelte';
import { vi } from 'vitest';
import WheelsWidget from './WheelsWidget.svelte';
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
		() => ({ getAll: vi.fn().mockResolvedValue(wheels) }) as never
	);
}

describe('WheelsWidget', () => {
	beforeEach(() => {
		vi.mocked(WheelApi).mockClear();
	});

	it('shows an empty state when there are no wheels', async () => {
		mockApi([]);
		const { getByTestId } = render(WheelsWidget);

		await waitFor(() => expect(getByTestId('wheel-widget-empty')).toBeInTheDocument());
	});

	it('quick-spins the default wheel and shows a result from its items', async () => {
		mockApi([makeWheel({ id: '1', name: 'Dinner', items: 'Pizza\nTacos\nSushi' })]);
		const { getByTestId } = render(WheelsWidget);

		await waitFor(() => expect(getByTestId('wheel-widget-spin-btn')).toBeInTheDocument());
		await fireEvent.click(getByTestId('wheel-widget-spin-btn'));

		await waitFor(() => expect(getByTestId('wheel-widget-result')).toBeInTheDocument());
		expect(['Pizza', 'Tacos', 'Sushi']).toContain(
			getByTestId('wheel-widget-result').textContent?.trim()
		);
	});

	it('disables Spin when the selected wheel has no items', async () => {
		mockApi([makeWheel({ id: '1', name: 'Empty', items: '' })]);
		const { getByTestId } = render(WheelsWidget);

		await waitFor(() => expect(getByTestId('wheel-widget-spin-btn')).toBeInTheDocument());
		expect(getByTestId('wheel-widget-spin-btn')).toBeDisabled();
	});
});
