import BaseApi from './baseApi';

export type ChoreTask = {
	id: string;
	name: string;
	isRecurring: boolean;
	intervalDays: number | null;
	doDate: string | null;
	completedAt: string | null;
	createdAt: string;
};

export type TaskCompletion = {
	id: string;
	taskId: string;
	completedAt: string;
	previousDoDate: string | null;
};

export type CreateTaskRequest = {
	name: string;
	isRecurring: boolean;
	intervalDays: number | null;
	doDate: string | null;
};

export type UpdateTaskRequest = {
	name: string;
	isRecurring: boolean;
	intervalDays: number | null;
	doDate: string | null;
};

export default class TasksApi extends BaseApi {
	constructor() {
		super();
	}

	async listTasks(): Promise<ChoreTask[]> {
		return this.Get<ChoreTask[]>('/api/tasks');
	}

	async createTask(req: CreateTaskRequest): Promise<ChoreTask> {
		return this.Post<ChoreTask>('/api/tasks', req);
	}

	async updateTask(id: string, req: UpdateTaskRequest): Promise<ChoreTask> {
		return this.Put<ChoreTask>(`/api/tasks/${id}`, req);
	}

	async deleteTask(id: string): Promise<void> {
		return this.Delete<void>(`/api/tasks/${id}`);
	}

	async completeTask(id: string): Promise<ChoreTask> {
		return this.Post<ChoreTask>(`/api/tasks/${id}/complete`, {});
	}

	async undoCompletion(id: string): Promise<ChoreTask> {
		return this.Delete<ChoreTask>(`/api/tasks/${id}/completions/latest`);
	}
}
