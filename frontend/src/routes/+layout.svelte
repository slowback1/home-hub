<script lang="ts">
	import { onMount } from 'svelte';
	import { page } from '$app/stores';
	import MessageBus from '$lib/bus/MessageBus';
	import UrlPathProvider, { RealUrlProvider } from '$lib/providers/urlPathProvider';
	import ConfigService from '$lib/services/Config/ConfigService';
	import ToastWrapper from '$lib/ui/containers/toast/ToastWrapper.svelte';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
	import ApiFeatureFlagProvider from '$lib/services/FeatureFlag/ApiFeatureFlagProvider';
	import LocalStorageProvider from '$lib/bus/providers/localStorageProvider';
	import Sidebar from '$lib/ui/navigation/Sidebar.svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';

	let ready = false;

	onMount(async () => {
		MessageBus.initialize(new LocalStorageProvider());
		UrlPathProvider.initialize(new RealUrlProvider());
		await ConfigService.initialize();
		FeatureFlagService.initialize(new ApiFeatureFlagProvider());
		ready = true;
	});
</script>

<svelte:head>
	<meta name="description" content="HomeHub" />
</svelte:head>

{#if ready}
	<div class="app-shell">
		<Sidebar currentPath={$page.url.pathname} />
		<ToastWrapper />
		<main id="content" class="main-content">
			<slot />
		</main>
	</div>
{:else}
	<div class="loading-shell">
		<Spinner />
	</div>
{/if}

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

	.loading-shell {
		display: flex;
		align-items: center;
		justify-content: center;
		min-height: 100vh;
	}
</style>
