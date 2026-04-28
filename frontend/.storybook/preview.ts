import { type Preview } from '@storybook/svelte';
import '../src/style/globals.css';
import '../src/style/reset.css';

const preview: Preview = {
	parameters: {
		actions: { argTypesRegex: '^on[A-Z].*' },
		controls: {
			matchers: {
				color: /(background|color)$/i,
				date: /Date$/i
			}
		},
		options: {
			storySort: {
				order: [
					'Design Tokens',
					'Buttons',
					'Inputs',
					'Containers',
					'Feedback',
					'Navigation',
					'Data',
					'Typography'
				]
			}
		}
	}
};

export default preview;
