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
	.alert-base {
		--border-width: 2px;
		--alert-padding-y: 1rem;
		--alert-padding-x: 1.25rem;
		--alert-font-weight: 400;
		--alert-background-color: var(--color-background);
		--alert-font-color: var(--color-font);
		--alert-border-color: var(--alert-font-color);
	}

	.alert__info {
		--alert-background-color: var(--color-info-light-background);
		--alert-font-color: var(--color-info-light-font);
	}

	.alert__warning {
		--alert-background-color: var(--color-warning-light-background);
		--alert-font-color: var(--color-warning-light-font);
	}

	.alert__error {
		--alert-background-color: var(--color-error-light-background);
		--alert-font-color: var(--color-error-light-font);
	}

	.alert {
		border: var(--border-width) solid var(--alert-border-color);
		border-radius: 0.25rem;
		padding: var(--alert-padding-y) var(--alert-padding-x);
		font-weight: var(--alert-font-weight);
		background-color: var(--alert-background-color);
		color: var(--alert-font-color);
		position: relative;
	}

	.alert__close-button {
		background-color: transparent;
		border: none;
		color: var(--alert-font-color);
		cursor: pointer;
		font-size: 1rem;
		margin-left: 0.5rem;
		padding: calc(var(--alert-padding-y) - 4px);
		position: absolute;
		right: 2rem;
		/* To offset the border width */
		top: 2px;
	}

	.alert__close-button:hover,
	.alert__close-button:focus {
		background-color: var(--alert-font-color);
		color: var(--alert-background-color);
	}
</style>
