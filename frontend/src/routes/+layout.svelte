<script lang="ts">
	import { onMount } from 'svelte';
	import MessageBus from '$lib/bus/MessageBus';
	import Header from '$lib/components/navigation/Header.svelte';
	import UrlPathProvider, { RealUrlProvider } from '$lib/providers/urlPathProvider';
	import { ColorTheme } from '$lib/services/Theme/ThemeService';
	import { Messages } from '$lib/bus/Messages';
	import ConfigService from '$lib/services/Config/ConfigService';
	import ToastWrapper from '$lib/ui/containers/toast/ToastWrapper.svelte';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
	import ConfigFeatureFlagProvider from '$lib/services/FeatureFlag/ConfigFeatureFlagProvider';
	import LocalStorageProvider from '$lib/bus/providers/localStorageProvider';

	let currentTheme: ColorTheme = ColorTheme.Light;

	onMount(() => {
		MessageBus.initialize(new LocalStorageProvider());
		UrlPathProvider.initialize(new RealUrlProvider());
		ConfigService.initialize();
		FeatureFlagService.initialize(new ConfigFeatureFlagProvider());

		MessageBus.subscribe<ColorTheme>(
			Messages.CurrentTheme,
			(value) => (currentTheme = value ?? ColorTheme.Light)
		);
	});
</script>

<svelte:head>
	<meta name="description" content="The Svelte Starter Kit!!!" />
</svelte:head>

<div
	class:light-theme={currentTheme === ColorTheme.Light}
	class:dark-theme={currentTheme === ColorTheme.Dark}
>
	<ToastWrapper />
	<Header />
	<main id="content" class="main-content">
		<slot />
	</main>
</div>

<style global>
	@import '../style/reset.css';
	@import '../style/globals.css';

	.main-content {
		min-height: calc(100vh - var(--gutters-y) * 2 - var(--header-height));
		padding: var(--gutters-y) var(--gutters-x);
		scroll-behavior: auto;
		display: flex;
		flex-direction: column;
	}
</style>
