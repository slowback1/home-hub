import { type Preview } from '@storybook/svelte';
import '../src/style/globals.css';
import '../src/style/reset.css';
import { withThemeByClassName } from '@storybook/addon-themes';

const preview: Preview = {
	parameters: {
		actions: { argTypesRegex: '^on[A-Z].*' },
		controls: {
			matchers: {
				color: /(background|color)$/i,
				date: /Date$/i
			}
		}
	},
	decorators: [
		withThemeByClassName({
			themes: {
				light: 'light-theme',
				dark: 'dark-theme'
			},
			defaultTheme: 'dark',
			parentSelector: 'html'
		})
	]
};

export default preview;
