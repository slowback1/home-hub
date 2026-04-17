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
		--label-spacing: 12px;
		--input-group-background: #efefef;
		--input-group-color: #321211;
		--input-group-padding-x: 4px;
		--input-group-padding-y: 8px;
		--focus-outline-color: #5dea5d;
		--focus-outline-size: 2px;
		--options-list-background: #efefef;
		--options-list-background-focus: #817a7a;
		--options-list-color: #030303;
		--options-list-spacing-y: 8px;
		--options-list-spacing-x: 4px;
		--options-list-separator-color: #85d7e3;
		--options-list-separator-size: 4px;

		position: relative;
		max-width: fit-content;
	}

	.combo-box__input-group {
		display: flex;
		margin-top: var(--label-spacing);
	}

	.combo-box__input-group > * {
		padding: var(--input-group-padding-y) var(--input-group-padding-x);
		margin: 0;
	}

	.combo-box__input {
		border-radius: 0;
		border: 0;
		background-color: var(--input-group-background);
		color: var(--input-group-color);
	}

	.combo-box__input:focus {
		outline: none;
	}

	.combo-box__input-group:has(:focus-visible, :focus) {
		outline: var(--focus-outline-size) solid var(--focus-outline-color);
	}

	.combo-box__toggle {
		border-radius: 0;
		border: 0;
		background-color: var(--input-group-background);
		color: var(--input-group-color);
		fill: var(--input-group-color);
	}

	.combo-box__option-list {
		position: absolute;
		right: 0;
		top: calc(100% + var(--options-list-separator-size) - var(--focus-outline-size));
		background-color: var(--options-list-background);
		color: var(--options-list-color);
		width: 100%;
		border-top: var(--options-list-separator-size) solid var(--options-list-separator-color);
	}

	.combo-box__option {
		padding: var(--options-list-spacing-y) var(--options-list-spacing-x);
		cursor: pointer;
	}

	.combo-box__option-focused {
		background-color: var(--options-list-background-focus);
	}
</style>
