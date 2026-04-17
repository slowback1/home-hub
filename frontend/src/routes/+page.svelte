<script lang="ts">
	import { FeatureFlags } from '$lib/services/FeatureFlag/FeatureFlags';
	import FeatureToggle from '$lib/utils/FeatureToggle.svelte';
</script>

<svelte:head>
	<title>Svelte Starter Kit</title>
</svelte:head>

<h1>Welcome to SvelteKit</h1>
<p>Visit <a href="https://kit.svelte.dev">kit.svelte.dev</a> to read the documentation</p>

<p>
	Visit <a href="demo/content">The demo pages</a> to see some of the functionalities of the starter kit!
</p>

<FeatureToggle featureFlag={FeatureFlags.DEMO_FEATURE_FLAG}>
	<p slot="enabled">Looks like you know about feature flags!</p>
</FeatureToggle>
