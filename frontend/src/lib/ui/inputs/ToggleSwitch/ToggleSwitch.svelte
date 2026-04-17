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

		--slider-background-color: #565656;
		--slider-accent-color: #32ab32;
		--slider-button-color: #bdbdbd;
	}

	.slider__button {
		width: 3em;
		height: 1.6em;
		position: relative;
		margin: 0 0 0 0.5em;
		background: var(--slider-background-color);
		border: none;
		border-radius: 12px;
	}

	.slider__button::before {
		content: '';
		position: absolute;
		width: 1.3em;
		height: 1.3em;
		background: var(--slider-button-color);
		top: 0.13em;
		right: 1.5em;
		transition: transform 0.3s;
		border-radius: 12px;
	}

	.slider__button[aria-checked='true'] {
		background-color: var(--slider-accent-color);
	}

	.slider__button[aria-checked='true']::before {
		transform: translateX(1.3em);
		transition: transform 0.3s;
	}

	.slider__button:focus {
		box-shadow: 0 0px 0px 1px var(--slider-accent-color);
	}
</style>
