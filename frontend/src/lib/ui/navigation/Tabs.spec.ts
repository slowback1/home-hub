import { act, fireEvent, render, waitFor } from '@testing-library/svelte';
import Tabs from './Tabs.svelte';

const defaultProps = {
	tabs: [
		{ id: 'tab1', label: 'First' },
		{ id: 'tab2', label: 'Second' },
		{ id: 'tab3', label: 'Third' }
	]
};

describe('Tabs', () => {
	it('renders a tablist', () => {
		const { getByRole } = render(Tabs, { props: defaultProps });
		expect(getByRole('tablist')).toBeInTheDocument();
	});

	it('renders a tab button for each tab', () => {
		const { getAllByRole } = render(Tabs, { props: defaultProps });
		expect(getAllByRole('tab')).toHaveLength(3);
	});

	it('renders tab labels', () => {
		const { getByText } = render(Tabs, { props: defaultProps });
		expect(getByText('First')).toBeInTheDocument();
		expect(getByText('Second')).toBeInTheDocument();
	});

	it('marks the first tab as active by default', () => {
		const { getAllByRole } = render(Tabs, { props: defaultProps });
		const tabs = getAllByRole('tab');
		expect(tabs[0]).toHaveAttribute('aria-selected', 'true');
	});

	it('marks non-first tabs as inactive by default', () => {
		const { getAllByRole } = render(Tabs, { props: defaultProps });
		const tabs = getAllByRole('tab');
		expect(tabs[1]).toHaveAttribute('aria-selected', 'false');
	});

	it('clicking a tab makes it active', async () => {
		const { getAllByRole } = render(Tabs, { props: defaultProps });
		const tabs = getAllByRole('tab');
		act(() => {
			fireEvent.click(tabs[1]);
		});
		await waitFor(() => {
			expect(tabs[1]).toHaveAttribute('aria-selected', 'true');
		});
	});

	it('clicking a tab deactivates the previously active tab', async () => {
		const { getAllByRole } = render(Tabs, { props: defaultProps });
		const tabs = getAllByRole('tab');
		act(() => {
			fireEvent.click(tabs[1]);
		});
		await waitFor(() => {
			expect(tabs[0]).toHaveAttribute('aria-selected', 'false');
		});
	});

	it('respects an initial activeTab prop', () => {
		const { getAllByRole } = render(Tabs, {
			props: { ...defaultProps, activeTab: 'tab2' }
		});
		const tabs = getAllByRole('tab');
		expect(tabs[1]).toHaveAttribute('aria-selected', 'true');
		expect(tabs[0]).toHaveAttribute('aria-selected', 'false');
	});
});
