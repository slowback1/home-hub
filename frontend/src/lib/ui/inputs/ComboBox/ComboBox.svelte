<script lang="ts">
	import ComboBoxService, {
		type ComboBoxOption,
		type ComboBoxOptionOutput
	} from '$lib/ui/inputs/ComboBox/ComboBoxService';
	import { reactiveInstanceOf } from '$lib/utils/reactiveClasses/reactiveInstanceOf';
	import { slugify } from '$lib/utils/stringUtils';

	export let label: string;
	export let onSelect: (value: never) => void;
	export let options: ComboBoxOption<never>[] = [];
	export let testId: string = '';

	const comboBoxService = reactiveInstanceOf(ComboBoxService, options, onSelect);

	function getOptionId(option: ComboBoxOptionOutput<never>) {
		return `${testId}-${slugify(option.value)}`;
	}
</script>

<div
	on:keyup={(e) => {
		comboBoxService.handleKeyboardEvent(e, $comboBoxService.focusedOption);
	}}
	data-testid={testId}
	class="combo-box"
	role="combobox"
	tabindex={0}
	aria-controls={`${testId}__listbox`}
	aria-expanded={$comboBoxService.isOpen ? 'true' : 'false'}
>
	<label class="combo-box__label" for={`${testId}__input`} data-testid={`${testId}__label`}>
		{label}
	</label>
	<div class="combo-box__input-group">
		<input
			on:input={(e) => comboBoxService.onInputChange(e as unknown as { target: { value: string } })}
			value={$comboBoxService.value}
			id={`${testId}__input`}
			data-testid={`${testId}__input`}
			class="combo-box__input"
			aria-activedescendant={$comboBoxService.focusedOption
				? getOptionId($comboBoxService.focusedOption)
				: ``}
		/>
		<button
			aria-label={label}
			aria-controls={`${testId}__listbox`}
			aria-expanded={$comboBoxService.isOpen ? 'true' : 'false'}
			type="button"
			on:click={() => comboBoxService.toggleIsOpen()}
			data-testid={`${testId}__toggle`}
			class="combo-box__toggle"
		>
			<svg
				width="18"
				height="16"
				aria-hidden="true"
				focusable="false"
				style="forced-color-adjust: auto"
			>
				<polygon
					class="arrow"
					stroke-width="0"
					fill-opacity="0.75"
					fill="currentcolor"
					points="3,6 15,6 9,14"
				></polygon>
			</svg>
		</button>
	</div>
	{#if $comboBoxService.isOpen}
		<ul
			id={`${testId}__listbox`}
			data-testid={`${testId}__options`}
			role="listbox"
			aria-label={label}
			class="combo-box__option-list"
		>
			{#each $comboBoxService.displayedOptions as option (option.value)}
				<li
					data-testid={`${testId}__option`}
					role="option"
					aria-selected={$comboBoxService.focusedOption?.id === option.id}
					id={getOptionId(option)}
					class="combo-box__option"
					on:click={() => comboBoxService.handleSelect(option)}
					on:mouseenter={() => comboBoxService.setFocus(option)}
					class:combo-box__option-focused={$comboBoxService.focusedOption.id === option.id}
					tabindex="0"
					on:keydown={(e) => comboBoxService.handleKeyboardEvent(e, option)}
				>
					{option.label}
				</li>
			{/each}
		</ul>
	{/if}
</div>

<style>
	.combo-box {
		position: relative;
		max-width: fit-content;
	}

	.combo-box__label {
		display: block;
		margin-bottom: var(--space-1);
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-text-secondary);
	}

	.combo-box__input-group {
		display: flex;
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-sm);
		background-color: var(--color-surface-raised);
		overflow: hidden;
		transition: border-color 0.15s ease;
	}

	.combo-box__input-group:has(:focus-visible, :focus) {
		outline: none;
		border-color: var(--color-brand-lighter);
	}

	.combo-box__input-group > * {
		padding: var(--space-2) var(--space-3);
		margin: 0;
	}

	.combo-box__input {
		border: 0;
		background-color: transparent;
		color: var(--color-text-primary);
		font-family: var(--font-family-primary);
		font-size: var(--font-size-md);
		flex: 1;
	}

	.combo-box__input:focus {
		outline: none;
	}

	.combo-box__toggle {
		border: 0;
		background-color: transparent;
		color: var(--color-text-secondary);
		cursor: pointer;
		padding: var(--space-2);
	}

	.combo-box__toggle:hover {
		color: var(--color-text-primary);
	}

	.combo-box__option-list {
		position: absolute;
		right: 0;
		top: calc(100% + var(--space-1));
		background-color: var(--color-surface-overlay);
		border: 1px solid var(--color-border-default);
		border-radius: var(--radius-sm);
		width: 100%;
		color: var(--color-text-primary);
		z-index: 10;
		list-style: none;
		padding: var(--space-1) 0;
	}

	.combo-box__option {
		padding: var(--space-2) var(--space-3);
		cursor: pointer;
		font-size: var(--font-size-md);
	}

	.combo-box__option-focused {
		background-color: var(--color-surface-raised);
		color: var(--color-brand-lighter);
	}
</style>
