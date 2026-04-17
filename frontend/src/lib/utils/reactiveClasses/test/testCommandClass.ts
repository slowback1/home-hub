import { command } from '$lib/utils/reactiveClasses/command';

export default class TestCommandClass {
	value: string;

	constructor(defaultValue: string) {
		this.value = defaultValue;
	}

	@command
	clear() {
		this.value = '';
	}

	@command
	addLetter(letter: string) {
		this.value += letter;
	}
	setValueInvisibly(value: string) {
		this.value = value;
	}
}
