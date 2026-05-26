<script lang="ts">
	import { onMount } from 'svelte';
	import { type ToastConfig, ToastVariant } from '$lib/ui/containers/toast/ToastService';
	import Button from '$lib/ui/buttons/Button/Button.svelte';

	export let config: ToastConfig;
	export let onClose: () => void;

	onMount(() => {
		if (!config.durationMs) return;
		const timer = setTimeout(onClose, config.durationMs);
		return () => clearTimeout(timer);
	});
</script>

<div
	data-testid="toast-item"
	class="toast-item"
	class:toast-item__error={config.variant === ToastVariant.error}
	class:toast-item__success={config.variant === ToastVariant.success}
	class:toast-item__info={(config.variant ?? ToastVariant.info) === ToastVariant.info}
	class:toast-item__warning={config.variant === ToastVariant.warning}
>
	<span class="toast-item__text">
		{config.message}
	</span>

	{#if config.action}
		<button class="toast-item__action" data-testid="toast-action" on:click={config.action.onClick}>
			{config.action.label}
		</button>
	{/if}

	<Button variant="text" size="small" onClick={onClose} testId="toast-item__close">X</Button>
</div>

<style>
	.toast-item {
		--toast-background: var(--color-surface-overlay);
		--toast-color: var(--color-text-primary);
		--toast-border: var(--color-border-default);

		width: clamp(200px, 25vw, 500px);
		display: flex;
		flex-direction: row;
		align-items: center;
		justify-content: space-between;
		gap: var(--space-3);
		padding: var(--space-3) var(--space-4);
		border-radius: var(--radius-md);
		border: 1px solid var(--toast-border);
		background-color: var(--toast-background);
		color: var(--toast-color);
	}

	.toast-item__error {
		--toast-background: var(--color-error-surface);
		--toast-color: var(--color-error-text);
		--toast-border: var(--color-error);
	}

	.toast-item__warning {
		--toast-background: var(--color-warning-surface);
		--toast-color: var(--color-warning-text);
		--toast-border: var(--color-warning);
	}

	.toast-item__success {
		--toast-background: var(--color-success-surface);
		--toast-color: var(--color-success-text);
		--toast-border: var(--color-success);
	}

	.toast-item__info {
		--toast-background: var(--color-info-surface);
		--toast-color: var(--color-info-text);
		--toast-border: var(--color-info);
	}

	.toast-item__action {
		background: none;
		border: none;
		color: var(--color-brand-lighter);
		cursor: pointer;
		font-family: var(--font-family-primary);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-semibold);
		padding: 0;
		white-space: nowrap;
	}

	.toast-item__action:hover {
		text-decoration: underline;
	}
</style>
