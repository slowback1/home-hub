import { render, type RenderResult } from '@testing-library/svelte';
import TestTable from '$lib/ui/table/table/testing/TestTable.svelte';
import { beforeEach } from 'vitest';
import {
	getTestTableData,
	getTestTableDefinitions
} from '$lib/ui/table/table/testing/testTableDefinitions';

describe('Table', () => {
	let result: RenderResult<TestTable>;
	const columns = getTestTableDefinitions();
	const rows = getTestTableData();
	const renderedByKeyColumns = columns.filter((col) => !!col.renderKey);

	beforeEach(() => {
		if (result) result.unmount();

		result = render(TestTable);
	});

	describe('basic rendering', () => {
		it('should render a table', () => {
			const table = result.getByRole('table');

			expect(table).toBeInTheDocument();
		});

		it.each(columns)('should render a column with title $title', (column) => {
			const header = result.getByText(column.title);

			expect(header).toBeInTheDocument();
		});

		it.each(columns)('the column header is in the thead as a proper th element', (column) => {
			const header = result.getByText(column.title);
			const headerCell = header.closest('th');

			expect(headerCell).toBeInTheDocument();
		});

		it.each(columns)(
			'the column header with the title $title can be accessed by a test id',
			(column) => {
				const header = result.getByTestId(`table__header-${column.key}`);

				expect(header).toBeInTheDocument();
			}
		);

		it('should render the correct number of rows', () => {
			const rows = result.getAllByTestId('table__row');

			const lengthOfData = getTestTableData().length;

			expect(rows).toHaveLength(lengthOfData);
		});

		it.each(renderedByKeyColumns)('should render a table cell for the key $key', (column) => {
			const firstRowColumn = result.getByTestId(`table__cell-${column.key}-0`);

			expect(firstRowColumn).toBeInTheDocument();
		});

		it.each(renderedByKeyColumns)('should render the correct value for the key $key', (column) => {
			const firstRowColumn = result.getByTestId(`table__cell-${column.key}-0`);

			expect(firstRowColumn).toHaveTextContent(rows[0][column.renderKey].toString());
		});

		it('can render a custom cell', () => {
			const customCell = result.getByTestId('table__cell-custom-0');
			expect(customCell).toBeInTheDocument();
			expect(customCell.textContent).toContain('solahtaR');
		});
	});
});
