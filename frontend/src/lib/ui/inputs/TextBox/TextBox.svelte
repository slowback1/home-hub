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
	.text-box {
		padding: 4px;
		background-color: transparent;
		color: var(--color-font);
		border: 1px solid var(--color-font);
		border-radius: 4px;
	}
</style>
