<script lang="ts">
	import { onMount } from 'svelte';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';

	export let featureFlag: string;
	let isEnabled = false;

	onMount(() => {
		let unsubscribe = FeatureFlagService.subscribeToFeature(featureFlag, (value) => {
			isEnabled = value;
		});

		return () => {
			unsubscribe();
		};
	});
</script>

{#if isEnabled}
	<slot name="enabled" />
{/if}

{#if !isEnabled}
	<slot name="disabled" />
{/if}
