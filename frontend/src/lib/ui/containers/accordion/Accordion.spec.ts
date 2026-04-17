import Accordion from './Accordion.Test.svelte';
import { act, fireEvent, render, type RenderResult, waitFor } from '@testing-library/svelte';
import { beforeEach } from 'vitest';
import MessageBus from '$lib/bus/MessageBus';
import getLocalStorageMock from '$lib/testHelpers/localStorageMock';

describe('Accordion', () => {
	let result: RenderResult<Accordion>;

	function renderComponent(overrides: object = {}) {
		if (result) result.unmount();

		const props = {
			...overrides
		};

		result = render(Accordion, { props });
	}

	beforeEach(() => {
		MessageBus.initialize(getLocalStorageMock());

		renderComponent();
	});

	it.each([
		[0, 'label 1'],
		[1, 'label 2'],
		[2, 'label 3']
	])('renders the label of each item for index %s', (index, label) => {
		const items = result.getAllByTestId('accordion-item');

		const item = items[index];

		expect(item).toBeTruthy();

		const labelElement = item.querySelector("[data-testid='accordion-item__label']");

		expect(labelElement).toBeInTheDocument();
		expect(labelElement).toHaveTextContent(label);
	});

	it('the accordion content is not visible when the accordion is closed', () => {
		const firstItem = result.getAllByTestId('accordion-item')[0];

		expect(firstItem).toBeInTheDocument();

		const content = firstItem.querySelector("[data-testid='accordion-item__content']");

		expect(content).not.toHaveClass('accordion-item__content-open');
	});

	function assertThatContentIsClosed(index = 0) {
		const firstItem = result.getAllByTestId('accordion-item')[index];

		expect(firstItem).toBeInTheDocument();

		const content = firstItem.querySelector("[data-testid='accordion-item__content']");
		const label = firstItem.querySelector("[data-testid='accordion-item__label']");

		expect(content).not.toHaveClass('accordion-item__content-open');
		expect(label).toHaveAttribute('aria-expanded', 'false');
	}

	function clickLabel(index = 0) {
		const item = result.getAllByTestId('accordion-item')[index];

		expect(item).toBeTruthy();

		const labelElement = item.querySelector("[data-testid='accordion-item__label']");

		act(() => {
			fireEvent.click(labelElement);
		});
	}

	async function clickLabelAndWaitForContentToOpen(index = 0) {
		clickLabel(index);

		await waitFor(() => {
			const item = result.getAllByTestId('accordion-item')[index];
			const content = item.querySelector("[data-testid='accordion-item__content']");
			const label = item.querySelector("[data-testid='accordion-item__label']");

			expect(content).toHaveClass('accordion-item__content-open');
			expect(label).toHaveAttribute('aria-expanded', 'true');
		});
	}

	it('clicking the label will open the content', async () => {
		await clickLabelAndWaitForContentToOpen();

		await waitFor(() => {
			const item = result.getAllByTestId('accordion-item')[0];
			const content = item.querySelector("[data-testid='accordion-item__content']");

			expect(content).toHaveClass('accordion-item__content-open');
		});
	});

	it('clicking the label twice will close the content', async () => {
		await clickLabelAndWaitForContentToOpen(0);

		clickLabel(0);

		await waitFor(() => {
			assertThatContentIsClosed(0);
		});
	});

	it('clicking the first label first, then the second label second closes the content of the first label', async () => {
		await clickLabelAndWaitForContentToOpen(0);

		clickLabel(1);

		await waitFor(() => {
			assertThatContentIsClosed(0);
		});
	});

	it('the content has an aria-controls attribute and the toggle has an id attribute that match each other', () => {
		const item = result.getAllByTestId('accordion-item')[0];

		const label = item.querySelector("[data-testid='accordion-item__label']");

		expect(label).toHaveAttribute('aria-controls', 'label-1');

		const content = item.querySelector("[data-testid='accordion-item__content']");

		expect(content).toHaveAttribute('id', 'label-1');
	});

	it("the content has an aria-role of 'region'", () => {
		const item = result.getAllByTestId('accordion-item')[0];
		const content = item.querySelector("[data-testid='accordion-item__content']");

		expect(content).toHaveAttribute('role', 'region');
	});

	it('the content has the correct labelled-by id', () => {
		const item = result.getAllByTestId('accordion-item')[0];

		const label = item.querySelector("[data-testid='accordion-item__label']");
		const content = item.querySelector("[data-testid='accordion-item__content']");

		expect(label).toHaveAttribute('id', 'label-1-control');
		expect(content).toHaveAttribute('aria-labelledby', 'label-1-control');
	});
});
