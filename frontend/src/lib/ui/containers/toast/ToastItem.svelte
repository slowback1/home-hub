<script lang="ts">
	import { type ToastConfig, ToastVariant } from '$lib/ui/containers/toast/ToastService';
	import Button from '$lib/ui/buttons/Button/Button.svelte';

	export let config: ToastConfig;
	export let onClose: () => void;
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
</style>
