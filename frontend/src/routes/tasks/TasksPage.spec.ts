import { render, waitFor, fireEvent } from '@testing-library/svelte';
import { vi } from 'vitest';
import TasksPage from './+page.svelte';

vi.mock('$lib/api/TasksApi', () => ({ default: vi.fn() }));

import TasksApi from '$lib/api/TasksApi';
import type { ChoreTask } from '$lib/api/TasksApi';

function makeTask(overrides: Partial<ChoreTask> = {}): ChoreTask {
	return {
		id: '1',
		name: 'Old task',
		isRecurring: false,
		intervalDays: null,
		doDate: null,
		completedAt: null,
		createdAt: '2026-07-29T12:00:00Z',
		...overrides
	};
}

function mockApi(tasks: ChoreTask[]) {
	const createTask = vi.fn((req: unknown) =>
		Promise.resolve(makeTask({ id: 'new-1', name: (req as { name: string }).name }))
	);
	const updateTask = vi.fn((id: string, req: unknown) =>
		Promise.resolve(
			makeTask({
				id,
				name: (req as { name: string }).name,
				userId: (req as { isGlobal: boolean }).isGlobal ? null : 'user-1'
			})
		)
	);
	vi.mocked(TasksApi).mockImplementation(
		() =>
			({
				listTasks: vi.fn().mockResolvedValue(tasks),
				createTask,
				updateTask,
				deleteTask: vi.fn(),
				completeTask: vi.fn(),
				undoCompletion: vi.fn()
			}) as never
	);
	return { createTask, updateTask };
}

async function openAddModal(getByTestId: ReturnType<typeof render>['getByTestId']) {
	await fireEvent.click(getByTestId('add-task-button'));
	await waitFor(() => expect(getByTestId('task-modal')).toBeInTheDocument());
}

describe('Tasks page ownership', () => {
	it('shows a shared toggle checked by default when adding a task', async () => {
		mockApi([]);
		const { getByTestId } = render(TasksPage);
		await waitFor(() => expect(getByTestId('add-task-button')).toBeInTheDocument());

		await openAddModal(getByTestId);

		const toggle = getByTestId('task-shared-toggle') as HTMLInputElement;
		expect(toggle.checked).toBe(true);
	});

	it('creates a shared task by default', async () => {
		const { createTask } = mockApi([]);
		const { getByTestId } = render(TasksPage);
		await waitFor(() => expect(getByTestId('add-task-button')).toBeInTheDocument());

		await openAddModal(getByTestId);
		await fireEvent.input(getByTestId('task-name-input'), { target: { value: 'Shared chore' } });
		await fireEvent.click(getByTestId('submit-task-button'));

		await waitFor(() => expect(createTask).toHaveBeenCalledTimes(1));
		expect(createTask.mock.calls[0][0]).toMatchObject({ isGlobal: true });
	});

	it('creates a personal task when the shared toggle is unchecked', async () => {
		const { createTask } = mockApi([]);
		const { getByTestId } = render(TasksPage);
		await waitFor(() => expect(getByTestId('add-task-button')).toBeInTheDocument());

		await openAddModal(getByTestId);
		await fireEvent.input(getByTestId('task-name-input'), { target: { value: 'My chore' } });
		await fireEvent.click(getByTestId('task-shared-toggle'));
		await fireEvent.click(getByTestId('submit-task-button'));

		await waitFor(() => expect(createTask).toHaveBeenCalledTimes(1));
		expect(createTask.mock.calls[0][0]).toMatchObject({ isGlobal: false });
	});

	it('shows the shared toggle checked when editing a global task', async () => {
		mockApi([makeTask({ id: '1', name: 'Global task', userId: null })]);
		const { getByTestId } = render(TasksPage);
		await waitFor(() => expect(getByTestId('edit-task-button')).toBeInTheDocument());

		await fireEvent.click(getByTestId('edit-task-button'));
		await waitFor(() => expect(getByTestId('task-modal')).toBeInTheDocument());

		const toggle = getByTestId('task-shared-toggle') as HTMLInputElement;
		expect(toggle.checked).toBe(true);
	});

	it('shows the shared toggle unchecked when editing a personal task', async () => {
		mockApi([makeTask({ id: '1', name: 'My task', userId: 'user-1' })]);
		const { getByTestId } = render(TasksPage);
		await waitFor(() => expect(getByTestId('edit-task-button')).toBeInTheDocument());

		await fireEvent.click(getByTestId('edit-task-button'));
		await waitFor(() => expect(getByTestId('task-modal')).toBeInTheDocument());

		const toggle = getByTestId('task-shared-toggle') as HTMLInputElement;
		expect(toggle.checked).toBe(false);
	});

	it('updates a task as personal when the shared toggle is unchecked', async () => {
		const { updateTask } = mockApi([makeTask({ id: '1', name: 'My task', userId: 'user-1' })]);
		const { getByTestId } = render(TasksPage);
		await waitFor(() => expect(getByTestId('edit-task-button')).toBeInTheDocument());

		await fireEvent.click(getByTestId('edit-task-button'));
		await waitFor(() => expect(getByTestId('task-modal')).toBeInTheDocument());

		await fireEvent.click(getByTestId('submit-task-button'));

		await waitFor(() => expect(updateTask).toHaveBeenCalledTimes(1));
		expect(updateTask.mock.calls[0][1]).toMatchObject({ isGlobal: false });
	});

	it('updates a task as global when the shared toggle is checked', async () => {
		const { updateTask } = mockApi([makeTask({ id: '1', name: 'My task', userId: 'user-1' })]);
		const { getByTestId } = render(TasksPage);
		await waitFor(() => expect(getByTestId('edit-task-button')).toBeInTheDocument());

		await fireEvent.click(getByTestId('edit-task-button'));
		await waitFor(() => expect(getByTestId('task-modal')).toBeInTheDocument());

		await fireEvent.click(getByTestId('task-shared-toggle'));
		await fireEvent.click(getByTestId('submit-task-button'));

		await waitFor(() => expect(updateTask).toHaveBeenCalledTimes(1));
		expect(updateTask.mock.calls[0][1]).toMatchObject({ isGlobal: true });
	});
});
