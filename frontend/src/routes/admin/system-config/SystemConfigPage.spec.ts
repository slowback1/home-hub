import { render, waitFor } from '@testing-library/svelte';
import { vi } from 'vitest';
import SystemConfigPage from './+page.svelte';

vi.mock('$lib/api/SystemConfigApi', () => ({
	default: vi.fn()
}));

import SystemConfigApi from '$lib/api/SystemConfigApi';

const weatherEntries = [
	{
		id: 'weather::zip_code',
		namespace: 'weather',
		key: 'zip_code',
		value: '10001',
		type: '',
		isSecret: false
	},
	{
		id: 'weather::api_key',
		namespace: 'weather',
		key: 'api_key',
		value: '***',
		type: '',
		isSecret: true
	}
];

const multiNsEntries = [
	...weatherEntries,
	{ id: 'app::debug', namespace: 'app', key: 'debug', value: 'false', type: '', isSecret: false }
];

describe('Admin System Config page', () => {
	beforeEach(() => {
		vi.mocked(SystemConfigApi).mockClear();
	});

	it('shows a spinner while loading', () => {
		vi.mocked(SystemConfigApi).mockImplementation(
			() =>
				({
					getAll: () => new Promise(() => {})
				}) as never
		);

		const { getByRole } = render(SystemConfigPage);
		expect(getByRole('status')).toBeInTheDocument();
	});

	it('hides the spinner and shows entries after loading', async () => {
		vi.mocked(SystemConfigApi).mockImplementation(
			() =>
				({
					getAll: vi.fn().mockResolvedValue(weatherEntries)
				}) as never
		);

		const { queryByRole, getByText } = render(SystemConfigPage);
		await waitFor(() => expect(queryByRole('status')).not.toBeInTheDocument());
		expect(getByText('zip_code')).toBeInTheDocument();
		expect(getByText('10001')).toBeInTheDocument();
	});

	it('shows an error message when fetch fails', async () => {
		vi.mocked(SystemConfigApi).mockImplementation(
			() =>
				({
					getAll: vi.fn().mockRejectedValue(new Error('network error'))
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
					getAll: vi.fn().mockResolvedValue(multiNsEntries)
				}) as never
		);

		const { getByTestId } = render(SystemConfigPage);
		await waitFor(() => expect(getByTestId('namespace-weather')).toBeInTheDocument());
		expect(getByTestId('namespace-app')).toBeInTheDocument();
	});
});
