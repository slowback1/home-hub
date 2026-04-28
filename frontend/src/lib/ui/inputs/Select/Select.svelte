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
	.select__group {
		display: flex;
		flex-direction: column;
		gap: var(--space-1);
		align-items: flex-start;
	}

	.select__label {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-text-secondary);
	}

	.select__input {
		padding: var(--space-2) var(--space-3);
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-sm);
		background-color: var(--color-surface-raised);
		color: var(--color-text-primary);
		font-family: var(--font-family-primary);
		font-size: var(--font-size-md);
		transition: border-color 0.15s ease;
	}

	.select__input:focus,
	.select__input:active {
		outline: none;
		border-color: var(--color-brand-lighter);
	}
</style>
