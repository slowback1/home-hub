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
		--checkbox-size: 18px;
		--checkbox-border-color: var(--color-border-default);
		--checkbox-fill: var(--color-success);
		--checkbox-focus-outline-color: var(--color-brand-lighter);
	}

	.checkbox__label {
		display: inline-flex;
		align-items: center;
		gap: var(--space-2);
		user-select: none;
		cursor: pointer;
		font-size: var(--font-size-md);
		color: var(--color-text-primary);
	}

	.checkbox__label:before {
		content: '';
		display: inline-block;
		flex-shrink: 0;
		width: var(--checkbox-size);
		height: var(--checkbox-size);
		border: 2px solid var(--checkbox-border-color);
		border-radius: var(--space-1);
		background-color: var(--color-surface-raised);
		transition:
			background-color 0.15s ease,
			border-color 0.15s ease;
	}

	.checkbox__input:checked + .checkbox__label:before {
		background-color: var(--checkbox-fill);
		border-color: var(--checkbox-fill);
	}

	.checkbox__input:focus-visible + .checkbox__label {
		outline: 2px solid var(--checkbox-focus-outline-color);
		outline-offset: var(--space-1);
		border-radius: var(--space-1);
	}
</style>
