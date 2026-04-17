import TestCommandClass from '$lib/utils/reactiveClasses/test/testCommandClass';

describe('TestCommandClass', () => {
	it('will set the value in the constructor', () => {
		const cmd = new TestCommandClass('test');

		expect(cmd.value).toEqual('test');
	});

	it('can clear the value', () => {
		const cmd = new TestCommandClass('test');

		cmd.clear();

		expect(cmd.value).toEqual('');
	});

	it('can add to the value', () => {
		const cmd = new TestCommandClass('test');

		cmd.addLetter('t');

		expect(cmd.value).toEqual('testt');
	});

	it('can set the value', () => {
		const cmd = new TestCommandClass('test');

		cmd.setValueInvisibly('t');

		expect(cmd.value).toEqual('t');
	});
});
