<script lang="ts">
	import type { SelectOption } from '$lib/ui/inputs/Select/SelectTypes';

	export let options: SelectOption[] = [];
	export let onChange: (value: string) => void = () => {};
	export let value: string | number = '';
	export let label: string = '';
	export let id: string = '';

	const handleSelect = (event: Event) => {
		const target = event.target as HTMLSelectElement;
		onChange(target.value);
	};

	const handleChange = (event: Event) => {
		const target = event.target as HTMLSelectElement;
		onChange(target.value);
	};
</script>

<div class="select__group select__base">
	<label class="select__label" for={id}>{label}</label>
	<select
		aria-labelledby={id}
		class="select__input"
		{id}
		on:change={handleChange}
		on:select={handleSelect}
		bind:value
		{...$$restProps}
	>
		{#each options as option (option.value)}
			<option selected={value === option.value} value={option.value}>{option.label}</option>
		{/each}
	</select>
</div>

<style>
	.select__base {
		--select-group-flex-direction: column;
		--select-group-gap: 0.5rem;
		--select-group-align-items: flex-start;
		--select-group-justify-content: flex-start;

		--select-label-font-size: 1rem;
		--select-label-font-weight: 500;
		--select-label-font-color: var(--color-font);

		--select-input-padding: 0.5rem;
		--select-input-border-width: 1px;
		--select-input-border-color: var(--color-font);
		--select-input-background-color: var(--color-background);
		--select-input-font-color: var(--color-font);
		--select-input-border-radius: 0.25rem;
		--select-input-font-size: 1rem;
		--select-input-dropdown-margin-right: 0.5rem;
		--select-input-dropdown-focus-outline-color-base: var(--color-base-blue);
		/*To customize the select arrow, base 64 encode an svg and place it here, below is an example*/
		/*--select-input-arrow-icon: url(data:image/svg+xml;base64,PHN2ZyBpZD0iTGF5ZXJfMSIgZGF0YS1uYW1lPSJMYXllciAxIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA0Ljk1IDEwIj48ZGVmcz48c3R5bGU+LmNscy0xe2ZpbGw6I2ZmZjt9LmNscy0ye2ZpbGw6IzQ0NDt9PC9zdHlsZT48L2RlZnM+PHRpdGxlPmFycm93czwvdGl0bGU+PHJlY3QgY2xhc3M9ImNscy0xIiB3aWR0aD0iNC45NSIgaGVpZ2h0PSIxMCIvPjxwb2x5Z29uIGNsYXNzPSJjbHMtMiIgcG9pbnRzPSIxLjQxIDQuNjcgMi40OCAzLjE4IDMuNTQgNC42NyAxLjQxIDQuNjciLz48cG9seWdvbiBjbGFzcz0iY2xzLTIiIHBvaW50cz0iMy41NCA1LjMzIDIuNDggNi44MiAxLjQxIDUuMzMgMy41NCA1LjMzIi8+PC9zdmc+);*/
	}

	.select__group {
		display: flex;
		flex-direction: var(--select-group-flex-direction);
		gap: var(--select-group-gap);
		align-items: var(--select-group-align-items);
		justify-content: var(--select-group-justify-content);
	}

	.select__input {
		padding: var(--select-input-padding);
		border-width: var(--select-input-border-width);
		border-color: var(--select-input-border-color);
		border-radius: var(--select-input-border-radius);
		background-color: var(--select-input-background-color);
		color: var(--select-input-font-color);
		font-size: var(--select-input-font-size);
		background-image: var(--select-input-arrow-icon, inherit);
		background-repeat: no-repeat;
		background-position: right center;
	}

	.select__input:focus,
	.select__input:active {
		outline-color: var(--select-input-dropdown-focus-outline-color-base);
	}

	.select__label {
		font-size: var(--select-label-font-size);
		font-weight: var(--select-label-font-weight);
		color: var(--select-label-font-color);
	}
</style>
