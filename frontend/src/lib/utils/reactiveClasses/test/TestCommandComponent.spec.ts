import { fireEvent, render, type RenderResult, waitFor } from '@testing-library/svelte';
import TestCommandComponent from '$lib/utils/reactiveClasses/test/TestCommandComponent.svelte';
import { beforeEach } from 'vitest';
import TestMultipleCommandComponents from '$lib/utils/reactiveClasses/test/TestMultipleCommandComponents.svelte';

describe('TestCommandComponent', () => {
	let result: RenderResult<TestCommandComponent>;

	beforeEach(() => {
		if (result) result.unmount();

		result = render(TestCommandComponent);
	});

	it('has a place to display the value', () => {
		const value = result.getByTestId('test-command__value');

		expect(value).toBeInTheDocument();
		expect(value).toHaveTextContent('test');
	});

	it('can clear the value by clicking the clear button', async () => {
		const clear = result.getByTestId('test-command__clear');

		expect(clear).toBeInTheDocument();

		fireEvent.click(clear);

		await waitFor(() => {
			const value = result.getByTestId('test-command__value');

			expect(value.textContent).toEqual('');
		});
	});

	it("can add a letter to the value by clicking the 'add a' button", async () => {
		const add = result.getByTestId('test-command__add');

		expect(add).toBeInTheDocument();

		fireEvent.click(add);

		await waitFor(() => {
			const value = result.getByTestId('test-command__value');

			expect(value.textContent).toEqual('testa');
		});
	});

	it('does not react when calling a function that does not have a command decorator', async () => {
		const set = result.getByTestId('test-command__set');

		expect(set).toBeInTheDocument();

		fireEvent.click(set);

		await new Promise((res) => setTimeout(res, 50));

		await waitFor(() => {
			const value = result.getByTestId('test-command__value');

			expect(value.textContent).toEqual('test');
		});
	});

	describe('when there are multiple of the same component', () => {
		let result: RenderResult<TestMultipleCommandComponents>;

		beforeEach(() => {
			if (result) result.unmount();

			result = render(TestMultipleCommandComponents);
		});

		function assertValueShouldBe(id: string, shouldBe: string) {
			const value = result
				.getByTestId(`${id}`)
				.querySelector("[data-testid='test-command__value']");

			expect(value.textContent).toEqual(shouldBe);
		}

		function clickClearButton(id: string) {
			const clear = result
				.getByTestId(`${id}`)
				.querySelector("[data-testid='test-command__clear']");

			fireEvent.click(clear);
		}

		it.each([
			['1', '2'],
			['1', '3'],
			['2', '1'],
			['2', '3'],
			['3', '1'],
			['3', '2']
		])(
			'clicking the clear button on the %s component should not affect the %s component',
			async (componentToClear, componentToAssert) => {
				clickClearButton(componentToClear);

				await waitFor(() => {
					assertValueShouldBe(componentToAssert, 'test');
				});
			}
		);

		it.each([['1'], ['2'], ['3']])(
			'clicking the clear button on the %s component should clear the value in that  component',
			async (component) => {
				clickClearButton(component);

				await waitFor(() => {
					assertValueShouldBe(component, '');
				});
			}
		);
	});
});
