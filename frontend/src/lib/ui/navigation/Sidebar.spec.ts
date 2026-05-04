import { act, fireEvent, render, waitFor } from '@testing-library/svelte';
import { beforeEach, afterEach } from 'vitest';
import Sidebar from './Sidebar.svelte';
import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
import { FeatureFlags } from '$lib/services/FeatureFlag/FeatureFlags';

const ALL_NAV_FLAGS_ENABLED = [
	{ name: FeatureFlags.CHORE_TASK_TRACKER_ENABLED, isEnabled: true },
	{ name: FeatureFlags.ACTIVITY_PICKER_ENABLED, isEnabled: true },
	{ name: FeatureFlags.RETRO_ACHIEVEMENTS_ENABLED, isEnabled: true },
	{ name: FeatureFlags.WEATHER_ENABLED, isEnabled: true }
];

describe('Sidebar', () => {
	beforeEach(() => {
		localStorage.clear();
		FeatureFlagService.featureFlags = ALL_NAV_FLAGS_ENABLED;
	});

	afterEach(() => {
		localStorage.clear();
		FeatureFlagService.featureFlags = [];
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

	it('renders nav labels in the DOM', () => {
		const { getByText } = render(Sidebar);
		expect(getByText('Home')).toBeInTheDocument();
		expect(getByText('Chore / Task Tracker')).toBeInTheDocument();
	});

	it('renders the collapse toggle button', () => {
		const { getByTestId } = render(Sidebar);
		expect(getByTestId('sidebar-toggle')).toBeInTheDocument();
	});

	it('clicking the toggle adds collapsed class', async () => {
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

	it('clicking the toggle twice removes collapsed class', async () => {
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

	it('renders the Admin nav item linking to /admin/system-config', () => {
		const { getByTestId } = render(Sidebar);
		const item = getByTestId('nav-item-admin');
		expect(item).toBeInTheDocument();
		expect(item.querySelector('a')).toHaveAttribute('href', '/admin/system-config');
	});

	it('marks Admin nav item active when current path starts with /admin/system-config', () => {
		const { getByTestId } = render(Sidebar, { props: { currentPath: '/admin/system-config' } });
		const item = getByTestId('nav-item-admin');
		expect(item).toHaveClass('active');
	});

	it('marks Admin nav item active when current path is /admin/feature-flags', () => {
		const { getByTestId } = render(Sidebar, { props: { currentPath: '/admin/feature-flags' } });
		const item = getByTestId('nav-item-admin');
		expect(item).toHaveClass('active');
	});
});
