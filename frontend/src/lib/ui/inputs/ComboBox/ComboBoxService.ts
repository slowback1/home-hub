import { KeyboardKeys } from '$lib/utils/KeyboardKeys';
import { command } from '$lib/utils/reactiveClasses/command';

export type ComboBoxOption<T> = {
	label: string;
	value: T;
};

export type ComboBoxOptionOutput<T> = ComboBoxOption<T> & { id: number };

export default class ComboBoxService<T> {
	value: string = '';
	isOpen: boolean = false;
	displayedOptions: ComboBoxOptionOutput<T>[];
	focusedOption?: ComboBoxOptionOutput<T>;

	constructor(
		private options: ComboBoxOption<T>[] = [],
		private onSelect: (value: T) => void = () => {}
	) {
		this.setDisplayedOptions();
	}

	private setDisplayedOptions() {
		this.displayedOptions = this.getOptions();
	}

	private getOptions(): ComboBoxOptionOutput<T>[] {
		if (!this.options) return [];

		const sortBThenA = -1;
		const sortAThenB = 1;
		const sortEvenly = 0;

		return this.options
			.map((opt, index) => ({ ...opt, id: index }))
			.filter((opt) => opt.label.toLowerCase().includes(this.value.toLowerCase()))
			.sort((a, b) => {
				const labelA = a.label;
				const labelB = b.label;

				const valueStartsWithA = labelA.startsWith(this.value);
				const valueStartsWithB = labelB.startsWith(this.value);

				if (valueStartsWithA && !valueStartsWithB) {
					return sortBThenA;
				}
				if (valueStartsWithB && !valueStartsWithA) {
					return sortAThenB;
				}
				return sortEvenly;
			});
	}

	@command
	onInputChange(event: { target: { value: string } }) {
		this.value = event.target.value;
		this.isOpen = true;
		this.setDisplayedOptions();
		this.focusFirstOption();
	}

	@command
	toggleIsOpen() {
		this.isOpen = !this.isOpen;

		if (this.isOpen) this.focusFirstOption();
		else this.clearFocus();
	}

	@command
	setFocus(option: ComboBoxOptionOutput<T>) {
		this.focusedOption = option;
	}

	@command
	handleSelect(option: ComboBoxOption<T>) {
		this.onSelect(option?.value);
		this.value = option?.label;
		this.isOpen = false;
		this.focusedOption = undefined;
	}

	@command
	handleKeyboardEvent(
		event: KeyboardEvent,
		focusedOption: ComboBoxOptionOutput<T> = this.focusedOption
	) {
		switch (event.key) {
			case KeyboardKeys.ArrowUp:
				this.moveFocusUp();
				break;
			case KeyboardKeys.ArrowDown:
				this.moveFocusDown();
				break;
			case KeyboardKeys.Enter:
				this.selectCurrentlyFocusedItem(focusedOption);
				break;
			case KeyboardKeys.Escape:
				if (this.isOpen) this.toggleIsOpen();
				break;
			default:
				break;
		}
	}

	private focusFirstOption() {
		this.focusedOption = this.displayedOptions[0];
	}

	private clearFocus() {
		this.focusedOption = undefined;
	}

	private selectCurrentlyFocusedItem(focusedOption: ComboBoxOptionOutput<T>) {
		this.handleSelect(focusedOption);
	}

	private moveFocusUp() {
		const currentIndex = this.focusedOption?.id ?? 0;

		if (currentIndex === 0) return;

		this.focusedOption = this.getOptions()[currentIndex - 1];
	}

	private moveFocusDown() {
		const options = this.getOptions();

		const currentIndex = this.focusedOption?.id ?? options.length - 1;

		if (currentIndex === options.length - 1) return;

		this.focusedOption = this.getOptions()[currentIndex + 1];
	}
}
