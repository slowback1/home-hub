import { fireEvent, screen } from '@testing-library/svelte';

export default class CheckboxTestHelpers {
	static assertHasLabel(testId: string, expectedLabel: string) {
		const label = screen.getByTestId(`${testId}-label`);

		expect(label).toHaveTextContent(expectedLabel);
	}

	static assertCheckboxIsChecked(testId: string) {
		const input = screen.getByTestId(`${testId}`);

		expect(input).toBeChecked();
	}

	static assertCheckboxISNotChecked(testId: string) {
		const input = screen.getByTestId(testId);

		expect(input).not.toBeChecked();
	}

	static checkCheckbox(testId: string) {
		const input = screen.getByTestId(testId);

		fireEvent.click(input);
	}
}
