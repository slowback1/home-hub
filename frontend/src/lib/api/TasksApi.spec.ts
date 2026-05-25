import { mockApi } from '$lib/testHelpers/getFetchMock';
import TasksApi, { type ChoreTask } from './TasksApi';

const mockTask: ChoreTask = {
	id: '1',
	name: 'Sweep floors',
	isRecurring: true,
	intervalDays: 7,
	doDate: '2026-05-24',
	completedAt: null,
	createdAt: '2026-05-01T00:00:00Z'
};

describe('TasksApi', () => {
	it('listTasks calls GET /api/tasks and returns ChoreTask[]', async () => {
		const mockFetch = mockApi({ '/api/tasks': [mockTask] });
		const api = new TasksApi();

		const result = await api.listTasks();

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/tasks');
		expect(options.method).toEqual('GET');
		expect(result).toEqual([mockTask]);
	});

	it('createTask calls POST /api/tasks with correct body', async () => {
		const mockFetch = mockApi({ '/api/tasks': mockTask });
		const api = new TasksApi();

		const result = await api.createTask({
			name: 'Sweep floors',
			isRecurring: true,
			intervalDays: 7,
			doDate: '2026-05-24'
		});

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/tasks');
		expect(options.method).toEqual('POST');
		expect(JSON.parse(options.body as string)).toEqual({
			name: 'Sweep floors',
			isRecurring: true,
			intervalDays: 7,
			doDate: '2026-05-24'
		});
		expect(result).toEqual(mockTask);
	});

	it('updateTask calls PUT /api/tasks/{id}', async () => {
		const updated = { ...mockTask, name: 'Sweep all floors' };
		const mockFetch = mockApi({ '/api/tasks/1': updated });
		const api = new TasksApi();

		const result = await api.updateTask('1', {
			name: 'Sweep all floors',
			isRecurring: true,
			intervalDays: 7,
			doDate: '2026-05-24'
		});

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/tasks/1');
		expect(options.method).toEqual('PUT');
		expect(result).toEqual(updated);
	});

	it('deleteTask calls DELETE /api/tasks/{id}', async () => {
		const mockFetch = mockApi({ '/api/tasks/1': {} });
		const api = new TasksApi();

		await api.deleteTask('1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/tasks/1');
		expect(options.method).toEqual('DELETE');
	});

	it('completeTask calls POST /api/tasks/{id}/complete', async () => {
		const completed = { ...mockTask, completedAt: '2026-05-24T10:00:00Z' };
		const mockFetch = mockApi({ '/api/tasks/1/complete': completed });
		const api = new TasksApi();

		const result = await api.completeTask('1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/tasks/1/complete');
		expect(options.method).toEqual('POST');
		expect(result).toEqual(completed);
	});

	it('undoCompletion calls DELETE /api/tasks/{id}/completions/latest', async () => {
		const mockFetch = mockApi({ '/api/tasks/1/completions/latest': mockTask });
		const api = new TasksApi();

		const result = await api.undoCompletion('1');

		const [url, options] = mockFetch.mock.calls[0] as never as [string, RequestInit];
		expect(url).toContain('/api/tasks/1/completions/latest');
		expect(options.method).toEqual('DELETE');
		expect(result).toEqual(mockTask);
	});
});
