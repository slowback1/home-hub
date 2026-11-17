<script lang="ts">
	import { onMount } from 'svelte';
	import { page } from '$app/stores';
	import MessageBus from '$lib/bus/MessageBus';
	import UrlPathProvider, { RealUrlProvider } from '$lib/providers/urlPathProvider';
	import ConfigService from '$lib/services/Config/ConfigService';
	import ToastWrapper from '$lib/ui/containers/toast/ToastWrapper.svelte';
	import { preventToastPersistence } from '$lib/ui/containers/toast/ToastService';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
	import ApiFeatureFlagProvider from '$lib/services/FeatureFlag/ApiFeatureFlagProvider';
	import LocalStorageProvider from '$lib/bus/providers/localStorageProvider';
	import Sidebar from '$lib/ui/navigation/Sidebar.svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import UserService from '$lib/services/User/UserService';
	import UserSwitcher from '$lib/ui/user/UserSwitcher.svelte';
	import { Messages } from '$lib/bus/Messages';

	let ready = false;
	let currentUserId: string | null = null;
	const userService = new UserService();

	onMount(async () => {
		MessageBus.initialize(new LocalStorageProvider());
		preventToastPersistence();
		UrlPathProvider.initialize(new RealUrlProvider());
		await ConfigService.initialize();
		await FeatureFlagService.initialize(new ApiFeatureFlagProvider());
		await UserService.initialize();
		MessageBus.subscribe<string | null>(Messages.CurrentUser, () => {
			currentUserId = userService.getCurrentUserId();
		});
		ready = true;
	});
</script>

<svelte:head>
	<meta name="description" content="HomeHub" />
</svelte:head>

{#if ready && currentUserId}
	{#if $page.url.searchParams.has('fullscreen')}
		<div class="app-shell app-shell--fullscreen" data-testid="app-shell-fullscreen">
			<ToastWrapper />
			<main id="content" class="main-content main-content--fullscreen">
				<slot />
			</main>
		</div>
	{:else}
		<div class="app-shell">
			<Sidebar currentPath={$page.url.pathname} />
			<ToastWrapper />
			<main id="content" class="main-content">
				<slot />
			</main>
		</div>
	{/if}
{:else if ready}
	<UserSwitcher users={userService.getUsers()} />
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

	.main-content--fullscreen {
		padding: 0;
	}

	.loading-shell {
		display: flex;
		align-items: center;
		justify-content: center;
		min-height: 100vh;
	}
</style>
