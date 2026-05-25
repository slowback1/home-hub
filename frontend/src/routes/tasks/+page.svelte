<script lang="ts">
	import { onMount } from 'svelte';
	import { SvelteDate } from 'svelte/reactivity';
	import TasksApi, { type ChoreTask } from '$lib/api/TasksApi';

	const api = new TasksApi();

	let tasks: ChoreTask[] = [];
	let loadError: string | null = null;
	let loading = true;

	// Done / undo state
	let completingTaskId: string | null = null;
	let undoTask: ChoreTask | null = null;
	let undoTimer: ReturnType<typeof setTimeout> | null = null;

	const UNDO_TIMEOUT_MS = 5000;
	const DEFAULT_INTERVAL_DAYS = 7;
	const SORT_BEFORE = -1;
	const SORT_AFTER = 1;

	function clearUndoToast() {
		undoTask = null;
		if (undoTimer) {
			clearTimeout(undoTimer);
			undoTimer = null;
		}
	}

	async function handleDone(task: ChoreTask) {
		if (completingTaskId) return;
		clearUndoToast();
		completingTaskId = task.id;
		const snapshot = { ...task };
		try {
			const updated = await api.completeTask(task.id);
			tasks = tasks.map((t) => (t.id === updated.id ? updated : t));
			undoTask = snapshot;
			undoTimer = setTimeout(clearUndoToast, UNDO_TIMEOUT_MS);
		} catch {
			// leave list unchanged on error
		} finally {
			completingTaskId = null;
		}
	}

	async function handleUndo() {
		if (!undoTask) return;
		const taskToUndo = undoTask;
		clearUndoToast();
		try {
			const restored = await api.undoCompletion(taskToUndo.id);
			tasks = tasks.map((t) => (t.id === restored.id ? restored : t));
		} catch {
			// undo failed silently — task remains in current state
		}
	}

	// Modal state
	let modalOpen = false;
	let editingTask: ChoreTask | null = null;
	let formName = '';
	let formDoDate = '';
	let formIsRecurring = false;
	let formIntervalDays = DEFAULT_INTERVAL_DAYS;
	let formError: string | null = null;
	let formSubmitting = false;
	let formDeleting = false;

	function today(): Date {
		const d = new SvelteDate();
		d.setHours(0, 0, 0, 0);
		return d;
	}

	function isDue(task: ChoreTask): boolean {
		if (!task.doDate) return true;
		return new Date(task.doDate) <= today();
	}

	function doDateMs(task: ChoreTask): number | null {
		return task.doDate ? new Date(task.doDate).getTime() : null;
	}

	function compareDueTasks(a: ChoreTask, b: ChoreTask): number {
		const aMs = doDateMs(a);
		const bMs = doDateMs(b);
		if (aMs !== null && bMs !== null) return aMs - bMs;
		if (aMs !== null) return SORT_BEFORE;
		if (bMs !== null) return SORT_AFTER;
		return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();
	}

	$: dueTasks = tasks.filter((t) => t.completedAt == null && isDue(t)).sort(compareDueTasks);

	$: upcomingTasks = tasks
		.filter((t) => t.completedAt == null && t.doDate && new Date(t.doDate) > today())
		.sort((a, b) => new Date(a.doDate!).getTime() - new Date(b.doDate!).getTime());

	function formatDate(iso: string): string {
		return new Date(iso).toLocaleDateString(undefined, {
			month: 'short',
			day: 'numeric',
			year: 'numeric'
		});
	}

	function openAddModal() {
		editingTask = null;
		formName = '';
		formDoDate = '';
		formIsRecurring = false;
		formIntervalDays = DEFAULT_INTERVAL_DAYS;
		formError = null;
		modalOpen = true;
	}

	function openEditModal(task: ChoreTask) {
		editingTask = task;
		formName = task.name;
		formDoDate = task.doDate ? task.doDate.split('T')[0] : '';
		formIsRecurring = task.isRecurring;
		formIntervalDays = task.intervalDays ?? DEFAULT_INTERVAL_DAYS;
		formError = null;
		modalOpen = true;
	}

	function closeModal() {
		modalOpen = false;
		editingTask = null;
		formError = null;
	}

	function handleKeydown(e: KeyboardEvent) {
		if (e.key === 'Escape') closeModal();
	}

	function validateTaskForm(): string | null {
		if (!formName.trim()) return 'Name is required.';
		if (formIsRecurring && formIntervalDays < 1) return 'Interval must be at least 1 day.';
		return null;
	}

	function buildTaskPayload() {
		return {
			name: formName.trim(),
			isRecurring: formIsRecurring,
			intervalDays: formIsRecurring ? formIntervalDays : null,
			doDate: formDoDate || null
		};
	}

	function submitButtonLabel(): string {
		if (formSubmitting) return 'Saving…';
		return editingTask ? 'Save' : 'Add Task';
	}

	async function handleSubmit() {
		const validationError = validateTaskForm();
		if (validationError) {
			formError = validationError;
			return;
		}

		formError = null;
		formSubmitting = true;
		try {
			const payload = buildTaskPayload();

			if (editingTask) {
				const updated = await api.updateTask(editingTask.id, payload);
				tasks = tasks.map((t) => (t.id === updated.id ? updated : t));
			} else {
				const created = await api.createTask(payload);
				tasks = [...tasks, created];
			}
			closeModal();
		} catch {
			formError = 'Something went wrong. Please try again.';
		} finally {
			formSubmitting = false;
		}
	}

	async function handleDelete() {
		if (!editingTask) return;
		formError = null;
		formDeleting = true;
		try {
			await api.deleteTask(editingTask.id);
			tasks = tasks.filter((t) => t.id !== editingTask!.id);
			closeModal();
		} catch {
			formError = 'Failed to delete task. Please try again.';
		} finally {
			formDeleting = false;
		}
	}

	onMount(async () => {
		try {
			tasks = await api.listTasks();
		} catch {
			loadError = 'Failed to load tasks. Please refresh the page.';
		} finally {
			loading = false;
		}
	});
</script>

<svelte:head>
	<title>HomeHub — Task Tracker</title>
</svelte:head>

<svelte:window on:keydown={handleKeydown} />

<div class="tasks-page" data-testid="tasks-page">
	<div class="page-header">
		<h1 class="page-title">Task Tracker</h1>
		<button class="btn btn--primary" data-testid="add-task-button" on:click={openAddModal}>
			Add Task
		</button>
	</div>

	{#if loadError}
		<p class="error" data-testid="load-error">{loadError}</p>
	{/if}

	{#if loading}
		<p class="loading" data-testid="loading">Loading…</p>
	{:else}
		<section class="task-section" data-testid="due-section">
			<h2 class="section-title">Due</h2>
			{#if dueTasks.length === 0}
				<p class="empty-state" data-testid="due-empty">No tasks due — you're all caught up!</p>
			{:else}
				<ul class="task-list">
					{#each dueTasks as task (task.id)}
						<li class="task-row" data-testid="due-task-row" data-task-id={task.id}>
							<span class="task-name" data-testid="task-name">{task.name}</span>
							{#if task.isRecurring}
								<span class="task-meta">every {task.intervalDays}d</span>
							{/if}
							<div class="task-actions">
								<button
									class="btn btn--secondary btn--sm"
									data-testid="edit-task-button"
									aria-label="Edit {task.name}"
									on:click={() => openEditModal(task)}
								>
									Edit
								</button>
								<button
									class="btn btn--done btn--sm"
									data-testid="done-task-button"
									aria-label="Done {task.name}"
									disabled={completingTaskId === task.id}
									on:click={() => handleDone(task)}
								>
									{completingTaskId === task.id ? '…' : 'Done'}
								</button>
							</div>
						</li>
					{/each}
				</ul>
			{/if}
		</section>

		<section class="task-section" data-testid="upcoming-section">
			<h2 class="section-title">Upcoming</h2>
			{#if upcomingTasks.length === 0}
				<p class="empty-state" data-testid="upcoming-empty">No upcoming tasks.</p>
			{:else}
				<ul class="task-list">
					{#each upcomingTasks as task (task.id)}
						<li class="task-row" data-testid="upcoming-task-row" data-task-id={task.id}>
							<span class="task-name" data-testid="task-name">{task.name}</span>
							{#if task.isRecurring}
								<span class="task-meta">every {task.intervalDays}d</span>
							{/if}
							<span class="task-date" data-testid="task-do-date">{formatDate(task.doDate!)}</span>
							<div class="task-actions">
								<button
									class="btn btn--secondary btn--sm"
									data-testid="edit-task-button"
									aria-label="Edit {task.name}"
									on:click={() => openEditModal(task)}
								>
									Edit
								</button>
							</div>
						</li>
					{/each}
				</ul>
			{/if}
		</section>
	{/if}
</div>

{#if undoTask}
	<div class="undo-toast" data-testid="undo-toast" role="status">
		<span>Task completed.</span>
		<button class="undo-btn" data-testid="undo-button" on:click={handleUndo}>Undo</button>
	</div>
{/if}

{#if modalOpen}
	<div
		class="modal-backdrop"
		data-testid="modal-backdrop"
		on:click|self={closeModal}
		role="presentation"
	>
		<div class="modal" role="dialog" aria-modal="true" data-testid="task-modal">
			<div class="modal-header">
				<h2 class="modal-title">{editingTask ? 'Edit Task' : 'Add Task'}</h2>
				<button
					class="modal-close"
					aria-label="Close"
					data-testid="modal-close-button"
					on:click={closeModal}>✕</button
				>
			</div>

			<form class="modal-form" on:submit|preventDefault={handleSubmit} data-testid="task-form">
				<div class="form-field">
					<label for="task-name">Name</label>
					<input
						id="task-name"
						type="text"
						bind:value={formName}
						placeholder="Task name"
						data-testid="task-name-input"
						required
					/>
				</div>

				<div class="form-field">
					<label for="task-do-date">Do Date <span class="optional">(optional)</span></label>
					<input
						id="task-do-date"
						type="date"
						bind:value={formDoDate}
						data-testid="task-do-date-input"
					/>
				</div>

				<div class="form-field form-field--toggle">
					<label for="task-recurring">Recurring</label>
					<input
						id="task-recurring"
						type="checkbox"
						bind:checked={formIsRecurring}
						data-testid="task-recurring-toggle"
					/>
				</div>

				{#if formIsRecurring}
					<div class="form-field">
						<label for="task-interval">Repeat every (days)</label>
						<input
							id="task-interval"
							type="number"
							min="1"
							bind:value={formIntervalDays}
							data-testid="task-interval-input"
						/>
					</div>
				{/if}

				{#if formError}
					<p class="form-error" data-testid="form-error">{formError}</p>
				{/if}

				<div class="modal-actions">
					{#if editingTask}
						<button
							type="button"
							class="btn btn--danger"
							data-testid="delete-task-button"
							disabled={formDeleting || formSubmitting}
							on:click={handleDelete}
						>
							{formDeleting ? 'Deleting…' : 'Delete'}
						</button>
					{/if}
					<div class="modal-actions-right">
						<button
							type="button"
							class="btn btn--secondary"
							data-testid="cancel-modal-button"
							on:click={closeModal}
						>
							Cancel
						</button>
						<button
							type="submit"
							class="btn btn--primary"
							data-testid="submit-task-button"
							disabled={formSubmitting || formDeleting}
						>
							{submitButtonLabel()}
						</button>
					</div>
				</div>
			</form>
		</div>
	</div>
{/if}

<style>
	.tasks-page {
		display: flex;
		flex-direction: column;
		gap: var(--space-6);
	}

	.page-header {
		display: flex;
		align-items: center;
		justify-content: space-between;
	}

	.page-title {
		font-size: var(--font-size-xl);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
		margin: 0;
	}

	.section-title {
		font-size: var(--font-size-md, 1rem);
		font-weight: var(--font-weight-semibold, 600);
		color: var(--color-text-secondary);
		margin: 0 0 var(--space-3);
	}

	.task-section {
		display: flex;
		flex-direction: column;
	}

	.task-list {
		list-style: none;
		padding: 0;
		margin: 0;
		display: flex;
		flex-direction: column;
		gap: var(--space-2);
	}

	.task-row {
		display: flex;
		align-items: center;
		gap: var(--space-3);
		padding: var(--space-3) var(--space-4);
		background-color: var(--color-surface-raised);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-sm);
	}

	.task-name {
		flex: 1;
		font-size: var(--font-size-sm);
		color: var(--color-text-primary);
	}

	.task-meta {
		font-size: var(--font-size-xs, 0.75rem);
		color: var(--color-text-secondary);
	}

	.task-date {
		font-size: var(--font-size-xs, 0.75rem);
		color: var(--color-text-secondary);
		white-space: nowrap;
	}

	.task-actions {
		display: flex;
		gap: var(--space-2);
	}

	.empty-state {
		color: var(--color-text-secondary);
		font-size: var(--font-size-sm);
		padding: var(--space-4);
		text-align: center;
		border: 1px dashed var(--color-border-subtle);
		border-radius: var(--radius-sm);
		margin: 0;
	}

	.loading {
		color: var(--color-text-secondary);
		font-size: var(--font-size-sm);
	}

	.error {
		color: var(--color-error, #e53e3e);
		font-size: var(--font-size-sm);
	}

	/* Buttons */
	.btn {
		padding: var(--space-2) var(--space-4);
		border-radius: var(--radius-sm);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		cursor: pointer;
		border: 1px solid transparent;
		transition: opacity 0.15s ease;
	}

	.btn:hover:not(:disabled) {
		opacity: 0.85;
	}

	.btn:disabled {
		opacity: 0.5;
		cursor: not-allowed;
	}

	.btn--sm {
		padding: var(--space-1) var(--space-3);
		font-size: var(--font-size-xs, 0.75rem);
	}

	.btn--primary {
		background: var(--color-brand-lighter);
		color: white;
	}

	.btn--secondary {
		background: var(--color-surface-overlay);
		color: var(--color-text-primary);
	}

	.btn--done {
		background: #c6f6d5;
		color: #276749;
		border-color: #9ae6b4;
	}

	.btn--danger {
		background: #fed7d7;
		color: #9b2c2c;
		border-color: #fc8181;
	}

	/* Modal */
	.modal-backdrop {
		position: fixed;
		inset: 0;
		background: rgba(0, 0, 0, 0.5);
		display: flex;
		align-items: center;
		justify-content: center;
		z-index: 100;
	}

	.modal {
		background: var(--color-surface-base);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md);
		padding: var(--space-6);
		width: 100%;
		max-width: 480px;
		display: flex;
		flex-direction: column;
		gap: var(--space-4);
	}

	.modal-header {
		display: flex;
		align-items: center;
		justify-content: space-between;
	}

	.modal-title {
		font-size: var(--font-size-lg, 1.125rem);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
		margin: 0;
	}

	.modal-close {
		background: none;
		border: none;
		font-size: var(--font-size-md);
		color: var(--color-text-secondary);
		cursor: pointer;
		padding: var(--space-1);
		line-height: 1;
	}

	.modal-close:hover {
		color: var(--color-text-primary);
	}

	.modal-form {
		display: flex;
		flex-direction: column;
		gap: var(--space-4);
	}

	.form-field {
		display: flex;
		flex-direction: column;
		gap: var(--space-1);
	}

	.form-field--toggle {
		flex-direction: row;
		align-items: center;
		gap: var(--space-3);
	}

	.form-field label {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-text-secondary);
	}

	.optional {
		font-weight: normal;
		color: var(--color-text-disabled, #aaa);
	}

	.form-field input[type='text'],
	.form-field input[type='date'],
	.form-field input[type='number'] {
		padding: var(--space-2) var(--space-3);
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-sm);
		background-color: var(--color-surface-raised);
		color: var(--color-text-primary);
		font-size: var(--font-size-sm);
	}

	.form-field input:focus {
		outline: none;
		border-color: var(--color-brand-lighter);
	}

	.form-error {
		color: var(--color-error, #e53e3e);
		font-size: var(--font-size-sm);
		margin: 0;
	}

	.modal-actions {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: var(--space-3);
		padding-top: var(--space-2);
	}

	.modal-actions-right {
		display: flex;
		gap: var(--space-2);
		margin-left: auto;
	}

	/* Undo toast */
	.undo-toast {
		position: fixed;
		bottom: var(--space-6);
		left: 50%;
		transform: translateX(-50%);
		display: flex;
		align-items: center;
		gap: var(--space-4);
		padding: var(--space-3) var(--space-6);
		background: var(--color-surface-overlay);
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-md);
		box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
		z-index: 200;
		font-size: var(--font-size-sm);
		color: var(--color-text-primary);
		white-space: nowrap;
	}

	.undo-btn {
		background: none;
		border: none;
		color: var(--color-brand-lighter);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		cursor: pointer;
		padding: 0;
		text-decoration: underline;
	}

	.undo-btn:hover {
		opacity: 0.8;
	}
</style>
