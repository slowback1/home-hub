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
	.chip__base {
		--chip-padding-x: 8px;
		--chip-padding-y: 4px;
		--chip-border-radius: 12px;
		--chip-font-size: 14px;
		--chip-background-color: #000;
		--chip-color: #fff;
		--chip-border-color: #000;
		--chip-border-width: 2px;
	}

	.chip__primary {
		--chip-background-color: color-mix(
			in lab,
			var(--color-primary-background),
			var(--color-primary-font) 10%
		);
		--chip-color: var(--color-primary-font);
		--chip-border-color: var(--color-primary-border);
	}

	.chip__secondary {
		--chip-background-color: color-mix(
			in lab,
			var(--color-secondary-background),
			var(--color-secondary-font) 10%
		);
		--chip-color: var(--color-secondary-font);
		--chip-border-color: var(--color-secondary-border);
	}

	.chip__outline {
		--chip-background-color: transparent;
		--chip-color: var(--color-font);
		--chip-border-color: var(--color-font);
	}

	.chip__success {
		--chip-background-color: color-mix(
			in lab,
			var(--color-success-light-background),
			var(--color-success-light-font) 10%
		);
		--chip-color: var(--color-success-light-font);
		--chip-border-color: var(--color-success-light-font);
	}

	.chip__error {
		--chip-background-color: color-mix(
			in lab,
			var(--color-error-light-background),
			var(--color-error-light-font) 10%
		);
		--chip-color: var(--color-error-light-font);
		--chip-border-color: var(--color-error-light-font);
	}

	.chip__small {
		--chip-padding-x: 2px;
		--chip-padding-y: 4px;
		--chip-font-size: 12px;
	}

	.chip__medium {
		--chip-padding-x: 8px;
		--chip-padding-y: 4px;
		--chip-font-size: 14px;
	}

	.chip__large {
		--chip-padding-x: 12px;
		--chip-padding-y: 6px;
		--chip-font-size: 16px;
	}

	.chip {
		display: inline-block;
		padding: var(--chip-padding-y) var(--chip-padding-x);
		border-radius: var(--chip-border-radius);
		font-size: var(--chip-font-size);
		background-color: var(--chip-background-color);
		color: var(--chip-color);
		border: var(--chip-border-width) solid var(--chip-border-color);
	}
</style>
