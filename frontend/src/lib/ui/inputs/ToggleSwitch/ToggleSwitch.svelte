<script lang="ts">
	export let checked: boolean = false;

	export let id: string;
	export let label: string = '';
	export let testId: string = id;

	export let onClick: (value: boolean) => void = () => {};

	function handleClicked(event) {
		const state = event.target.getAttribute('aria-checked');

		checked = state !== 'true';

		onClick(checked);
	}
</script>

<div data-testid={testId} class="slider">
	<span class="slider__label" {id}>{label}</span>
	<button
		class="slider__button"
		role="switch"
		aria-checked={checked}
		aria-labelledby={id}
		on:click={handleClicked}
	></button>
</div>

<style>
	.slider {
		display: flex;
		align-items: center;
		gap: var(--space-3);
	}

	.slider__label {
		font-size: var(--font-size-md);
		color: var(--color-text-primary);
	}

	.slider__button {
		width: 3em;
		height: 1.6em;
		position: relative;
		background: var(--color-border-default);
		border: none;
		border-radius: var(--radius-full);
		cursor: pointer;
		transition: background-color 0.2s ease;
	}

	.slider__button::before {
		content: '';
		position: absolute;
		width: 1.3em;
		height: 1.3em;
		background: var(--color-text-secondary);
		top: 0.13em;
		left: 0.13em;
		transition: transform 0.2s ease;
		border-radius: var(--radius-full);
	}

	.slider__button[aria-checked='true'] {
		background-color: var(--color-success);
	}

	.slider__button[aria-checked='true']::before {
		background: #fff;
		transform: translateX(1.3em);
	}

	.slider__button:focus-visible {
		outline: 2px solid var(--color-brand-lighter);
		outline-offset: 2px;
	}
</style>
