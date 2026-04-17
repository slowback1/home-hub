import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';

export enum ColorTheme {
	Light,
	Dark
}

export default class ThemeService {
	constructor() {
		this.initializeColorTheme();
	}

	changeTheme(theme: ColorTheme) {
		MessageBus.sendMessage(Messages.CurrentTheme, theme);
	}

	private initializeColorTheme() {
		const currentMessage = this.getCurrentMessage();

		if (currentMessage) return;

		const prefersDarkTheme = this.prefersDarkColorScheme();
		const theme = prefersDarkTheme ? ColorTheme.Dark : ColorTheme.Light;

		this.changeTheme(theme);
	}

	private getCurrentMessage(): ColorTheme | null {
		return MessageBus.getLastMessage<ColorTheme>(Messages.CurrentTheme);
	}

	private prefersDarkColorScheme(): boolean {
		if (!!window && window.matchMedia) {
			const result = window.matchMedia('prefers-color-scheme: dark');

			return result.matches;
		}
		return false;
	}
}
