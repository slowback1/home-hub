import type { Component } from 'svelte';
import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
import { FeatureFlags } from '$lib/services/FeatureFlag/FeatureFlags';
import TasksWidget from './widgets/TasksWidget.svelte';
import ActivityWidget from './widgets/ActivityWidget.svelte';
import RetroWidget from './widgets/RetroWidget.svelte';
import WeatherWidget from './widgets/WeatherWidget.svelte';
import AudiobookWidget from './widgets/AudiobookWidget.svelte';
import BookmarksWidget from './widgets/BookmarksWidget.svelte';
import ComfyUiWidget from './widgets/ComfyUiWidget.svelte';

export type WidgetEntry = {
	id: string;
	name: string;
	description: string;
	icon: string;
	href: string;
	featureFlag?: string;
	component: Component;
};

export const WIDGET_REGISTRY: WidgetEntry[] = [
	{
		id: 'tasks',
		name: 'Chore Tasks',
		description: 'Track and complete your recurring chores.',
		icon: 'check-square',
		href: '/tasks',
		featureFlag: FeatureFlags.CHORE_TASK_TRACKER_ENABLED,
		component: TasksWidget as unknown as Component
	},
	{
		id: 'activity',
		name: 'Activity Picker',
		description: 'Get a random activity suggestion.',
		icon: 'shuffle',
		href: '/activity',
		featureFlag: FeatureFlags.ACTIVITY_PICKER_ENABLED,
		component: ActivityWidget as unknown as Component
	},
	{
		id: 'retro',
		name: 'RetroAchievements',
		description: 'Track your retro gaming achievements.',
		icon: 'gamepad-2',
		href: '/retro',
		featureFlag: FeatureFlags.RETRO_ACHIEVEMENTS_ENABLED,
		component: RetroWidget as unknown as Component
	},
	{
		id: 'weather',
		name: 'Weather',
		description: 'Current conditions and forecast.',
		icon: 'cloud',
		href: '/weather',
		featureFlag: FeatureFlags.WEATHER_ENABLED,
		component: WeatherWidget as unknown as Component
	},
	{
		id: 'audiobook',
		name: 'Audiobook',
		description: 'Monitor and manage audiobook conversions.',
		icon: 'book-audio',
		href: '/audiobook',
		featureFlag: FeatureFlags.AUDIOBOOK_ENABLED,
		component: AudiobookWidget as unknown as Component
	},
	{
		id: 'bookmarks',
		name: 'Bookmarks',
		description: 'Quick access to your saved links.',
		icon: 'bookmark',
		href: '/bookmarks',
		featureFlag: FeatureFlags.BOOKMARKS_ENABLED,
		component: BookmarksWidget as unknown as Component
	},
	{
		id: 'comfyui',
		name: 'ComfyUI',
		description: 'Generate images with your ComfyUI workflows.',
		icon: 'image',
		href: '/comfyui',
		featureFlag: FeatureFlags.COMFYUI_ENABLED,
		component: ComfyUiWidget as unknown as Component
	}
];

export function getVisibleWidgets(): WidgetEntry[] {
	return WIDGET_REGISTRY.filter(
		(w) =>
			!w.featureFlag ||
			FeatureFlagService.featureFlags.some((f) => f.name === w.featureFlag && f.isEnabled)
	);
}
