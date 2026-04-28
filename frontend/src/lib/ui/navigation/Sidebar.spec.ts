import { act, fireEvent, render, waitFor } from '@testing-library/svelte';
import { beforeEach, afterEach } from 'vitest';
import Sidebar from './Sidebar.svelte';
import UrlPathProvider, { TestUrlProvider } from '$lib/providers/urlPathProvider';

describe('Sidebar', () => {
	beforeEach(() => {
		UrlPathProvider.initialize(new TestUrlProvider('/'));
		localStorage.clear();
	});

	afterEach(() => {
		localStorage.clear();
	});

	it('renders a nav element', () => {
		const { getByRole } = render(Sidebar);
		expect(getByRole('navigation')).toBeInTheDocument();
	});

	it('renders the app logo linking to /', () => {
		const { getByTestId } = render(Sidebar);
		const logo = getByTestId('sidebar-logo');
		expect(logo).toBeInTheDocument();
		expect(logo).toHaveAttribute('href', '/');
	});

	it('renders all nav items', () => {
		const { getAllByRole } = render(Sidebar);
		const links = getAllByRole('link');
		// logo link + 5 nav items
		expect(links.length).toBeGreaterThanOrEqual(6);
	});

	it('shows nav labels when expanded', () => {
		const { getByText } = render(Sidebar);
		expect(getByText('Home')).toBeInTheDocument();
		expect(getByText('Task Tracker')).toBeInTheDocument();
	});

	it('hides nav labels when collapsed', async () => {
		const { queryByText, getByTestId } = render(Sidebar);
		act(() => {
			fireEvent.click(getByTestId('sidebar-toggle'));
		});
		await waitFor(() => {
			expect(queryByText('Home')).not.toBeInTheDocument();
			expect(queryByText('Task Tracker')).not.toBeInTheDocument();
		});
	});

	it('renders the collapse toggle button', () => {
		const { getByTestId } = render(Sidebar);
		expect(getByTestId('sidebar-toggle')).toBeInTheDocument();
	});

	it('clicking the toggle collapses the sidebar', async () => {
		const { getByTestId, getByRole } = render(Sidebar);
		const nav = getByRole('navigation');
		expect(nav).not.toHaveClass('collapsed');
		act(() => {
			fireEvent.click(getByTestId('sidebar-toggle'));
		});
		await waitFor(() => {
			expect(nav).toHaveClass('collapsed');
		});
	});

	it('clicking the toggle twice expands the sidebar again', async () => {
		const { getByTestId, getByRole } = render(Sidebar);
		const toggle = getByTestId('sidebar-toggle');
		act(() => {
			fireEvent.click(toggle);
		});
		act(() => {
			fireEvent.click(toggle);
		});
		await waitFor(() => {
			expect(getByRole('navigation')).not.toHaveClass('collapsed');
		});
	});

	it('persists collapsed state to localStorage on toggle', async () => {
		const { getByTestId } = render(Sidebar);
		act(() => {
			fireEvent.click(getByTestId('sidebar-toggle'));
		});
		await waitFor(() => {
			expect(localStorage.getItem('sidebar_collapsed')).toBe('true');
		});
	});

	it('reads collapsed state from localStorage on mount', async () => {
		localStorage.setItem('sidebar_collapsed', 'true');
		const { getByRole } = render(Sidebar);
		await waitFor(() => {
			expect(getByRole('navigation')).toHaveClass('collapsed');
		});
	});

	it('marks the nav item as active when the path matches', () => {
		UrlPathProvider.initialize(new TestUrlProvider('/tasks'));
		const { getByTestId } = render(Sidebar);
		expect(getByTestId('nav-item-tasks')).toHaveClass('active');
	});

	it('does not mark non-matching nav items as active', () => {
		UrlPathProvider.initialize(new TestUrlProvider('/tasks'));
		const { getByTestId } = render(Sidebar);
		expect(getByTestId('nav-item-home')).not.toHaveClass('active');
	});
});
