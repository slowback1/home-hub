<script lang="ts">
	import { onMount } from 'svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import ComfyUiApi, { type ComfyUiWorkflow } from '$lib/api/ComfyUiApi';

	const HEADING_LEVEL_1 = 1;
	const PROMPT_ROWS = 4;
	const api = new ComfyUiApi();

	let workflows: ComfyUiWorkflow[] = [];
	let loadError: string | null = null;
	let loading = true;

	let selectedWorkflowId = '';
	let prompt = '';
	let negativePrompt = '';
	let generating = false;
	let generateError: string | null = null;
	let imageBase64: string | null = null;

	onMount(async () => {
		try {
			workflows = await api.getWorkflows();
			if (workflows.length > 0) selectedWorkflowId = workflows[0].id;
		} catch {
			loadError = 'Failed to load workflows.';
		} finally {
			loading = false;
		}
	});

	async function generate() {
		if (!selectedWorkflowId || !prompt.trim()) return;
		generating = true;
		generateError = null;
		imageBase64 = null;
		try {
			const result = await api.generateImage(selectedWorkflowId, prompt.trim(), negativePrompt);
			imageBase64 = result.imageBase64;
		} catch {
			generateError =
				'Generation failed. Check that ComfyUI is running and the base URL is configured.';
		} finally {
			generating = false;
		}
	}
</script>

<svelte:head>
	<title>HomeHub — ComfyUI</title>
</svelte:head>

<Heading level={HEADING_LEVEL_1}>ComfyUI Text-to-Image</Heading>

{#if loading}
	<Spinner />
{:else if loadError}
	<p class="error" data-testid="load-error">{loadError}</p>
{:else}
	<form class="generate-form" on:submit|preventDefault={generate} data-testid="generate-form">
		<div class="field">
			<label class="field-label" for="workflow-select">Workflow</label>
			<select
				id="workflow-select"
				class="workflow-select"
				data-testid="workflow-select"
				bind:value={selectedWorkflowId}
				disabled={generating}
			>
				{#each workflows as workflow (workflow.id)}
					<option value={workflow.id}>{workflow.name}</option>
				{/each}
			</select>
		</div>

		<div class="field">
			<label class="field-label" for="prompt-input">Prompt</label>
			<textarea
				id="prompt-input"
				class="prompt-textarea"
				data-testid="prompt-input"
				bind:value={prompt}
				placeholder="Describe the image you want to generate..."
				rows={PROMPT_ROWS}
				disabled={generating}
			></textarea>
		</div>

		<div class="field">
			<label class="field-label" for="negative-prompt-input">Negative Prompt</label>
			<textarea
				id="negative-prompt-input"
				class="prompt-textarea"
				data-testid="negative-prompt-input"
				bind:value={negativePrompt}
				placeholder="Describe what to avoid..."
				rows={PROMPT_ROWS}
				disabled={generating}
			></textarea>
		</div>

		{#if generateError}
			<p class="error" data-testid="generate-error">{generateError}</p>
		{/if}

		<Button
			testId="generate-btn"
			variant="primary"
			onClick={generate}
			disabled={generating || !selectedWorkflowId || !prompt.trim()}
		>
			{generating ? 'Generating...' : 'Generate'}
		</Button>
	</form>

	{#if generating}
		<div class="loading-state" data-testid="generating-indicator">
			<Spinner />
			<p class="loading-text">Generating image, this may take up to a minute...</p>
		</div>
	{/if}

	{#if imageBase64}
		<div class="result" data-testid="result-image-container">
			<img
				src={imageBase64}
				alt="Generated result"
				class="result-image"
				data-testid="result-image"
			/>
		</div>
	{/if}
{/if}

<style>
	.generate-form {
		display: flex;
		flex-direction: column;
		gap: var(--space-4);
		max-width: 600px;
	}

	.field {
		display: flex;
		flex-direction: column;
		gap: var(--space-1);
	}

	.field-label {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-text-secondary);
	}

	.workflow-select {
		padding: var(--space-2) var(--space-3);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-sm, 4px);
		background: var(--color-surface-raised);
		color: var(--color-text-primary);
		font-size: var(--font-size-sm);
	}

	.workflow-select:focus {
		outline: none;
		border-color: var(--color-brand);
	}

	.prompt-textarea {
		font-family: inherit;
		font-size: var(--font-size-sm);
		padding: var(--space-2) var(--space-3);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-sm, 4px);
		background: var(--color-surface-raised);
		color: var(--color-text-primary);
		resize: vertical;
	}

	.prompt-textarea:focus {
		outline: none;
		border-color: var(--color-brand);
	}

	.loading-state {
		display: flex;
		align-items: center;
		gap: var(--space-3);
		margin-top: var(--space-6);
	}

	.loading-text {
		color: var(--color-text-secondary);
		font-size: var(--font-size-sm);
	}

	.result {
		margin-top: var(--space-6);
	}

	.result-image {
		max-width: 100%;
		border-radius: var(--radius-md, 8px);
		border: 1px solid var(--color-border-subtle);
	}

	.error {
		color: var(--color-error, red);
		font-size: var(--font-size-sm);
	}
</style>
