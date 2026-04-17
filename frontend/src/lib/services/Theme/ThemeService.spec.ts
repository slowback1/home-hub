import ThemeService, { ColorTheme } from '$lib/services/Theme/ThemeService';
import { afterEach, beforeEach } from 'vitest';
import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';

describe('ThemeService', () => {
	let service: ThemeService;
	const actualMatchMedia = window.matchMedia;

	function initializeService(clearMessages: boolean = true) {
		if (clearMessages) MessageBus.clearAll();

		service = new ThemeService();
	}

	function mockPrefersDarkMode() {
		window.matchMedia = (query) =>
			({
				matches: query.includes('dark')
			}) as never;
	}

	beforeEach(() => {
		initializeService();
	});

	afterEach(() => {
		window.matchMedia = actualMatchMedia;
	});

	it('initializes the current theme to light by default', () => {
		expect(MessageBus.getLastMessage<ColorTheme>(Messages.CurrentTheme)).toEqual(ColorTheme.Light);
	});

	it('initializes the current theme to dark if the browser settings indicate that the user prefers dark color scheme', () => {
		mockPrefersDarkMode();
		initializeService();

		expect(MessageBus.getLastMessage<ColorTheme>(Messages.CurrentTheme)).toEqual(ColorTheme.Dark);
	});

	it("does not overwrite the theme selection if it's already there", () => {
		MessageBus.sendMessage(Messages.CurrentTheme, ColorTheme.Dark);

		initializeService(false);

		expect(MessageBus.getLastMessage<ColorTheme>(Messages.CurrentTheme)).toEqual(ColorTheme.Dark);
	});

	it('can change the color theme', () => {
		service.changeTheme(ColorTheme.Dark);

		expect(MessageBus.getLastMessage<ColorTheme>(Messages.CurrentTheme)).toEqual(ColorTheme.Dark);
	});
});
