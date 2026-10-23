import { render, waitFor } from '@testing-library/svelte';
import { vi } from 'vitest';
import ActivityWidget from './ActivityWidget.svelte';
import type { ActivityPick } from '$lib/api/ActivityPickApi';

vi.mock('$lib/api/ActivityPickApi', () => ({ default: vi.fn() }));

import ActivityPickApi from '$lib/api/ActivityPickApi';

function makePick(overrides: Partial<ActivityPick> = {}): ActivityPick {
	return {
		id: '1',
		activityName: 'Reading',
		pickedAt: '2026-10-21T12:00:00Z',
		...overrides
	};
}

function mockApi(pick: ActivityPick) {
	vi.mocked(ActivityPickApi).mockImplementation(
		() => ({ getCurrent: vi.fn().mockResolvedValue(pick) }) as never
	);
}

describe('ActivityWidget', () => {
	beforeEach(() => {
		vi.mocked(ActivityPickApi).mockClear();
	});

	it('shows the picked activity name', async () => {
		mockApi(makePick());
		const { getByTestId } = render(ActivityWidget);

		await waitFor(() => expect(getByTestId('activity-widget-name')).toBeInTheDocument());
		expect(getByTestId('activity-widget-name').textContent?.trim()).toBe('Reading');
	});

	it('does not show a picked-date subtitle', async () => {
		mockApi(makePick());
		const { getByTestId, queryByText } = render(ActivityWidget);

		await waitFor(() => expect(getByTestId('activity-widget-name')).toBeInTheDocument());
		expect(queryByText(/picked/i)).not.toBeInTheDocument();
	});
});
