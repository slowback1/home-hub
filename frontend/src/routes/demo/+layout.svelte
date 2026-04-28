<script lang="ts">
	import Tabs from '$lib/ui/navigation/Tabs.svelte';
	import { page } from '$app/stores';
	import { derived } from 'svelte/store';

	const demoTabs = [
		{ id: '/demo/form', label: 'Form' },
		{ id: '/demo/list', label: 'List' },
		{ id: '/demo/content', label: 'Content' }
	];

	const activeTab = derived(page, ($page) => $page.url.pathname);
</script>

<div class="demo-layout">
	<Tabs
		tabs={demoTabs}
		activeTab={$activeTab}
		onTabChange={(id) => history.pushState({}, '', id)}
	/>
	<div class="demo-content">
		<slot />
	</div>
</div>

<style>
	.demo-layout {
		display: flex;
		flex-direction: column;
		gap: var(--space-6);
	}

	.demo-content {
		padding-top: var(--space-4);
	}
</style>
