import type TasksApi from '$lib/api/TasksApi';
import type { ChoreTask } from '$lib/api/TasksApi';
import type ToastService from '$lib/ui/containers/toast/ToastService';

const UNDO_DURATION_MS = 5000;

type TasksUpdater = (updater: (tasks: ChoreTask[]) => ChoreTask[]) => void;

export default class TaskCompletionService {
	constructor(
		private api: TasksApi,
		private toastService: ToastService,
		private updateTasks: TasksUpdater
	) {}

	async completeTask(task: ChoreTask): Promise<void> {
		let completed: ChoreTask;
		try {
			completed = await this.api.completeTask(task.id);
		} catch {
			return;
		}

		this.updateTasks((tasks) => tasks.map((t) => (t.id === completed.id ? completed : t)));

		this.toastService.AddToast({
			message: `Marked "${task.name}" done.`,
			durationMs: UNDO_DURATION_MS,
			action: {
				label: 'Undo',
				onClick: () => this.undo(task)
			}
		});
	}

	private async undo(task: ChoreTask): Promise<void> {
		try {
			const restored = await this.api.undoCompletion(task.id);
			this.updateTasks((tasks) => tasks.map((t) => (t.id === restored.id ? restored : t)));
		} catch {
			// undo failed silently
		}
	}
}
