<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import AudiobookApi, { type AudiobookJob } from '$lib/api/AudiobookApi';

	const POLL_INTERVAL_MS = 30_000;
	const TERMINAL_STATUSES = new Set(['completed', 'failed', 'cancelled']);

	const api = new AudiobookApi();

	let voiceSamples: string[] = [];
	let jobs: AudiobookJob[] = [];
	let loadError: string | null = null;
	let submitError: string | null = null;

	let selectedEpub: File | null = null;
	let selectedVoiceSample = '';
	let submitting = false;

	let pollTimer: ReturnType<typeof setInterval> | null = null;

	function hasActiveJobs(): boolean {
		return jobs.some((j) => !TERMINAL_STATUSES.has(j.status));
	}

	function startPolling() {
		if (pollTimer) return;
		pollTimer = setInterval(async () => {
			jobs = await api.listJobs();
			if (!hasActiveJobs()) stopPolling();
		}, POLL_INTERVAL_MS);
	}

	function stopPolling() {
		if (pollTimer) {
			clearInterval(pollTimer);
			pollTimer = null;
		}
	}

	onMount(async () => {
		try {
			[voiceSamples, jobs] = await Promise.all([api.listVoiceSamples(), api.listJobs()]);
			if (voiceSamples.length > 0) selectedVoiceSample = voiceSamples[0];
			if (hasActiveJobs()) startPolling();
		} catch {
			loadError = 'Failed to load data. Please refresh the page.';
		}
	});

	onDestroy(() => stopPolling());

	async function handleSubmit() {
		if (!selectedEpub || !selectedVoiceSample) return;
		submitError = null;
		submitting = true;
		try {
			const job = await api.submitJob(selectedEpub, selectedVoiceSample);
			jobs = [...jobs, job];
			selectedEpub = null;
			startPolling();
		} catch {
			submitError = 'Failed to submit job. Please try again.';
		} finally {
			submitting = false;
		}
	}

	async function handleCancel(job: AudiobookJob) {
		try {
			await api.cancelJob(job.id);
			jobs = jobs.map((j) => (j.id === job.id ? { ...j, status: 'cancelled' } : j));
		} catch {
			// ignore — job may have already transitioned
		}
	}

	async function handleDeleteJob(job: AudiobookJob) {
		try {
			await api.deleteJob(job.id);
			jobs = jobs.filter((j) => j.id !== job.id);
		} catch {
			// ignore
		}
	}

	function formatDate(iso: string): string {
		return new Date(iso).toLocaleString();
	}
</script>

<svelte:head>
	<title>HomeHub — Audiobook Convert</title>
</svelte:head>

<section class="convert-section">
	<h1 class="page-title">Convert EPUB to Audiobook</h1>

	{#if loadError}
		<p class="error" data-testid="load-error">{loadError}</p>
	{/if}

	{#if voiceSamples.length === 0}
		<p class="no-samples-message" data-testid="no-voice-samples-message">
			No voice samples available. Add one in the <a href="/audiobook/voice-samples">Voice Samples</a
			> tab.
		</p>
	{/if}

	<form
		class="submit-form"
		data-testid="conversion-form"
		on:submit|preventDefault={handleSubmit}
		aria-disabled={voiceSamples.length === 0}
	>
		<div class="form-field">
			<label for="epub-file">EPUB File</label>
			<input
				id="epub-file"
				type="file"
				accept=".epub"
				disabled={voiceSamples.length === 0}
				data-testid="epub-file-input"
				on:change={(e) => {
					selectedEpub = (e.currentTarget as HTMLInputElement).files?.[0] ?? null;
				}}
			/>
		</div>

		<div class="form-field">
			<label for="voice-sample">Voice Sample</label>
			<select
				id="voice-sample"
				bind:value={selectedVoiceSample}
				disabled={voiceSamples.length === 0}
				data-testid="voice-sample-select"
			>
				{#each voiceSamples as sample (sample)}
					<option value={sample}>{sample}</option>
				{/each}
			</select>
		</div>

		<button
			type="submit"
			disabled={voiceSamples.length === 0 || !selectedEpub || submitting}
			data-testid="submit-job-button"
		>
			{submitting ? 'Submitting…' : 'Convert'}
		</button>

		{#if submitError}
			<p class="error" data-testid="submit-error">{submitError}</p>
		{/if}
	</form>

	{#if jobs.length > 0}
		<div class="job-list" data-testid="job-list">
			<h2>Conversion Queue</h2>
			<table class="jobs-table">
				<thead>
					<tr>
						<th>File</th>
						<th>Voice</th>
						<th>Status</th>
						<th>Created</th>
						<th>Actions</th>
					</tr>
				</thead>
				<tbody>
					{#each jobs as job (job.id)}
						<tr
							class="job-row"
							class:job-row--in-progress={job.status === 'in_progress'}
							data-testid="job-row"
							data-job-id={job.id}
						>
							<td>{job.epubFilename}</td>
							<td>{job.voiceSampleName}</td>
							<td>
								<span class="status-badge status-badge--{job.status}" data-testid="job-status">
									{job.status.replace('_', ' ')}
								</span>
								{#if job.status === 'failed' && job.errorMessage}
									<p class="error-message" data-testid="job-error-message">{job.errorMessage}</p>
								{/if}
							</td>
							<td>{formatDate(job.createdAt)}</td>
							<td class="actions">
								{#if job.status === 'queued' || job.status === 'in_progress'}
									<button
										class="action-btn action-btn--danger"
										data-testid="cancel-job-button"
										on:click={() => handleCancel(job)}
									>
										Cancel
									</button>
								{/if}
								{#if job.status === 'completed'}
									<a
										href={api.getFileUrl(job.id)}
										download
										class="action-btn action-btn--primary"
										data-testid="download-job-button"
									>
										Download
									</a>
								{/if}
								{#if TERMINAL_STATUSES.has(job.status)}
									<button
										class="action-btn action-btn--secondary"
										data-testid="delete-job-button"
										on:click={() => handleDeleteJob(job)}
									>
										Delete
									</button>
								{/if}
							</td>
						</tr>
					{/each}
				</tbody>
			</table>
		</div>
	{/if}
</section>

<style>
	.convert-section {
		display: flex;
		flex-direction: column;
		gap: var(--space-6);
	}

	.page-title {
		font-size: var(--font-size-xl);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
		margin: 0;
	}

	.no-samples-message {
		padding: var(--space-3) var(--space-4);
		background-color: var(--color-surface-raised);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-sm);
		color: var(--color-text-secondary);
	}

	.submit-form {
		display: flex;
		flex-direction: column;
		gap: var(--space-3);
		max-width: 480px;
	}

	.form-field {
		display: flex;
		flex-direction: column;
		gap: var(--space-1);
	}

	.form-field label {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-text-secondary);
	}

	.form-field input,
	.form-field select {
		padding: var(--space-2) var(--space-3);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-sm);
		background-color: var(--color-surface-base);
		color: var(--color-text-primary);
		font-size: var(--font-size-sm);
	}

	.form-field input:disabled,
	.form-field select:disabled {
		opacity: 0.5;
		cursor: not-allowed;
	}

	.error {
		color: var(--color-error, #e53e3e);
		font-size: var(--font-size-sm);
		margin: 0;
	}

	.jobs-table {
		width: 100%;
		border-collapse: collapse;
		font-size: var(--font-size-sm);
	}

	.jobs-table th,
	.jobs-table td {
		text-align: left;
		padding: var(--space-2) var(--space-3);
		border-bottom: 1px solid var(--color-border-subtle);
	}

	.jobs-table th {
		color: var(--color-text-secondary);
		font-weight: var(--font-weight-medium);
	}

	.job-row--in-progress {
		background-color: var(--color-surface-raised);
		outline: 1px solid var(--color-brand-lighter);
	}

	.status-badge {
		display: inline-block;
		padding: 2px var(--space-2);
		border-radius: var(--radius-sm);
		font-size: var(--font-size-xs, 0.75rem);
		font-weight: var(--font-weight-medium);
		text-transform: capitalize;
	}

	.status-badge--queued {
		background: var(--color-surface-overlay);
		color: var(--color-text-secondary);
	}
	.status-badge--in_progress {
		background: #bee3f8;
		color: #2b6cb0;
	}
	.status-badge--completed {
		background: #c6f6d5;
		color: #276749;
	}
	.status-badge--failed {
		background: #fed7d7;
		color: #9b2c2c;
	}
	.status-badge--cancelled {
		background: var(--color-surface-overlay);
		color: var(--color-text-secondary);
	}

	.error-message {
		margin: var(--space-1) 0 0;
		color: var(--color-error, #e53e3e);
		font-size: var(--font-size-xs, 0.75rem);
	}

	.actions {
		display: flex;
		gap: var(--space-2);
		align-items: center;
	}

	.action-btn {
		padding: var(--space-1) var(--space-3);
		border-radius: var(--radius-sm);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		cursor: pointer;
		text-decoration: none;
		border: 1px solid transparent;
		transition: opacity 0.15s ease;
	}

	.action-btn:hover {
		opacity: 0.85;
	}

	.action-btn--primary {
		background: var(--color-brand-lighter);
		color: white;
	}
	.action-btn--secondary {
		background: var(--color-surface-overlay);
		color: var(--color-text-primary);
	}
	.action-btn--danger {
		background: #fed7d7;
		color: #9b2c2c;
		border-color: #fc8181;
	}
</style>
