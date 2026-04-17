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
	class="button-base button"
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
	.button-base {
		--button-border-radius: 8px;
		--button-border-width: 1px;
		--button-padding-x: 0.5rem;
		--button-padding-y: 1rem;
		--button-font-size: var(--font-size-medium);
		--button-color: var(--color-background);
		--button-border-color: var(--color-font);
		--button-disabled-opacity: 0.5;
	}

	.button-small {
		--button-font-size: var(--font-size-small);
	}

	.button-large {
		--button-font-size: var(--font-size-large);
	}

	.button-primary {
		--button-color: var(--color-base-blue);
	}

	.button-secondary {
		--button-color: var(--color-base-green);
	}

	.button-text {
		--button-color: var(--color-background);
		--button-border-color: transparent;
	}

	.button-text:hover,
	.button-text:focus {
		--button-color: var(--color-base-blue);
		--button-border-color: var(--color-font);
	}

	.button {
		border-radius: var(--button-border-radius);
		border: var(--button-border-width) solid var(--button-border-color);
		padding: var(--button-padding-x) var(--button-padding-y);
		text-decoration: none;
		font-family: var(--font-family-primary);
		font-size: var(--button-font-size);
		transition: background-color 0.5s ease-in-out;
		background-color: color-mix(in lab, var(--button-color) 80%, var(--color-background));
		color: color-mix(in lab, var(--button-color) 20%, var(--color-font));
	}

	.button:hover,
	.button:focus {
		cursor: pointer;
		background-color: color-mix(in lab, var(--button-color) 95%, var(--color-background));
	}

	.button:disabled {
		cursor: not-allowed;
		opacity: var(--button-disabled-opacity);
	}
</style>
