<script lang="ts">
	import { onMount } from 'svelte';
	import { Trash2 } from 'lucide-svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import TextBox from '$lib/ui/inputs/TextBox/TextBox.svelte';
	import ActivityApi, { type Activity } from '$lib/api/ActivityApi';
	import ToastService, { ToastVariant } from '$lib/ui/containers/toast/ToastService';

	const MIN_WEIGHT = 1;
	const MAX_WEIGHT = 5;
	const ICON_SIZE = 16;
	const WEIGHTS = Array.from({ length: MAX_WEIGHT }, (_, i) => i + MIN_WEIGHT);

	const api = new ActivityApi();
	const toastService = new ToastService();

	let loading = true;
	let error: string | null = null;
	let activities: Activity[] = [];
	let newName = '';
	let newWeight = 1;

	onMount(async () => {
		try {
			activities = await api.getAll();
		} catch {
			error = 'Failed to load activities.';
		} finally {
			loading = false;
		}
	});

	async function changeWeight(activity: Activity, weight: number) {
		try {
			const updated = await api.update(activity.id, activity.name, weight);
			activities = activities.map((a) => (a.id === updated.id ? updated : a));
			toastService.AddToast({ message: 'Weight updated.', variant: ToastVariant.success });
		} catch {
			toastService.AddToast({ message: 'Failed to update weight.', variant: ToastVariant.error });
		}
	}

	async function deleteActivity(activity: Activity) {
		try {
			await api.delete(activity.id);
			activities = activities.filter((a) => a.id !== activity.id);
			toastService.AddToast({ message: 'Activity deleted.', variant: ToastVariant.success });
		} catch {
			toastService.AddToast({ message: 'Failed to delete activity.', variant: ToastVariant.error });
		}
	}

	async function addActivity() {
		if (!newName.trim()) return;
		try {
			const created = await api.create(newName.trim(), newWeight);
			activities = [...activities, created];
			newName = '';
			newWeight = 1;
			toastService.AddToast({ message: 'Activity added.', variant: ToastVariant.success });
		} catch {
			toastService.AddToast({ message: 'Failed to add activity.', variant: ToastVariant.error });
		}
	}
</script>

<svelte:head>
	<title>HomeHub — Activity Config</title>
</svelte:head>

<Heading level={1}>Activity Config</Heading>

{#if loading}
	<Spinner />
{:else if error}
	<p class="error">{error}</p>
{:else}
	<table class="activity-table" data-testid="activity-table">
		<thead>
			<tr>
				<th class="col-name">Activity</th>
				<th class="col-weight">Weight</th>
				<th class="col-actions"></th>
			</tr>
		</thead>
		<tbody>
			{#each activities as activity (activity.id)}
				<tr data-testid="activity-row-{activity.name}">
					<td class="col-name">{activity.name}</td>
					<td class="col-weight">
						<div class="weight-buttons" data-testid="weight-buttons-{activity.name}">
							{#each WEIGHTS as w (w)}
								<button
									class="weight-btn"
									class:weight-btn--active={activity.weight === w}
									data-testid="weight-btn-{activity.name}-{w}"
									on:click={() => changeWeight(activity, w)}
									aria-pressed={activity.weight === w}>{w}</button
								>
							{/each}
						</div>
					</td>
					<td class="col-actions">
						<button
							class="delete-btn"
							data-testid="delete-btn-{activity.name}"
							aria-label="Delete {activity.name}"
							on:click={() => deleteActivity(activity)}
						>
							<Trash2 size={ICON_SIZE} />
						</button>
					</td>
				</tr>
			{/each}

			<tr class="add-row" data-testid="add-row">
				<td class="col-name">
					<TextBox
						label=""
						id="new-activity-name"
						bind:value={newName}
						placeholder="New activity name"
					/>
				</td>
				<td class="col-weight">
					<div class="weight-buttons">
						{#each WEIGHTS as w (w)}
							<button
								class="weight-btn"
								class:weight-btn--active={newWeight === w}
								data-testid="new-weight-btn-{w}"
								on:click={() => (newWeight = w)}
								aria-pressed={newWeight === w}>{w}</button
							>
						{/each}
					</div>
				</td>
				<td class="col-actions">
					<Button testId="add-activity-btn" variant="primary" size="small" onClick={addActivity}
						>Add</Button
					>
				</td>
			</tr>
		</tbody>
	</table>
{/if}

<style>
	.activity-table {
		width: 100%;
		border-collapse: collapse;
	}

	.activity-table th {
		text-align: left;
		padding: var(--space-2) var(--space-4);
		color: var(--color-text-secondary);
		font-weight: var(--font-weight-semibold);
		border-bottom: 2px solid var(--color-border-subtle);
	}

	.activity-table td {
		padding: var(--space-3) var(--space-4);
		border-bottom: 1px solid var(--color-border-subtle);
		vertical-align: middle;
	}

	.col-name {
		width: 50%;
	}

	.col-weight {
		width: 35%;
	}

	.col-actions {
		width: 15%;
		text-align: right;
	}

	.weight-buttons {
		display: flex;
		gap: var(--space-1);
	}

	.weight-btn {
		width: 2rem;
		height: 2rem;
		border: 1px solid var(--color-border-subtle);
		background: var(--color-surface-raised);
		color: var(--color-text-primary);
		border-radius: var(--radius-sm, 4px);
		cursor: pointer;
		font-size: var(--font-size-sm);
		display: flex;
		align-items: center;
		justify-content: center;
	}

	.weight-btn:hover {
		border-color: var(--color-brand);
		color: var(--color-brand);
	}

	.weight-btn--active {
		background: var(--color-brand);
		border-color: var(--color-brand);
		color: var(--color-text-on-brand, white);
	}

	.delete-btn {
		background: none;
		border: none;
		cursor: pointer;
		color: var(--color-text-secondary);
		padding: var(--space-1);
		display: inline-flex;
		align-items: center;
	}

	.delete-btn:hover {
		color: var(--color-error, red);
	}

	.add-row td {
		padding-top: var(--space-4);
	}

	.add-row :global(.text-box__label) {
		display: none;
	}

	.error {
		color: var(--color-error, red);
		padding: var(--space-4);
	}
</style>
