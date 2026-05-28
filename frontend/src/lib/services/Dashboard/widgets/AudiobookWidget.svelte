<script lang="ts">
	import { onMount } from 'svelte';
	import AudiobookApi, { type AudiobookJob, type AudiobookJobStatus } from '$lib/api/AudiobookApi';
	import Badge from '$lib/ui/feedback/Badge.svelte';

	const POLL_MS = 300_000;

	type BadgeVariant = 'default' | 'success' | 'warning' | 'error';

	const STATUS_MAP: Record<AudiobookJobStatus, { label: string; variant: BadgeVariant }> = {
		queued: { label: 'queued', variant: 'default' },
		in_progress: { label: 'in progress', variant: 'default' },
		completed: { label: 'completed', variant: 'success' },
		failed: { label: 'failed', variant: 'error' },
		cancelled: { label: 'cancelled', variant: 'warning' }
	};

	const api = new AudiobookApi();
	let job: AudiobookJob | null = null;
	let loading = true;
	let error = false;

	function pickJob(jobs: AudiobookJob[]): AudiobookJob | null {
		const active = jobs.filter((j) => j.status === 'queued' || j.status === 'in_progress');
		if (active.length > 0) {
			return active.sort((a, b) => b.updatedAt.localeCompare(a.updatedAt))[0];
		}
		const completed = jobs.filter((j) => j.status === 'completed');
		if (completed.length > 0) {
			return completed.sort((a, b) => b.updatedAt.localeCompare(a.updatedAt))[0];
		}
		return null;
	}

	async function fetchData() {
		try {
			const jobs = await api.listJobs();
			job = pickJob(jobs);
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

<div class="audiobook-widget">
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
	{:else if !job}
		<div class="audiobook-empty" data-testid="audiobook-widget-empty">
			<span class="audiobook-empty__text">No conversions yet</span>
		</div>
	{:else}
		{@const status = STATUS_MAP[job.status] ?? STATUS_MAP.queued}
		<div class="audiobook-body">
			<span
				class="audiobook-filename"
				data-testid="audiobook-widget-filename"
				title={job.epubFilename}
			>
				{job.epubFilename}
			</span>
			<div class="audiobook-meta">
				<Badge text={status.label} variant={status.variant} testId="audiobook-widget-status" />
			</div>
		</div>
	{/if}
</div>

<style>
	.audiobook-widget {
		height: 100%;
		display: flex;
		align-items: center;
		justify-content: center;
	}

	.widget-state {
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
		to {
			transform: rotate(360deg);
		}
	}

	.widget-state__dash {
		font-size: var(--font-size-2xl);
		line-height: 1;
	}

	.widget-state__label {
		font-size: var(--font-size-sm);
		font-style: italic;
	}

	.audiobook-empty {
		display: flex;
		align-items: center;
		justify-content: center;
		width: 100%;
	}

	.audiobook-empty__text {
		font-size: var(--font-size-sm);
		font-style: italic;
		color: var(--color-text-secondary);
	}

	.audiobook-body {
		display: flex;
		flex-direction: column;
		gap: var(--space-2);
		width: 100%;
	}

	.audiobook-filename {
		font-family: var(--font-family-mono, monospace);
		font-size: var(--font-size-sm);
		color: var(--color-text-primary);
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}

	.audiobook-meta {
		display: flex;
		align-items: center;
		gap: var(--space-2);
	}
</style>
