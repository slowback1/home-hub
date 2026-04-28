<script lang="ts">
	import { type ChipProps, ChipVariant } from '$lib/ui/containers/chip/chipTypes';
	import { ChipSize } from '$lib/ui/containers/chip/chipTypes.js';

	let props: ChipProps = $props();

	let testId = props.testId ?? 'chip';
	let variant = props.variant ?? ChipVariant.Primary;
	let size = props.size ?? ChipSize.Medium;

	function isVariant(givenVariant: ChipVariant): boolean {
		return givenVariant === variant;
	}
	function isSize(givenSize: ChipSize): boolean {
		return givenSize === size;
	}
</script>

<div
	data-testid={testId}
	class="chip chip__base"
	class:chip__primary={isVariant(ChipVariant.Primary)}
	class:chip__secondary={isVariant(ChipVariant.Secondary)}
	class:chip__success={isVariant(ChipVariant.Success)}
	class:chip__error={isVariant(ChipVariant.Error)}
	class:chip__outline={isVariant(ChipVariant.Outline)}
	class:chip__small={isSize(ChipSize.Small)}
	class:chip__medium={isSize(ChipSize.Medium)}
	class:chip__large={isSize(ChipSize.Large)}
>
	{props.text}
</div>

<style>
	.chip {
		--chip-background-color: var(--color-surface-overlay);
		--chip-color: var(--color-text-primary);
		--chip-border-color: var(--color-border-default);

		display: inline-block;
		padding: var(--space-1) var(--space-3);
		border-radius: var(--radius-full);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		background-color: var(--chip-background-color);
		color: var(--chip-color);
		border: 1px solid var(--chip-border-color);
	}

	.chip__primary {
		--chip-background-color: var(--color-brand);
		--chip-color: #fff;
		--chip-border-color: var(--color-brand-light);
	}

	.chip__secondary {
		--chip-background-color: var(--color-surface-raised);
		--chip-color: var(--color-text-secondary);
		--chip-border-color: var(--color-border-subtle);
	}

	.chip__outline {
		--chip-background-color: transparent;
		--chip-color: var(--color-text-primary);
		--chip-border-color: var(--color-border-default);
	}

	.chip__success {
		--chip-background-color: var(--color-success-surface);
		--chip-color: var(--color-success-text);
		--chip-border-color: var(--color-success);
	}

	.chip__error {
		--chip-background-color: var(--color-error-surface);
		--chip-color: var(--color-error-text);
		--chip-border-color: var(--color-error);
	}

	.chip__small {
		font-size: var(--font-size-xs);
		padding: 2px var(--space-2);
	}

	.chip__large {
		font-size: var(--font-size-md);
		padding: var(--space-1) var(--space-4);
	}
</style>
