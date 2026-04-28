<script lang="ts">
	import { type AlertProps, AlertType } from '$lib/ui/containers/alert/alertTypes';

	const { testId, message, type, onClose }: AlertProps = $props();

	const dataTestId = $derived(testId ?? 'alert');
	const alertType = $derived(type ?? AlertType.Info);

	const isInfo = $derived(alertType === AlertType.Info);
	const isWarning = $derived(alertType === AlertType.Warning);
	const isError = $derived(alertType === AlertType.Error);
</script>

<div
	class="alert alert-base"
	class:alert__info={isInfo}
	class:alert__warning={isWarning}
	class:alert__error={isError}
	data-testid={dataTestId}
	role="alert"
>
	{message}

	<button class="alert__close-button" onclick={onClose} data-testid="alert-close">X</button>
</div>

<style>
	.alert {
		--alert-background-color: var(--color-surface-raised);
		--alert-font-color: var(--color-text-primary);
		--alert-border-color: var(--color-border-subtle);

		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: var(--space-4);
		border: 1px solid var(--alert-border-color);
		border-left-width: 4px;
		border-radius: var(--radius-md);
		padding: var(--space-3) var(--space-4);
		background-color: var(--alert-background-color);
		color: var(--alert-font-color);
	}

	.alert__info {
		--alert-background-color: var(--color-info-surface);
		--alert-font-color: var(--color-info-text);
		--alert-border-color: var(--color-info);
	}

	.alert__warning {
		--alert-background-color: var(--color-warning-surface);
		--alert-font-color: var(--color-warning-text);
		--alert-border-color: var(--color-warning);
	}

	.alert__error {
		--alert-background-color: var(--color-error-surface);
		--alert-font-color: var(--color-error-text);
		--alert-border-color: var(--color-error);
	}

	.alert__close-button {
		flex-shrink: 0;
		background-color: transparent;
		border: none;
		color: var(--alert-font-color);
		cursor: pointer;
		font-size: var(--font-size-md);
		padding: var(--space-1);
		border-radius: var(--radius-sm);
		opacity: 0.7;
		transition: opacity 0.15s ease;
	}

	.alert__close-button:hover,
	.alert__close-button:focus {
		opacity: 1;
	}
</style>
