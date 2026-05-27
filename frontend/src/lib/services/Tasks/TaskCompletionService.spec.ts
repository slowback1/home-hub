import { describe, it, expect, vi, beforeEach } from 'vitest';
import TaskCompletionService from './TaskCompletionService';
import type TasksApi from '$lib/api/TasksApi';
import type { ChoreTask } from '$lib/api/TasksApi';
import type ToastService from '$lib/ui/containers/toast/ToastService';
import type { ToastConfig } from '$lib/ui/containers/toast/ToastService';

function makeTask(overrides: Partial<ChoreTask> = {}): ChoreTask {
	return {
		id: 'task-1',
		name: 'Wash dishes',
		isRecurring: false,
		intervalDays: null,
		doDate: new Date().toISOString().slice(0, 10),
		completedAt: null,
		createdAt: new Date().toISOString(),
		...overrides
	};
}

describe('TaskCompletionService', () => {
	let mockApi: { completeTask: ReturnType<typeof vi.fn>; undoCompletion: ReturnType<typeof vi.fn> };
	let mockToastService: { AddToast: ReturnType<typeof vi.fn> };
	let updateTasks: ReturnType<typeof vi.fn>;
	let service: TaskCompletionService;

	beforeEach(() => {
		const completedTask = makeTask({ completedAt: new Date().toISOString() });
		mockApi = {
			completeTask: vi.fn().mockResolvedValue(completedTask),
			undoCompletion: vi.fn().mockResolvedValue(makeTask())
		};
		mockToastService = { AddToast: vi.fn() };
		updateTasks = vi.fn();
		service = new TaskCompletionService(
			mockApi as unknown as TasksApi,
			mockToastService as unknown as ToastService,
			updateTasks
		);
	});

	it('calls TasksApi.completeTask with the task id', async () => {
		const task = makeTask();
		await service.completeTask(task);

		expect(mockApi.completeTask).toHaveBeenCalledWith(task.id);
	});

	it('calls updateTasks with an updater that removes the completed task', async () => {
		const task = makeTask({ id: 'task-1' });
		const other = makeTask({ id: 'task-2', name: 'Other task' });

		await service.completeTask(task);

		expect(updateTasks).toHaveBeenCalled();
		const updater = updateTasks.mock.calls[0][0];
		const result = updater([task, other]);
		expect(result).toHaveLength(1);
		expect(result[0].id).toBe('task-2');
	});

	it('fires a toast with an Undo action after successful completion', async () => {
		const task = makeTask();
		await service.completeTask(task);

		expect(mockToastService.AddToast).toHaveBeenCalled();
		const config: ToastConfig = mockToastService.AddToast.mock.calls[0][0];
		expect(config.action?.label).toEqual('Undo');
		expect(config.durationMs).toEqual(5000);
	});

	it('does not fire a toast when the API call fails', async () => {
		mockApi.completeTask.mockRejectedValue(new Error('network error'));
		const task = makeTask();
		await service.completeTask(task);

		expect(mockToastService.AddToast).not.toHaveBeenCalled();
	});

	it('does not call updateTasks when the API call fails', async () => {
		mockApi.completeTask.mockRejectedValue(new Error('network error'));
		await service.completeTask(makeTask());

		expect(updateTasks).not.toHaveBeenCalled();
	});

	it('calls undoCompletion and adds the restored task back via updateTasks', async () => {
		const task = makeTask();
		const restoredTask = makeTask({ completedAt: null });
		mockApi.undoCompletion.mockResolvedValue(restoredTask);

		await service.completeTask(task);

		const config: ToastConfig = mockToastService.AddToast.mock.calls[0][0];
		await config.action!.onClick();

		expect(mockApi.undoCompletion).toHaveBeenCalledWith(task.id);
		expect(updateTasks).toHaveBeenCalledTimes(2);
		const undoUpdater = updateTasks.mock.calls[1][0];
		const result = undoUpdater([]);
		expect(result).toContain(restoredTask);
	});

	it('silently swallows errors from undoCompletion', async () => {
		const task = makeTask();
		mockApi.undoCompletion.mockRejectedValue(new Error('undo failed'));

		await service.completeTask(task);
		const config: ToastConfig = mockToastService.AddToast.mock.calls[0][0];

		await expect(config.action!.onClick()).resolves.toBeUndefined();
	});
});
