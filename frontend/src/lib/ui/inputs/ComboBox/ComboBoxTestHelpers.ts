import { fireEvent, screen, waitFor } from '@testing-library/svelte';

export default class ComboBoxTestHelpers {
	static assertThatComboBoxHasLabel(testId: string, expectedLabel: string) {
		const label = screen.getByTestId(`${testId}__label`);

		expect(label).toBeInTheDocument();
		expect(label).toHaveTextContent(expectedLabel);
	}

	static assertThatComboBoxHasValue(testId: string, expectedValue: string) {
		const input = screen.getByTestId(`${testId}__input`);

		expect(input).toBeInTheDocument();
		expect(input).toHaveValue(expectedValue);
	}

	static async selectComboBoxOption(testId: string, labelToSelect: string) {
		const input = screen.getByTestId(`${testId}__input`);

		fireEvent.input(input, { target: { value: labelToSelect } });

		await waitFor(() => {
			const options = screen.getByTestId(`${testId}__options`);

			expect(options).toBeInTheDocument();
		});

		const firstOption = screen.getAllByTestId(`${testId}__options`)[0];

		fireEvent.click(firstOption);
	}
}
