<script lang="ts">
	import { onMount } from 'svelte';
	import AudiobookApi from '$lib/api/AudiobookApi';

	const api = new AudiobookApi();

	let samples: string[] = [];
	let loadError: string | null = null;
	let uploadError: string | null = null;

	let fileInput: HTMLInputElement;

	onMount(async () => {
		try {
			samples = await api.listVoiceSamples();
		} catch {
			loadError = 'Failed to load voice samples. Please refresh the page.';
		}
	});

	async function handleUpload(e: Event) {
		const file = (e.currentTarget as HTMLInputElement).files?.[0];
		if (!file) return;
		uploadError = null;
		const name = file.name.replace(/\.wav$/i, '');
		try {
			await api.uploadVoiceSample(file, name);
			samples = [...samples, name];
		} catch {
			uploadError = 'Failed to upload voice sample.';
		} finally {
			fileInput.value = '';
		}
	}

	async function handleDelete(name: string) {
		try {
			await api.deleteVoiceSample(name);
			samples = samples.filter((s) => s !== name);
		} catch {
			// ignore
		}
	}
</script>

<svelte:head>
	<title>HomeHub — Voice Samples</title>
</svelte:head>

<section class="voice-samples-section">
	<h1 class="page-title">Voice Samples</h1>

	{#if loadError}
		<p class="error" data-testid="load-error">{loadError}</p>
	{/if}

	<div class="upload-row">
		<label class="upload-btn" data-testid="upload-voice-sample-label">
			Upload WAV
			<input
				type="file"
				accept=".wav"
				class="hidden-input"
				bind:this={fileInput}
				data-testid="wav-file-input"
				on:change={handleUpload}
			/>
		</label>
		{#if uploadError}
			<p class="error" data-testid="upload-error">{uploadError}</p>
		{/if}
	</div>

	{#if samples.length === 0}
		<p class="empty-state" data-testid="no-samples-message">No voice samples uploaded yet.</p>
	{:else}
		<ul class="sample-list" data-testid="voice-sample-list">
			{#each samples as sample (sample)}
				<li class="sample-item" data-testid="voice-sample-item" data-sample-name={sample}>
					<span class="sample-name">{sample}</span>
					<button
						class="delete-btn"
						data-testid="delete-voice-sample-button"
						on:click={() => handleDelete(sample)}
					>
						Delete
					</button>
				</li>
			{/each}
		</ul>
	{/if}
</section>

<style>
	.voice-samples-section {
		display: flex;
		flex-direction: column;
		gap: var(--space-4);
	}

	.page-title {
		font-size: var(--font-size-xl);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
		margin: 0;
	}

	.upload-row {
		display: flex;
		align-items: center;
		gap: var(--space-3);
	}

	.upload-btn {
		display: inline-flex;
		align-items: center;
		padding: var(--space-2) var(--space-4);
		background-color: var(--color-brand-lighter);
		color: white;
		border-radius: var(--radius-sm);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		cursor: pointer;
		transition: opacity 0.15s ease;
	}

	.upload-btn:hover {
		opacity: 0.85;
	}

	.hidden-input {
		display: none;
	}

	.error {
		color: var(--color-error, #e53e3e);
		font-size: var(--font-size-sm);
		margin: 0;
	}

	.empty-state {
		color: var(--color-text-secondary);
		font-size: var(--font-size-sm);
	}

	.sample-list {
		list-style: none;
		margin: 0;
		padding: 0;
		display: flex;
		flex-direction: column;
		gap: var(--space-2);
	}

	.sample-item {
		display: flex;
		align-items: center;
		justify-content: space-between;
		padding: var(--space-2) var(--space-4);
		background-color: var(--color-surface-raised);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-sm);
	}

	.sample-name {
		font-size: var(--font-size-sm);
		color: var(--color-text-primary);
	}

	.delete-btn {
		padding: var(--space-1) var(--space-3);
		background-color: #fed7d7;
		color: #9b2c2c;
		border: 1px solid #fc8181;
		border-radius: var(--radius-sm);
		font-size: var(--font-size-sm);
		cursor: pointer;
		transition: opacity 0.15s ease;
	}

	.delete-btn:hover {
		opacity: 0.85;
	}
</style>
