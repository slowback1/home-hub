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
		width: clamp(200px, 25vw, 500px);
		display: flex;
		flex-direction: row;
		align-items: center;
		justify-content: space-between;
		padding: 8px 4px;
	}

	.toast-item__error {
		background-color: var(--color-error-light-background);
		color: var(--color-error-light-font);
	}

	.toast-item__warning {
		background-color: var(--color-warning-light-background);
		color: var(--color-warning-light-font);
	}

	.toast-item__success {
		background-color: var(--color-success-light-background);
		color: var(--color-success-light-font);
	}

	.toast-item__info {
		background-color: var(--color-info-light-background);
		color: var(--color-info-light-font);
	}
</style>
