<script>
	import FeatureToggle from '$lib/utils/FeatureToggle.svelte';
</script>

<FeatureToggle featureFlag="test">
	<p data-testid="enabled" slot="enabled">Enabled</p>
	<p data-testid="disabled" slot="disabled">Disabled</p>
</FeatureToggle>
