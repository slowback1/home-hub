<script lang="ts">
	import { onMount } from 'svelte';
	import {
		House,
		CheckSquare,
		Shuffle,
		Gamepad2,
		Cloud,
		ChevronLeft,
		ChevronRight
	} from 'lucide-svelte';
	import UrlPathProvider from '$lib/providers/urlPathProvider';

	const STORAGE_KEY = 'sidebar_collapsed';
	const LOGO_ICON_SIZE = 24;
	const NAV_ICON_SIZE = 20;

	const navItems = [
		{ testId: 'nav-item-home', href: '/', label: 'Home', icon: House },
		{ testId: 'nav-item-tasks', href: '/tasks', label: 'Task Tracker', icon: CheckSquare },
		{ testId: 'nav-item-activity', href: '/activity', label: 'Activity Picker', icon: Shuffle },
		{ testId: 'nav-item-retro', href: '/retro', label: 'RetroAchievements', icon: Gamepad2 },
		{ testId: 'nav-item-weather', href: '/weather', label: 'Weather', icon: Cloud }
	];

	let collapsed = $state(false);

	onMount(() => {
		collapsed = localStorage.getItem(STORAGE_KEY) === 'true';
	});

	function toggleCollapse() {
		collapsed = !collapsed;
		localStorage.setItem(STORAGE_KEY, String(collapsed));
	}

	function isActive(href: string): boolean {
		if (href === '/') {
			return UrlPathProvider.matchesPath('/');
		}
		return UrlPathProvider.matchesPath(href);
	}
</script>

<nav class="sidebar" class:collapsed>
	<a href="/" class="sidebar-logo" data-testid="sidebar-logo">
		{#if collapsed}
			<House size={LOGO_ICON_SIZE} />
		{:else}
			<span>HomeHub</span>
		{/if}
	</a>

	<ul class="sidebar-nav">
		{#each navItems as item (item.href)}
			{@const Icon = item.icon}
			<li class="nav-item" class:active={isActive(item.href)} data-testid={item.testId}>
				<a href={item.href} class="nav-link" title={item.label}>
					<Icon size={NAV_ICON_SIZE} />
					{#if !collapsed}
						<span class="nav-label">{item.label}</span>
					{/if}
				</a>
			</li>
		{/each}
	</ul>

	<button class="collapse-toggle" data-testid="sidebar-toggle" onclick={toggleCollapse}>
		{#if collapsed}
			<ChevronRight size={NAV_ICON_SIZE} />
		{:else}
			<ChevronLeft size={NAV_ICON_SIZE} />
		{/if}
	</button>
</nav>

<style>
	.sidebar {
		display: flex;
		flex-direction: column;
		width: 220px;
		min-height: 100vh;
		background-color: var(--color-surface-base);
		border-right: 1px solid var(--color-border-subtle);
		transition: width 0.2s ease;
		flex-shrink: 0;
	}

	.sidebar.collapsed {
		width: 60px;
	}

	.sidebar-logo {
		display: flex;
		align-items: center;
		justify-content: center;
		padding: var(--space-4) var(--space-3);
		color: var(--color-text-primary);
		font-size: var(--font-size-lg);
		font-weight: var(--font-weight-bold);
		text-decoration: none;
		border-bottom: 1px solid var(--color-border-subtle);
		min-height: 64px;
	}

	.sidebar-logo:hover {
		color: var(--color-brand-lighter);
	}

	.sidebar-nav {
		list-style: none;
		margin: 0;
		padding: var(--space-2) 0;
		flex: 1;
	}

	.nav-item {
		display: flex;
	}

	.nav-link {
		display: flex;
		align-items: center;
		gap: var(--space-3);
		width: 100%;
		padding: var(--space-3) var(--space-4);
		color: var(--color-text-secondary);
		text-decoration: none;
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		border-radius: var(--radius-sm);
		margin: 0 var(--space-2);
		width: calc(100% - var(--space-4));
		transition:
			background-color 0.15s ease,
			color 0.15s ease;
	}

	.nav-link:hover {
		background-color: var(--color-surface-raised);
		color: var(--color-text-primary);
	}

	.nav-item.active .nav-link {
		background-color: var(--color-surface-overlay);
		color: var(--color-brand-lighter);
	}

	.nav-label {
		white-space: nowrap;
		overflow: hidden;
	}

	.collapse-toggle {
		display: flex;
		align-items: center;
		justify-content: center;
		width: 100%;
		padding: var(--space-3);
		background: none;
		border: none;
		border-top: 1px solid var(--color-border-subtle);
		color: var(--color-text-secondary);
		cursor: pointer;
		transition: color 0.15s ease;
	}

	.collapse-toggle:hover {
		color: var(--color-text-primary);
	}
</style>
