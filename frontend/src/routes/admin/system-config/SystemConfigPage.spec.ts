import { render, waitFor, fireEvent } from '@testing-library/svelte';
import { vi } from 'vitest';
import SystemConfigPage from './+page.svelte';

vi.mock('$lib/api/SystemConfigApi', () => ({
	default: vi.fn()
}));

vi.mock('$lib/ui/containers/toast/ToastService', () => ({
	default: vi.fn(),
	ToastVariant: { info: 0, warning: 1, error: 2, success: 3 }
}));

import SystemConfigApi from '$lib/api/SystemConfigApi';
import ToastService from '$lib/ui/containers/toast/ToastService';

const zipEntry = {
	id: 'weather::zip_code',
	namespace: 'weather',
	key: 'zip_code',
	value: '10001',
	type: 'text',
	isSecret: false,
	options: []
};

const apiKeyEntry = {
	id: 'weather::api_key',
	namespace: 'weather',
	key: 'api_key',
	value: '***',
	type: 'secret',
	isSecret: true,
	options: []
};

const providerEntry = {
	id: 'weather::provider',
	namespace: 'weather',
	key: 'provider',
	value: 'mock',
	type: 'select',
	isSecret: false,
	options: [
		{ value: 'mock', label: 'Mock' },
		{ value: 'openweathermap', label: 'Open Weather Map' }
	]
};

const weatherEntries = [zipEntry, apiKeyEntry, providerEntry];

const multiNsEntries = [
	...weatherEntries,
	{
		id: 'app::debug',
		namespace: 'app',
		key: 'debug',
		value: 'false',
		type: '',
		isSecret: false,
		options: []
	}
];

function makeApiMock({
	getAllResult = Promise.resolve(weatherEntries),
	updateResult = Promise.resolve({ ...zipEntry, value: '90210' })
}: {
	getAllResult?: Promise<unknown>;
	updateResult?: Promise<unknown>;
} = {}) {
	const mockUpdate = vi.fn().mockReturnValue(updateResult);
	const mockGetAll = vi.fn().mockReturnValue(getAllResult);
	vi.mocked(SystemConfigApi).mockImplementation(
		() => ({ getAll: mockGetAll, update: mockUpdate }) as never
	);
	return { mockGetAll, mockUpdate };
}

describe('Admin System Config page', () => {
	let mockAddToast: ReturnType<typeof vi.fn>;

	beforeEach(() => {
		vi.mocked(SystemConfigApi).mockClear();
		vi.mocked(ToastService).mockClear();
		mockAddToast = vi.fn();
		vi.mocked(ToastService).mockImplementation(() => ({ AddToast: mockAddToast }) as never);
	});

	it('shows a spinner while loading', () => {
		vi.mocked(SystemConfigApi).mockImplementation(
			() =>
				({
					getAll: () => new Promise(() => {}),
					update: vi.fn()
				}) as never
		);

		const { getByRole } = render(SystemConfigPage);
		expect(getByRole('status')).toBeInTheDocument();
	});

	it('hides the spinner and shows entries after loading', async () => {
		makeApiMock();
		const { queryByRole, getByText } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());
		expect(getByText('Zip Code')).toBeInTheDocument();
		expect(getByText('10001')).toBeInTheDocument();
	});

	it('shows an error message when fetch fails', async () => {
		vi.mocked(SystemConfigApi).mockImplementation(
			() =>
				({
					getAll: vi.fn().mockRejectedValue(new Error('network error')),
					update: vi.fn()
				}) as never
		);

		const { getByTestId, queryByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());
		expect(getByTestId('load-error')).toBeInTheDocument();
	});

	it('groups entries by namespace with a section header per namespace', async () => {
		vi.mocked(SystemConfigApi).mockImplementation(
			() =>
				({
					getAll: vi.fn().mockResolvedValue(multiNsEntries),
					update: vi.fn()
				}) as never
		);

		const { getByTestId } = render(SystemConfigPage);
		await waitFor(() => expect(getByTestId('namespace-weather')).toBeInTheDocument());
		expect(getByTestId('namespace-app')).toBeInTheDocument();
	});

	it('clicking a value enters edit mode showing input and Save/Cancel buttons', async () => {
		makeApiMock();
		const { getByTestId, queryByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		fireEvent.click(getByTestId('value-zip_code'));

		await waitFor(() => expect(queryByRole('textbox')).toBeInTheDocument());
		expect(getByTestId('btn-save')).toBeInTheDocument();
		expect(getByTestId('btn-cancel')).toBeInTheDocument();
	});

	it('saving successfully updates the displayed value and shows success toast', async () => {
		const { mockUpdate } = makeApiMock();
		const { getByTestId, queryByRole, getByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		fireEvent.click(getByTestId('value-zip_code'));
		await waitFor(() => expect(getByRole('textbox')).toBeInTheDocument());

		const input = getByRole('textbox');
		fireEvent.input(input, { target: { value: '90210' } });
		fireEvent.click(getByTestId('btn-save'));

		await waitFor(() => expect(mockUpdate).toHaveBeenCalledWith('weather', 'zip_code', '90210'));
		await waitFor(() => expect(queryByRole('textbox')).not.toBeInTheDocument());
		expect(mockAddToast).toHaveBeenCalledWith(expect.objectContaining({ variant: 3 }));
	});

	it('saving with error shows error toast and keeps edit mode open', async () => {
		vi.mocked(SystemConfigApi).mockImplementation(
			() =>
				({
					getAll: vi.fn().mockResolvedValue(weatherEntries),
					update: vi.fn().mockRejectedValue(new Error('save failed'))
				}) as never
		);

		const { getByTestId, queryByRole, getByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		fireEvent.click(getByTestId('value-zip_code'));
		await waitFor(() => expect(getByRole('textbox')).toBeInTheDocument());
		fireEvent.click(getByTestId('btn-save'));

		await waitFor(() =>
			expect(mockAddToast).toHaveBeenCalledWith(expect.objectContaining({ variant: 2 }))
		);
		expect(getByRole('textbox')).toBeInTheDocument();
	});

	it('secret entries display masked value by default', async () => {
		makeApiMock();
		const { getByTestId, queryByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		expect(getByTestId('value-api_key').textContent).toBe('••••••••');
	});

	it('clicking the show toggle reveals the actual value', async () => {
		makeApiMock();
		const { getByTestId, queryByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		fireEvent.click(getByTestId('toggle-api_key'));

		await waitFor(() => expect(getByTestId('value-api_key').textContent).toBe('***'));
	});

	it('clicking the show toggle again re-masks the value', async () => {
		makeApiMock();
		const { getByTestId, queryByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		fireEvent.click(getByTestId('toggle-api_key'));
		await waitFor(() => expect(getByTestId('value-api_key').textContent).toBe('***'));

		fireEvent.click(getByTestId('toggle-api_key'));
		await waitFor(() => expect(getByTestId('value-api_key').textContent).toBe('••••••••'));
	});

	it('entering edit mode pre-fills input with actual value for secret entries', async () => {
		makeApiMock();
		const { getByTestId, queryByRole, getByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		fireEvent.click(getByTestId('value-api_key'));

		await waitFor(() => expect(getByRole('textbox')).toBeInTheDocument());
		expect((getByRole('textbox') as HTMLInputElement).value).toBe('***');
	});

	it('non-secret rows are not masked and have no toggle', async () => {
		makeApiMock();
		const { getByTestId, queryByTestId, queryByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		expect(getByTestId('value-zip_code').textContent).toBe('10001');
		expect(queryByTestId('toggle-zip_code')).not.toBeInTheDocument();
	});

	it('select-type entries render as a dropdown', async () => {
		makeApiMock();
		const { getByTestId, queryByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		expect(getByTestId('select-provider')).toBeInTheDocument();
		expect(getByTestId('select-provider').tagName).toBe('SELECT');
	});

	it('select dropdown contains the correct options', async () => {
		makeApiMock();
		const { getByTestId, queryByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		const select = getByTestId('select-provider') as HTMLSelectElement;
		const optionValues = Array.from(select.options).map((o) => o.value);
		expect(optionValues).toContain('mock');
		expect(optionValues).toContain('openweathermap');
	});

	it('changing a select field calls update immediately and shows success toast', async () => {
		const mockUpdate = vi.fn().mockResolvedValue({ ...providerEntry, value: 'openweathermap' });
		vi.mocked(SystemConfigApi).mockImplementation(
			() => ({ getAll: vi.fn().mockResolvedValue(weatherEntries), update: mockUpdate }) as never
		);
		const { getByTestId, queryByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		fireEvent.change(getByTestId('select-provider'), { target: { value: 'openweathermap' } });

		await waitFor(() =>
			expect(mockUpdate).toHaveBeenCalledWith('weather', 'provider', 'openweathermap')
		);
		await waitFor(() =>
			expect(mockAddToast).toHaveBeenCalledWith(expect.objectContaining({ variant: 3 }))
		);
	});

	it('clicking Cancel returns the row to display mode', async () => {
		makeApiMock();
		const { getByTestId, queryByRole, getByRole } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());

		fireEvent.click(getByTestId('value-zip_code'));
		await waitFor(() => expect(getByRole('textbox')).toBeInTheDocument());

		fireEvent.click(getByTestId('btn-cancel'));

		await waitFor(() => expect(queryByRole('textbox')).not.toBeInTheDocument());
		expect(getByTestId('value-zip_code')).toBeInTheDocument();
	});
});
