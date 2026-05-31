import { getVisibleWidgets, WIDGET_REGISTRY } from './widgetRegistry';
import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
import { createTestFeatureFlag } from '$lib/testHelpers/testFeatureFlagProvider';

describe('WIDGET_REGISTRY', () => {
	it('contains entries for all 6 active widget types', () => {
		const ids = WIDGET_REGISTRY.map((w) => w.id);
		expect(ids).toContain('tasks');
		expect(ids).toContain('activity');
		expect(ids).toContain('weather');
		expect(ids).toContain('audiobook');
		expect(ids).toContain('bookmarks');
		expect(ids).toContain('walk-history');
	});

	it('does not contain comfyui or retro (no query API exists for these)', () => {
		const ids = WIDGET_REGISTRY.map((w) => w.id);
		expect(ids).not.toContain('comfyui');
		expect(ids).not.toContain('retro');
	});

	it('every entry has id, name, description, icon, href, and component', () => {
		for (const entry of WIDGET_REGISTRY) {
			expect(entry.id).toBeTruthy();
			expect(entry.name).toBeTruthy();
			expect(entry.description).toBeTruthy();
			expect(entry.icon).toBeTruthy();
			expect(entry.href).toBeTruthy();
			expect(entry.component).toBeTruthy();
		}
	});
});

describe('getVisibleWidgets', () => {
	it('includes widgets with no feature flag regardless of flag state', () => {
		FeatureFlagService.featureFlags = [];
		const visible = getVisibleWidgets();
		const noFlagEntries = WIDGET_REGISTRY.filter((w) => !w.featureFlag);
		for (const entry of noFlagEntries) {
			expect(visible.map((v) => v.id)).toContain(entry.id);
		}
	});

	it('excludes widgets whose feature flag is disabled', () => {
		FeatureFlagService.featureFlags = [createTestFeatureFlag('AUDIOBOOK_ENABLED', false)];
		const visible = getVisibleWidgets();
		expect(visible.map((v) => v.id)).not.toContain('audiobook');
	});

	it('includes widgets whose feature flag is enabled', () => {
		FeatureFlagService.featureFlags = [createTestFeatureFlag('AUDIOBOOK_ENABLED', true)];
		const visible = getVisibleWidgets();
		expect(visible.map((v) => v.id)).toContain('audiobook');
	});

	it('includes bookmarks when BOOKMARKS_ENABLED is on', () => {
		FeatureFlagService.featureFlags = [createTestFeatureFlag('BOOKMARKS_ENABLED', true)];
		const visible = getVisibleWidgets();
		expect(visible.map((v) => v.id)).toContain('bookmarks');
	});

	it('excludes bookmarks when BOOKMARKS_ENABLED is off', () => {
		FeatureFlagService.featureFlags = [createTestFeatureFlag('BOOKMARKS_ENABLED', false)];
		const visible = getVisibleWidgets();
		expect(visible.map((v) => v.id)).not.toContain('bookmarks');
	});

	it('includes walk-history when WALK_SESSION_HISTORY_ENABLED is on', () => {
		FeatureFlagService.featureFlags = [createTestFeatureFlag('WALK_SESSION_HISTORY_ENABLED', true)];
		const visible = getVisibleWidgets();
		expect(visible.map((v) => v.id)).toContain('walk-history');
	});

	it('excludes walk-history when WALK_SESSION_HISTORY_ENABLED is off', () => {
		FeatureFlagService.featureFlags = [
			createTestFeatureFlag('WALK_SESSION_HISTORY_ENABLED', false)
		];
		const visible = getVisibleWidgets();
		expect(visible.map((v) => v.id)).not.toContain('walk-history');
	});
});
