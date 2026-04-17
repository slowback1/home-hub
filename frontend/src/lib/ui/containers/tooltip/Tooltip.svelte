<script lang="ts">
	export let text: string;

	const fadeDurationInMs = 300;

	let tooltipShown = false;
	let tooltipFading = false;

	const onMouseEnter = () => {
		tooltipShown = true;
	};

	const onMouseLeave = () => {
		tooltipFading = true;

		setTimeout(() => {
			tooltipFading = false;
			tooltipShown = false;
		}, fadeDurationInMs);
	};
</script>

<div
	class="tooltip__content"
	data-testid="tooltip-content"
	on:mouseenter={onMouseEnter}
	on:mouseleave={onMouseLeave}
	role="tooltip"
>
	<slot>Slot Content</slot>

	{#if tooltipShown}
		<span class:tooltip__fading={tooltipFading} class="tooltip" data-testid="tooltip">
			{text}
		</span>
	{/if}
</div>

<style>
	@keyframes tooltip-fade-in {
		from {
			opacity: 0;
		}
		to {
			opacity: 1;
		}
	}

	@keyframes tooltip-fade-out {
		from {
			opacity: 1;
		}
		to {
			opacity: 0;
		}
	}

	.tooltip__content {
		position: relative;
		display: inline-block;
	}

	.tooltip {
		position: absolute;
		background-color: color-mix(in lab, var(--color-background) 80%, var(--color-font));
		color: color-mix(in lab, var(--color-background) 20%, var(--color-font));
		padding: 5px;
		border-radius: 5px;
		z-index: 10;
		top: 150%;
		left: 0;
		white-space: nowrap;
		animation: tooltip-fade-in 500ms ease-in-out;
		animation-fill-mode: forwards;
	}

	.tooltip__fading {
		animation: tooltip-fade-out 500ms ease-in-out;
		animation-fill-mode: forwards;
	}
</style>
