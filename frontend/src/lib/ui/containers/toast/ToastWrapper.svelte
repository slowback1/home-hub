<script lang="ts">
	import ToastService, { type ToastConfig } from '$lib/ui/containers/toast/ToastService';
	import { onMount } from 'svelte';
	import MessageBus from '$lib/bus/MessageBus';
	import { Messages } from '$lib/bus/Messages';
	import ToastItem from '$lib/ui/containers/toast/ToastItem.svelte';

	let toasts: ToastConfig[] = [];

	const service = new ToastService();

	onMount(() => {
		MessageBus.subscribe<ToastConfig[]>(Messages.Toasts, (value) => (toasts = value ?? []));
	});

	function onClose(index: number) {
		service.RemoveToast(index);
	}

	const maxToasts = 3;

	$: mostRecentMessage = toasts.length === 0 ? '' : toasts[toasts.length - 1].message;
	$: oldestToasts = toasts.length > maxToasts ? toasts.slice(0, maxToasts) : toasts;
</script>

<div class="toast-wrapper" data-testid="toast-wrapper">
	<span data-testid="toast-wrapper__live" aria-live="polite" class="screen-reader-only">
		{mostRecentMessage}
	</span>
	{#each oldestToasts as toast, index (toast.message + index)}
		<ToastItem config={toast} onClose={() => onClose(index)} />
	{/each}
</div>

<style>
	.toast-wrapper {
		position: absolute;
		bottom: 12px;
		left: 12px;
		display: flex;
		flex-direction: column;
		gap: 16px;
	}
</style>
