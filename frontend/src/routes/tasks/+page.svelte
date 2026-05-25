<script lang="ts">
	import { onMount } from 'svelte';
	import TasksApi, { type ChoreTask } from '$lib/api/TasksApi';

	const api = new TasksApi();

	let tasks: ChoreTask[] = [];
	let loadError: string | null = null;
	let loading = true;

	function today(): Date {
		const d = new Date();
		d.setHours(0, 0, 0, 0);
		return d;
	}

	function isDue(task: ChoreTask): boolean {
		if (!task.doDate) return true;
		return new Date(task.doDate) <= today();
	}

	$: dueTasks = tasks
		.filter(isDue)
		.sort((a, b) => {
			const aHasDate = !!a.doDate;
			const bHasDate = !!b.doDate;
			if (aHasDate && bHasDate) return new Date(a.doDate!).getTime() - new Date(b.doDate!).getTime();
			if (aHasDate && !bHasDate) return -1;
			if (!aHasDate && bHasDate) return 1;
			return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();
		});

	$: upcomingTasks = tasks
		.filter((t) => t.doDate && new Date(t.doDate) > today())
		.sort((a, b) => new Date(a.doDate!).getTime() - new Date(b.doDate!).getTime());

	function formatDate(iso: string): string {
		return new Date(iso).toLocaleDateString(undefined, { month: 'short', day: 'numeric', year: 'numeric' });
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

<div class="tasks-page" data-testid="tasks-page">
	<div class="page-header">
		<h1 class="page-title">Task Tracker</h1>
		<button class="btn btn--primary" data-testid="add-task-button">Add Task</button>
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
								>
									Edit
								</button>
								<button
									class="btn btn--done btn--sm"
									data-testid="done-task-button"
									aria-label="Done {task.name}"
								>
									Done
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

	.btn {
		padding: var(--space-2) var(--space-4);
		border-radius: var(--radius-sm);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		cursor: pointer;
		border: 1px solid transparent;
		transition: opacity 0.15s ease;
	}

	.btn:hover {
		opacity: 0.85;
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
</style>
