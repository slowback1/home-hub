<script lang="ts">
	import { onMount } from 'svelte';
	import TasksApi, { type ChoreTask } from '$lib/api/TasksApi';
	import ToastService from '$lib/ui/containers/toast/ToastService';
	import TaskCompletionService from '$lib/services/Tasks/TaskCompletionService';

	const POLL_MS = 3_600_000;
	const MAX_VISIBLE = 5;

	const api = new TasksApi();
	const toastService = new ToastService();

	let tasks: ChoreTask[] = [];
	let loading = true;
	let error = false;

	const completionService = new TaskCompletionService(api, toastService, (updater) => {
		tasks = updater(tasks);
	});

	function today(): Date {
		const d = new Date();
		d.setHours(0, 0, 0, 0);
		return d;
	}

	function isDue(task: ChoreTask): boolean {
		if (!task.doDate) return true;
		return new Date(task.doDate) <= today();
	}

	$: dueTasks = tasks.filter(isDue).sort((a, b) => a.name.localeCompare(b.name));
	$: visibleTasks = dueTasks.slice(0, MAX_VISIBLE);
	$: overflow = Math.max(0, dueTasks.length - MAX_VISIBLE);

	async function fetchData() {
		try {
			tasks = await api.listTasks();
			error = false;
		} catch {
			error = true;
		}
	}

	onMount(() => {
		fetchData().finally(() => (loading = false));
		const interval = setInterval(fetchData, POLL_MS);
		return () => clearInterval(interval);
	});
</script>

<div class="tasks-widget">
	{#if loading}
		<div class="widget-state" role="status">
			<span class="widget-spinner" aria-hidden="true"></span>
			<span class="widget-state__label">Loading…</span>
		</div>
	{:else if error}
		<div class="widget-state" role="alert">
			<span class="widget-state__dash" aria-hidden="true">—</span>
			<span class="widget-state__label">Unavailable</span>
		</div>
	{:else if dueTasks.length === 0}
		<div class="widget-state">
			<span class="widget-state__label">No tasks due</span>
		</div>
	{:else}
		<ul class="tasks-list">
			{#each visibleTasks as task (task.id)}
				<li class="task-row" data-testid="tasks-widget-row">
					<span class="task-name">{task.name}</span>
					<button
						class="task-done-btn"
						data-testid="tasks-widget-done-btn"
						aria-label="Done {task.name}"
						on:click={() => completionService.completeTask(task)}
					>
						DONE
					</button>
				</li>
			{/each}
		</ul>
		{#if overflow > 0}
			<div class="tasks-overflow" data-testid="tasks-widget-overflow">
				and {overflow} more…
			</div>
		{/if}
	{/if}
</div>

<style>
	.tasks-widget {
		height: 100%;
		display: flex;
		flex-direction: column;
	}

	.widget-state {
		flex: 1;
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		gap: var(--space-2);
		color: var(--color-text-secondary);
	}

	.widget-spinner {
		width: 20px;
		height: 20px;
		border: 2px solid var(--color-border-subtle);
		border-top-color: var(--color-brand-lighter);
		border-radius: 50%;
		animation: spin 0.7s linear infinite;
	}

	@keyframes spin {
		to { transform: rotate(360deg); }
	}

	.widget-state__dash {
		font-size: var(--font-size-2xl);
		line-height: 1;
	}

	.widget-state__label {
		font-size: var(--font-size-sm);
		font-style: italic;
	}

	.tasks-list {
		list-style: none;
		padding: 0;
		margin: 0;
		display: flex;
		flex-direction: column;
		flex: 1;
	}

	.task-row {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: var(--space-2);
		padding: 5px 0;
	}

	.task-name {
		font-size: var(--font-size-sm);
		color: var(--color-text-primary);
		flex: 1;
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}

	.task-done-btn {
		background: none;
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-full);
		color: var(--color-text-secondary);
		cursor: pointer;
		font-family: var(--font-family-primary);
		font-size: var(--font-size-xs);
		font-weight: var(--font-weight-semibold);
		letter-spacing: 0.04em;
		padding: 2px var(--space-2);
		white-space: nowrap;
		transition:
			background-color 0.15s ease,
			color 0.15s ease,
			border-color 0.15s ease;
		flex-shrink: 0;
	}

	.task-done-btn:hover {
		background: var(--color-success-surface);
		border-color: var(--color-success);
		color: var(--color-success-text);
	}

	.tasks-overflow {
		font-size: var(--font-size-xs);
		color: var(--color-text-disabled);
		padding-top: var(--space-1);
	}
</style>
