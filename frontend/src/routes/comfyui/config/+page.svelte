<script lang="ts">
	import { onMount } from 'svelte';
	import { Trash2 } from 'lucide-svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import TextBox from '$lib/ui/inputs/TextBox/TextBox.svelte';
	import ComfyUiApi, { type ComfyUiWorkflow } from '$lib/api/ComfyUiApi';

	const ICON_SIZE = 16;
	const JSON_TEXTAREA_ROWS = 10;
	const HEADING_LEVEL_1 = 1;
	const HEADING_LEVEL_2 = 2;
	const api = new ComfyUiApi();

	let loading = true;
	let error: string | null = null;
	let workflows: ComfyUiWorkflow[] = [];
	let newName = '';
	let newJson = '';
	let submitting = false;
	let submitError: string | null = null;

	onMount(async () => {
		try {
			workflows = await api.getWorkflows();
		} catch {
			error = 'Failed to load workflows.';
		} finally {
			loading = false;
		}
	});

	async function addWorkflow() {
		if (!newName.trim() || !newJson.trim()) return;
		submitting = true;
		submitError = null;
		try {
			const created = await api.createWorkflow(newName.trim(), newJson.trim());
			workflows = [...workflows, created];
			newName = '';
			newJson = '';
		} catch {
			submitError = 'Failed to add workflow.';
		} finally {
			submitting = false;
		}
	}

	async function deleteWorkflow(workflow: ComfyUiWorkflow) {
		try {
			await api.deleteWorkflow(workflow.id);
			workflows = workflows.filter((w) => w.id !== workflow.id);
		} catch {
			error = 'Failed to delete workflow.';
		}
	}
</script>

<svelte:head>
	<title>HomeHub — ComfyUI Config</title>
</svelte:head>

<Heading level={HEADING_LEVEL_1}>ComfyUI Workflows</Heading>

{#if loading}
	<Spinner />
{:else if error}
	<p class="error" data-testid="load-error">{error}</p>
{:else}
	<table class="workflow-table" data-testid="workflow-table">
		<thead>
			<tr>
				<th class="col-name">Name</th>
				<th class="col-actions"></th>
			</tr>
		</thead>
		<tbody>
			{#each workflows as workflow (workflow.id)}
				<tr data-testid="workflow-row-{workflow.name}">
					<td class="col-name">{workflow.name}</td>
					<td class="col-actions">
						<button
							class="delete-btn"
							data-testid="delete-btn-{workflow.name}"
							aria-label="Delete {workflow.name}"
							on:click={() => deleteWorkflow(workflow)}
						>
							<Trash2 size={ICON_SIZE} />
						</button>
					</td>
				</tr>
			{/each}
		</tbody>
	</table>

	<section class="add-section" data-testid="add-workflow-form">
		<Heading level={HEADING_LEVEL_2}>Add Workflow</Heading>
		{#if submitError}
			<p class="error" data-testid="submit-error">{submitError}</p>
		{/if}
		<div class="form-field">
			<TextBox label="Name" id="workflow-name" bind:value={newName} placeholder="Workflow name" />
		</div>
		<div class="form-field">
			<label class="textarea-label" for="workflow-json">Workflow JSON (API format)</label>
			<textarea
				id="workflow-json"
				class="json-textarea"
				data-testid="workflow-json-input"
				bind:value={newJson}
				placeholder="Paste ComfyUI API-format JSON with prompt/negative_prompt placeholders..."
				rows={JSON_TEXTAREA_ROWS}
			></textarea>
		</div>
		<Button
			testId="add-workflow-btn"
			variant="primary"
			onClick={addWorkflow}
			disabled={submitting || !newName.trim() || !newJson.trim()}
		>
			{submitting ? 'Adding...' : 'Add Workflow'}
		</Button>
	</section>
{/if}

<style>
	.workflow-table {
		width: 100%;
		border-collapse: collapse;
		margin-bottom: var(--space-8);
	}

	.workflow-table th {
		text-align: left;
		padding: var(--space-2) var(--space-4);
		color: var(--color-text-secondary);
		font-weight: var(--font-weight-semibold);
		border-bottom: 2px solid var(--color-border-subtle);
	}

	.workflow-table td {
		padding: var(--space-3) var(--space-4);
		border-bottom: 1px solid var(--color-border-subtle);
		vertical-align: middle;
	}

	.col-name {
		width: 85%;
	}

	.col-actions {
		width: 15%;
		text-align: right;
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

	.add-section {
		max-width: 600px;
	}

	.form-field {
		margin-bottom: var(--space-4);
	}

	.textarea-label {
		display: block;
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-text-secondary);
		margin-bottom: var(--space-1);
	}

	.json-textarea {
		width: 100%;
		font-family: monospace;
		font-size: var(--font-size-sm);
		padding: var(--space-2) var(--space-3);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-sm, 4px);
		background: var(--color-surface-raised);
		color: var(--color-text-primary);
		resize: vertical;
		box-sizing: border-box;
	}

	.json-textarea:focus {
		outline: none;
		border-color: var(--color-brand);
	}

	.error {
		color: var(--color-error, red);
		padding: var(--space-2) 0;
	}
</style>
