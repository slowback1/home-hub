<script lang="ts">
	import { page } from '$app/stores';
	import { goto } from '$app/navigation';
	import Tabs from '$lib/ui/navigation/Tabs.svelte';

	const adminTabs = [
		{ id: '/admin/system-config', label: 'System Config' },
		{ id: '/admin/feature-flags', label: 'Feature Flags' }
	];

	$: activeTab = $page.url.pathname;
</script>

<div class="admin-layout">
	<Tabs tabs={adminTabs} {activeTab} onTabChange={(id) => goto(id)} />
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

	.admin-content {
		display: flex;
		flex-direction: column;
	}
</style>
