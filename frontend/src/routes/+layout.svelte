<script lang="ts">
	import { onMount } from 'svelte';
	import MessageBus from '$lib/bus/MessageBus';
	import UrlPathProvider, { RealUrlProvider } from '$lib/providers/urlPathProvider';
	import ConfigService from '$lib/services/Config/ConfigService';
	import ToastWrapper from '$lib/ui/containers/toast/ToastWrapper.svelte';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
	import ConfigFeatureFlagProvider from '$lib/services/FeatureFlag/ConfigFeatureFlagProvider';
	import LocalStorageProvider from '$lib/bus/providers/localStorageProvider';
	import Sidebar from '$lib/ui/navigation/Sidebar.svelte';

	onMount(() => {
		MessageBus.initialize(new LocalStorageProvider());
		UrlPathProvider.initialize(new RealUrlProvider());
		ConfigService.initialize();
		FeatureFlagService.initialize(new ConfigFeatureFlagProvider());
	});
</script>

<svelte:head>
	<meta name="description" content="HomeHub" />
</svelte:head>

<div class="app-shell">
	<Sidebar />
	<ToastWrapper />
	<main id="content" class="main-content">
		<slot />
	</main>
</div>

<style global>
	@import '../style/reset.css';
	@import '../style/globals.css';

	.app-shell {
		display: flex;
		min-height: 100vh;
	}

	.main-content {
		flex: 1;
		padding: var(--space-6);
		display: flex;
		flex-direction: column;
		overflow: auto;
	}
</style>
