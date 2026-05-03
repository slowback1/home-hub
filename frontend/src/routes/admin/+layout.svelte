<script lang="ts">
	import { page } from '$app/stores';

	const adminTabs = [
		{ href: '/admin/system-config', label: 'System Config' },
		{ href: '/admin/feature-flags', label: 'Feature Flags' }
	];
</script>

<div class="admin-layout">
	<div class="admin-tabs" role="tablist" aria-label="Admin sections">
		{#each adminTabs as tab (tab.href)}
			<a
				href={tab.href}
				class="admin-tab"
				class:admin-tab--active={$page.url.pathname === tab.href}
			>
				{tab.label}
			</a>
		{/each}
	</div>
	<div class="admin-content">
		<slot />
	</div>
</div>

<style>
	.admin-layout {
		display: flex;
		flex-direction: column;
		gap: var(--space-4);
	}

	.admin-tabs {
		display: flex;
		gap: var(--space-1);
		border-bottom: 1px solid var(--color-border-subtle);
		padding-bottom: var(--space-1);
	}

	.admin-tab {
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

	.admin-tab:hover {
		color: var(--color-text-primary);
		background-color: var(--color-surface-raised);
	}

	.admin-tab--active {
		color: var(--color-brand-lighter);
		border-bottom: 2px solid var(--color-brand-lighter);
		margin-bottom: -1px;
	}

	.admin-content {
		display: flex;
		flex-direction: column;
	}
</style>
