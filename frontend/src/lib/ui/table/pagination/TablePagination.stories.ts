import type { Meta } from '@storybook/svelte';
import TablePagination from '$lib/ui/table/pagination/TablePagination.svelte';
import PaginationService from '$lib/ui/table/pagination/paginationService.svelte';

const meta: Meta = {
	title: 'UI/Table/Table Pagination',
	component: TablePagination as never
};

export default meta;

const makeProps = ({
	totalCount = 100,
	rowsPerPageOptions = [10, 25, 50, 100],
	currentPage = 1
}) => {
	const service = new PaginationService({ rowsPerPageOptions: rowsPerPageOptions });

	service.onDataRetrieved({ totalCount });
	service.goToPage(currentPage);

	return {
		service
	};
};

export const Default = {
	args: makeProps({})
};

export const WithLargeTotalCount = {
	args: makeProps({ totalCount: 1000 })
};

export const WithDifferentRowsPerPageOptions = {
	args: makeProps({ rowsPerPageOptions: [5, 10, 15, 20] })
};

export const WithPageInTheMiddle = {
	args: makeProps({ currentPage: 3 })
};

export const WithPageAtTheEnd = {
	args: makeProps({ currentPage: 10 })
};

export const WithOnePage = {
	args: makeProps({ totalCount: 10 })
};

export const WithNoData = {
	args: makeProps({ totalCount: 0 })
};
