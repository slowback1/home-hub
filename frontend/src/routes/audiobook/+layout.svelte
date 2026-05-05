<script lang="ts">
	import { page } from '$app/stores';

	const audiobookTabs = [
		{ href: '/audiobook', label: 'Convert' },
		{ href: '/audiobook/voice-samples', label: 'Voice Samples' }
	];
</script>

<div class="audiobook-layout">
	<div class="audiobook-tabs" role="tablist" aria-label="Audiobook sections">
		{#each audiobookTabs as tab (tab.href)}
			<a
				href={tab.href}
				class="audiobook-tab"
				class:audiobook-tab--active={$page.url.pathname === tab.href}
			>
				{tab.label}
			</a>
		{/each}
	</div>
	<div class="audiobook-content">
		<slot />
	</div>
</div>

<style>
	.audiobook-layout {
		display: flex;
		flex-direction: column;
		gap: var(--space-4);
	}

	.audiobook-tabs {
		display: flex;
		gap: var(--space-1);
		border-bottom: 1px solid var(--color-border-subtle);
		padding-bottom: var(--space-1);
	}

	.audiobook-tab {
		padding: var(--space-2) var(--space-4);
		background: none;
		border: none;
		border-radius: var(--radius-sm) var(--radius-sm) 0 0;
		color: var(--color-text-secondary);
		font-family: var(--font-family-primary);
		font-size: var(--font-size-md);
		font-weight: var(--font-weight-medium);
		cursor: pointer;
		text-decoration: none;
		transition:
			color 0.15s ease,
			background-color 0.15s ease;
	}

	.audiobook-tab:hover {
		color: var(--color-text-primary);
		background-color: var(--color-surface-raised);
	}

	.audiobook-tab--active {
		color: var(--color-brand-lighter);
		border-bottom: 2px solid var(--color-brand-lighter);
		margin-bottom: -1px;
	}

	.audiobook-content {
		display: flex;
		flex-direction: column;
	}
</style>
