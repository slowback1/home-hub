import type { Component } from 'svelte';
import { CheckSquare, Shuffle, Cloud, BookAudio, Bookmark, Footprints } from 'lucide-svelte';
import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
import { FeatureFlags } from '$lib/services/FeatureFlag/FeatureFlags';
import TasksWidget from './widgets/TasksWidget.svelte';
import ActivityWidget from './widgets/ActivityWidget.svelte';
import WeatherWidget from './widgets/WeatherWidget.svelte';
import AudiobookWidget from './widgets/AudiobookWidget.svelte';
import BookmarksWidget from './widgets/BookmarksWidget.svelte';
import WalkHistoryWidget from './widgets/WalkHistoryWidget.svelte';

export type WidgetEntry = {
	id: string;
	name: string;
	description: string;
	icon: Component;
	href: string;
	featureFlag?: string;
	component: Component;
};

export const WIDGET_REGISTRY: WidgetEntry[] = [
	{
		id: 'tasks',
		name: 'Chore Tasks',
		description: 'Track and complete your recurring chores.',
		icon: CheckSquare as unknown as Component,
		href: '/tasks',
		featureFlag: FeatureFlags.CHORE_TASK_TRACKER_ENABLED,
		component: TasksWidget as unknown as Component
	},
	{
		id: 'activity',
		name: 'Activity Picker',
		description: 'Get a random activity suggestion.',
		icon: Shuffle as unknown as Component,
		href: '/activity',
		featureFlag: FeatureFlags.ACTIVITY_PICKER_ENABLED,
		component: ActivityWidget as unknown as Component
	},
	{
		id: 'weather',
		name: 'Weather',
		description: 'Current conditions and forecast.',
		icon: Cloud as unknown as Component,
		href: '/weather',
		featureFlag: FeatureFlags.WEATHER_ENABLED,
		component: WeatherWidget as unknown as Component
	},
	{
		id: 'audiobook',
		name: 'Audiobook',
		description: 'Monitor and manage audiobook conversions.',
		icon: BookAudio as unknown as Component,
		href: '/audiobook',
		featureFlag: FeatureFlags.AUDIOBOOK_ENABLED,
		component: AudiobookWidget as unknown as Component
	},
	{
		id: 'bookmarks',
		name: 'Bookmarks',
		description: 'Quick access to your saved links.',
		icon: Bookmark as unknown as Component,
		href: '/bookmarks',
		featureFlag: FeatureFlags.BOOKMARKS_ENABLED,
		component: BookmarksWidget as unknown as Component
	},
	{
		id: 'walk-history',
		name: 'Walk History',
		description: 'Recent walk sessions synced from your phone.',
		icon: Footprints as unknown as Component,
		href: '/walk-history',
		featureFlag: FeatureFlags.WALK_SESSION_HISTORY_ENABLED,
		component: WalkHistoryWidget as unknown as Component
	}
];

export function getVisibleWidgets(): WidgetEntry[] {
	return WIDGET_REGISTRY.filter(
		(w) =>
			!w.featureFlag ||
			FeatureFlagService.featureFlags.some((f) => f.name === w.featureFlag && f.isEnabled)
	);
}
