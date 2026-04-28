<script lang="ts">
	import { FeatureFlags } from '$lib/services/FeatureFlag/FeatureFlags';
	import FeatureToggle from '$lib/utils/FeatureToggle.svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
</script>

<svelte:head>
	<title>HomeHub</title>
</svelte:head>

<Heading level={1}>HomeHub</Heading>
<p>Select a section from the sidebar to get started.</p>

<FeatureToggle featureFlag={FeatureFlags.DEMO_FEATURE_FLAG}>
	<p slot="enabled">Feature flags are active.</p>
</FeatureToggle>
