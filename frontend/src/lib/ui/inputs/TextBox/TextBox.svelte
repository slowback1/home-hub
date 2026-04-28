<script lang="ts">
	import { slugify } from '$lib/utils/stringUtils';

	export let type = 'text';
	export let label = '';
	export let onChange: (event: Event) => void = () => {};
	export let id = slugify(label);
	export let value = '';
	let boundValue = '';

	$: updateBound(value);

	function updateBound(newValue: string) {
		boundValue = newValue;
	}

	const handleInput = (e) => {
		value = type.match(/^(number|range)$/) ? +e.target.value : e.target.value;
	};
</script>

<label class="text-box__label" data-testid={id + '-label'} for={id}>
	{label}
</label>
<input
	class="text-box"
	data-testid={id}
	{type}
	value={boundValue}
	on:input={handleInput}
	on:change={onChange}
	{id}
	{...$$restProps}
/>

<style>
	.text-box__label {
		display: block;
		margin-bottom: var(--space-1);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-text-secondary);
	}

	.text-box {
		width: 100%;
		padding: var(--space-2) var(--space-3);
		background-color: var(--color-surface-raised);
		color: var(--color-text-primary);
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-sm);
		font-family: var(--font-family-primary);
		font-size: var(--font-size-md);
		transition: border-color 0.15s ease;
	}

	.text-box:focus {
		outline: none;
		border-color: var(--color-brand-lighter);
	}

	.text-box::placeholder {
		color: var(--color-text-disabled);
	}
</style>
