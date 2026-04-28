<script lang="ts">
	import type { ButtonSize, ButtonVariant } from '$lib/ui/buttons/Button/ButtonTypes';

	export let testId: string = '';
	export let variant: ButtonVariant = 'primary';
	export let size: ButtonSize = 'medium';
	export let href: string = undefined;
	export let disabled: boolean = false;
	export let tabIndex: number = 0;
	export let onClick: (event: Event) => void = () => {};

	const isSecondary = variant === 'secondary';
	const isPrimary = variant === 'primary';
	const isText = variant === 'text';

	const isSmall = size === 'small';
	const isLarge = size === 'large';

	const tag = href ? 'a' : 'button';
	const role = tag === 'a' ? 'link' : 'button';
</script>

<svelte:element
	this={tag}
	class="button"
	class:button-large={isLarge}
	class:button-small={isSmall}
	class:button-primary={isPrimary}
	class:button-secondary={isSecondary}
	class:button-text={isText}
	{href}
	on:click={onClick}
	{disabled}
	data-testid={testId}
	{role}
	tabindex={tabIndex}
>
	<slot>Button Content Goes Here</slot>
</svelte:element>

<style>
	.button {
		display: inline-flex;
		align-items: center;
		justify-content: center;
		gap: var(--space-2);
		border-radius: var(--radius-sm);
		border: 1px solid transparent;
		padding: var(--space-2) var(--space-4);
		font-family: var(--font-family-primary);
		font-size: var(--font-size-md);
		font-weight: var(--font-weight-semibold);
		line-height: var(--line-height-tight);
		text-decoration: none;
		cursor: pointer;
		transition:
			background-color 0.15s ease,
			border-color 0.15s ease,
			color 0.15s ease;
	}

	.button:disabled {
		cursor: not-allowed;
		opacity: 0.45;
	}

	/* Size variants */
	.button-small {
		padding: var(--space-1) var(--space-3);
		font-size: var(--font-size-sm);
	}

	.button-large {
		padding: var(--space-3) var(--space-6);
		font-size: var(--font-size-lg);
	}

	/* Primary */
	.button-primary {
		background-color: var(--color-brand-lighter);
		border-color: var(--color-brand-lighter);
		color: #fff;
	}

	.button-primary:hover:not(:disabled),
	.button-primary:focus:not(:disabled) {
		background-color: var(--color-brand-light);
		border-color: var(--color-brand-light);
	}

	/* Secondary */
	.button-secondary {
		background-color: transparent;
		border-color: var(--color-border-default);
		color: var(--color-text-primary);
	}

	.button-secondary:hover:not(:disabled),
	.button-secondary:focus:not(:disabled) {
		background-color: var(--color-surface-raised);
		border-color: var(--color-brand-lighter);
		color: var(--color-brand-lighter);
	}

	/* Text */
	.button-text {
		background-color: transparent;
		border-color: transparent;
		color: var(--color-text-secondary);
		padding-left: var(--space-2);
		padding-right: var(--space-2);
	}

	.button-text:hover:not(:disabled),
	.button-text:focus:not(:disabled) {
		color: var(--color-brand-lighter);
	}
</style>
