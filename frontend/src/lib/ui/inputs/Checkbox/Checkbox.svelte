<script lang="ts">
	export let label: string = '';
	export let id: string = '';
	export let testId: string = '';
	export let checked = false;

	$: usedTestId = testId || id;
</script>

<div class="checkbox">
	<input
		class="checkbox__input screen-reader-only"
		bind:checked
		type="checkbox"
		{id}
		data-testid={usedTestId}
	/>
	<label class="checkbox__label" data-testid={`${usedTestId}-label`} for={id}>{label}</label>
</div>

<style>
	.checkbox {
		--checkbox-size: 20px;
		--checkbox-border-color: var(--color-font);
		--checkbox-fill: var(--color-success-light-background);
		--checkbox-focus-outline-color: color-mix(
			in lab,
			var(--color-font) 40%,
			var(--color-background)
		);
	}

	.checkbox__label {
		user-select: none;
		cursor: pointer;
	}

	.checkbox__label:before {
		content: '';
		display: inline-block;
		vertical-align: middle;
		width: var(--checkbox-size);
		height: var(--checkbox-size);
		border: 2px solid var(--checkbox-border-color);
		margin-right: calc(var(--checkbox-size) / 2);
	}

	.checkbox__input:checked + .checkbox__label:before {
		background-color: var(--checkbox-fill);
		border-color: color-mix(in lab, var(--checkbox-fill) 20%, var(--checkbox-border-color));
	}

	.checkbox__input:focus + .checkbox__label {
		outline: 2px dashed var(--checkbox-focus-outline-color);
		outline-offset: 12px;
	}

	.checkbox__input:focus:not(:focus-visible) {
		outline: none;
	}
</style>
