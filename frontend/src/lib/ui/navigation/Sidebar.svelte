<script lang="ts">
	import { onMount } from 'svelte';
	import {
		House,
		CheckSquare,
		Shuffle,
		Gamepad2,
		Cloud,
		Settings,
		BookAudio,
		Image,
		Bookmark,
		Footprints,
		ChevronLeft,
		ChevronRight
	} from 'lucide-svelte';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
	import { FeatureFlags } from '$lib/services/FeatureFlag/FeatureFlags';
	import MessageBus from '$lib/bus/MessageBus';
	import { Messages } from '$lib/bus/Messages';

	export let currentPath: string = '/';

	const STORAGE_KEY = 'sidebar_collapsed';
	const LOGO_ICON_SIZE = 24;
	const NAV_ICON_SIZE = 20;

	const navItems = [
		{ testId: 'nav-item-home', href: '/', label: 'Home', icon: House },
		{
			testId: 'nav-item-tasks',
			href: '/tasks',
			label: 'Chore / Task Tracker',
			icon: CheckSquare,
			flag: FeatureFlags.CHORE_TASK_TRACKER_ENABLED
		},
		{
			testId: 'nav-item-activity',
			href: '/activity',
			label: 'Activity Picker',
			icon: Shuffle,
			flag: FeatureFlags.ACTIVITY_PICKER_ENABLED
		},
		{
			testId: 'nav-item-retro',
			href: '/retro',
			label: 'RetroAchievements',
			icon: Gamepad2,
			flag: FeatureFlags.RETRO_ACHIEVEMENTS_ENABLED
		},
		{
			testId: 'nav-item-weather',
			href: '/weather',
			label: 'Weather',
			icon: Cloud,
			flag: FeatureFlags.WEATHER_ENABLED
		},
		{
			testId: 'nav-item-audiobook',
			href: '/audiobook',
			label: 'Audiobook',
			icon: BookAudio,
			flag: FeatureFlags.AUDIOBOOK_ENABLED,
			activePath: '/audiobook'
		},
		{
			testId: 'nav-item-bookmarks',
			href: '/bookmarks',
			label: 'Bookmarks',
			icon: Bookmark,
			flag: FeatureFlags.BOOKMARKS_ENABLED
		},
		{
			testId: 'nav-item-walk-history',
			href: '/walk-history',
			label: 'Walk History',
			icon: Footprints,
			flag: FeatureFlags.WALK_SESSION_HISTORY_ENABLED
		},
		{
			testId: 'nav-item-comfyui',
			href: '/comfyui',
			label: 'ComfyUI',
			icon: Image,
			activePath: '/comfyui',
			flag: FeatureFlags.COMFYUI_ENABLED
		},
		{
			testId: 'nav-item-admin',
			href: '/admin/system-config',
			label: 'Admin',
			icon: Settings,
			activePath: '/admin'
		}
	];

	let enabledFlags = new Set(
		FeatureFlagService.featureFlags.filter((f) => f.isEnabled).map((f) => f.name)
	);

	$: visibleNavItems = navItems.filter((item) => !item.flag || enabledFlags.has(item.flag));

	let navEl: HTMLElement;

	function toggleCollapse() {
		if (!navEl) return;
		navEl.classList.toggle('collapsed');
		localStorage.setItem(STORAGE_KEY, String(navEl.classList.contains('collapsed')));
	}

	onMount(() => {
		if (localStorage.getItem(STORAGE_KEY) === 'true') {
			navEl.classList.add('collapsed');
		}
		const btn = navEl.querySelector<HTMLElement>('[data-testid="sidebar-toggle"]');
		btn?.addEventListener('click', toggleCollapse);
		navEl.setAttribute('data-mounted', 'true');

		const unsubscribe = MessageBus.subscribe(Messages.FeatureFlagsChanged, () => {
			enabledFlags = new Set(
				FeatureFlagService.featureFlags.filter((f) => f.isEnabled).map((f) => f.name)
			);
		});

		return () => {
			btn?.removeEventListener('click', toggleCollapse);
			unsubscribe();
		};
	});

	function isActive(href: string, path: string, activePath?: string): boolean {
		const prefix = activePath ?? href;
		if (prefix === '/') return path === '/';
		return path.startsWith(prefix);
	}
</script>

<nav class="sidebar" bind:this={navEl}>
	<a href="/" class="sidebar-logo" data-testid="sidebar-logo">
		<span class="logo-icon"><House size={LOGO_ICON_SIZE} /></span>
		<span class="logo-text">HomeHub</span>
	</a>

	<ul class="sidebar-nav">
		{#each visibleNavItems as item (item.href)}
			{@const Icon = item.icon}
			<li
				class="nav-item"
				class:active={isActive(item.href, currentPath, item.activePath)}
				data-testid={item.testId}
			>
				<a href={item.href} class="nav-link" title={item.label}>
					<Icon size={NAV_ICON_SIZE} />
					<span class="nav-label">{item.label}</span>
				</a>
			</li>
		{/each}
	</ul>

	<button class="collapse-toggle" data-testid="sidebar-toggle">
		<span class="icon-expand"><ChevronRight size={NAV_ICON_SIZE} /></span>
		<span class="icon-collapse"><ChevronLeft size={NAV_ICON_SIZE} /></span>
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
		overflow: hidden;
	}

	.sidebar:global(.collapsed) {
		width: 60px;
	}

	/* Logo */
	.sidebar-logo {
		display: flex;
		align-items: center;
		justify-content: center;
		gap: var(--space-2);
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

	.logo-icon {
		display: none;
		flex-shrink: 0;
	}

	.sidebar:global(.collapsed) .logo-icon {
		display: flex;
	}

	.logo-text {
		white-space: nowrap;
	}

	.sidebar:global(.collapsed) .logo-text {
		display: none;
	}

	/* Nav */
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

	.sidebar:global(.collapsed) .nav-label {
		display: none;
	}

	/* Toggle */
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

	.icon-expand {
		display: none;
	}

	.sidebar:global(.collapsed) .icon-expand {
		display: flex;
	}

	.icon-collapse {
		display: flex;
	}

	.sidebar:global(.collapsed) .icon-collapse {
		display: none;
	}
</style>
