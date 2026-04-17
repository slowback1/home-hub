import doNothing from '$lib/utils/doNothing';

export type PaginationServiceSettings = {
	rowsPerPageOptions: number[];
	onPageChange: (parameters: PaginationRequestParameters) => void;
};

export type PaginationRequestParameters = {
	page: number;
	rowsPerPage: number;
};

export type PaginationDataRetrieved = {
	totalCount: number;
};

export default class PaginationService {
	private static readonly MAX_VISIBLE_PAGES = 5;
	private static readonly PAGE_OFFSET_ONE = 1;
	// eslint-disable-next-line no-magic-numbers -- it should be clear that these are the default options for rows per page
	private static readonly DEFAULT_ROWS_PER_PAGE_OPTIONS = [10, 25, 50, 100];

	public settings: PaginationServiceSettings;
	private parameters: PaginationRequestParameters = $state();
	public totalCount: number = $state(0);

	constructor(settings: Partial<PaginationServiceSettings> = {}) {
		this.settings = this.normalizeSettings(settings);
		this.parameters = this.buildInitialParameters();
	}

	public getPageParameters(): PaginationRequestParameters {
		return this.parameters;
	}

	public onDataRetrieved(data: PaginationDataRetrieved) {
		this.totalCount = data.totalCount;
	}

	public getVisiblePageNumbers(): number[] {
		const { page, rowsPerPage } = this.parameters;
		const totalPages = Math.ceil(this.totalCount / rowsPerPage);
		const maxPages = PaginationService.MAX_VISIBLE_PAGES;

		// eslint-disable-next-line no-magic-numbers -- it should be clear that we are calculating the half of the max visible pages
		const half = Math.floor(maxPages / 2);

		let start = Math.max(page - half, PaginationService.PAGE_OFFSET_ONE);
		let end = start + maxPages - 1;

		if (end > totalPages) {
			end = totalPages;
			start = Math.max(end - maxPages + 1, PaginationService.PAGE_OFFSET_ONE);
		}

		const visiblePages: number[] = [];
		for (let i = start; i <= end; i++) {
			visiblePages.push(i);
		}
		return visiblePages;
	}

	public goToPage(page: number) {
		this.parameters.page = page;
		this.settings.onPageChange(this.parameters);
	}

	public goToFirstPage() {
		this.goToPage(1);
	}

	public goToLastPage() {
		const lastPage = this.getLastPage();

		this.goToPage(lastPage);
	}

	public shouldDisableNextButton() {
		return this.parameters.page === this.getLastPage();
	}

	public shouldDisablePreviousButton() {
		return this.parameters.page === 1;
	}

	private getLastPage() {
		return Math.ceil(this.totalCount / this.parameters.rowsPerPage);
	}

	public goToNextPage() {
		const isLastPage = this.parameters.page === this.getLastPage();

		if (!isLastPage) {
			this.goToPage(this.parameters.page + 1);
		}
	}

	public goToPreviousPage() {
		const isFirstPage = this.parameters.page === 1;

		if (!isFirstPage) {
			this.goToPage(this.parameters.page - 1);
		}
	}

	public updateRowsPerPage(rowsPerPage: number) {
		this.parameters.rowsPerPage = rowsPerPage;
		this.goToPage(1);
	}

	private buildInitialParameters(): PaginationRequestParameters {
		return {
			page: 1,
			rowsPerPage:
				this.settings.rowsPerPageOptions[0] || PaginationService.DEFAULT_ROWS_PER_PAGE_OPTIONS[0]
		};
	}

	private normalizeSettings(
		settings: Partial<PaginationServiceSettings>
	): PaginationServiceSettings {
		return {
			rowsPerPageOptions:
				settings.rowsPerPageOptions || PaginationService.DEFAULT_ROWS_PER_PAGE_OPTIONS,
			onPageChange: settings.onPageChange || doNothing
		};
	}
}
