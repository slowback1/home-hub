<script lang="ts">
	import { onMount } from 'svelte';
	import Spinner from '$lib/ui/feedback/Spinner.svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';
	import ToggleSwitch from '$lib/ui/inputs/ToggleSwitch/ToggleSwitch.svelte';
	import FeatureFlagApi from '$lib/api/FeatureFlagApi';
	import type { FeatureFlag } from '$lib/services/FeatureFlag/IFeatureFlagProvider';
	import ToastService, { ToastVariant } from '$lib/ui/containers/toast/ToastService';

	const api = new FeatureFlagApi();
	const toastService = new ToastService();

	let loading = true;
	let flags: FeatureFlag[] = [];

	onMount(async () => {
		try {
			flags = await api.getAll();
		} catch {
			// no-op: empty list is acceptable on load failure
		} finally {
			loading = false;
		}
	});

	function formatName(name: string): string {
		return name
			.split('_')
			.map((word) => word.charAt(0).toUpperCase() + word.slice(1).toLowerCase())
			.join(' ');
	}

	async function handleToggle(flag: FeatureFlag, newValue: boolean) {
		try {
			const updated = await api.toggle(flag.name, newValue);
			flags = flags.map((f) => (f.name === flag.name ? updated : f));
			toastService.AddToast({ message: 'Flag updated.', variant: ToastVariant.success });
		} catch {
			flags = [...flags];
			toastService.AddToast({ message: 'Failed to update flag.', variant: ToastVariant.error });
		}
	}
</script>

<svelte:head>
	<title>HomeHub — Admin: Feature Flags</title>
</svelte:head>

{#if loading}
	<Spinner />
{:else}
	<Heading level={1}>Feature Flags</Heading>
	<div class="flags-list">
		{#each flags as flag (flag.name)}
			<div class="flag-row" data-testid="flag-row-{flag.name}">
				<ToggleSwitch
					id="flag-toggle-{flag.name}"
					testId="flag-toggle-{flag.name}"
					label={formatName(flag.name)}
					checked={flag.isEnabled}
					onClick={(value) => handleToggle(flag, value)}
				/>
			</div>
		{/each}
	</div>
{/if}

<style>
	.flags-list {
		display: flex;
		flex-direction: column;
		gap: var(--space-4);
		margin-top: var(--space-4);
	}

	.flag-row {
		padding: var(--space-3) var(--space-4);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-sm);
		background-color: var(--color-surface-base);
	}
</style>
