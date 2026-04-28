<script lang="ts">
	type Tab = { id: string; label: string };

	export let tabs: Tab[] = [];
	export let activeTab: string = tabs[0]?.id ?? '';
	export let onTabChange: (id: string) => void = () => {};

	function selectTab(id: string) {
		activeTab = id;
		onTabChange(id);
	}
</script>

<div class="tabs">
	<div class="tabs__list" role="tablist">
		{#each tabs as tab (tab.id)}
			<button
				class="tabs__tab"
				class:tabs__tab--active={activeTab === tab.id}
				role="tab"
				aria-selected={activeTab === tab.id}
				onclick={() => selectTab(tab.id)}
			>
				{tab.label}
			</button>
		{/each}
	</div>
</div>

<style>
	.tabs {
		display: flex;
		flex-direction: column;
	}

	.tabs__list {
		display: flex;
		gap: var(--space-1);
		border-bottom: 1px solid var(--color-border-subtle);
		padding-bottom: var(--space-1);
	}

	.tabs__tab {
		padding: var(--space-2) var(--space-4);
		background: none;
		border: none;
		border-radius: var(--radius-sm) var(--radius-sm) 0 0;
		color: var(--color-text-secondary);
		font-family: var(--font-family-primary);
		font-size: var(--font-size-md);
		font-weight: var(--font-weight-medium);
		cursor: pointer;
		transition:
			color 0.15s ease,
			background-color 0.15s ease;
	}

	.tabs__tab:hover {
		color: var(--color-text-primary);
		background-color: var(--color-surface-raised);
	}

	.tabs__tab--active {
		color: var(--color-brand-lighter);
		border-bottom: 2px solid var(--color-brand-lighter);
		margin-bottom: -1px;
	}
</style>
