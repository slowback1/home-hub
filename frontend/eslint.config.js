// eslint.config.js
import js from '@eslint/js';
import svelte from 'eslint-plugin-svelte';
import globals from 'globals';
import ts from 'typescript-eslint';
import svelteConfig from './svelte.config.js';
import { globalIgnores } from 'eslint/config';

const maximumComplexity = 5;

export default ts.config(
	globalIgnores(['build', 'dist', 'node_modules', '.svelte-kit']),
	js.configs.recommended,
	...ts.configs.recommended,
	...svelte.configs.recommended,
	{
		languageOptions: {
			globals: {
				...globals.browser,
				...globals.node
			}
		}
	},
	{
		files: ['**/*.svelte', '**/*.svelte.ts', '**/*.svelte.js'],
		// See more details at: https://typescript-eslint.io/packages/parser/
		languageOptions: {
			parserOptions: {
				projectService: true,
				extraFileExtensions: ['.svelte'], // Add support for additional file extensions, such as .svelte
				parser: ts.parser,
				// Specify a parser for each language, if needed:
				// parser: {
				//   ts: ts.parser,
				//   js: espree,    // Use espree for .js files (add: import espree from 'espree')
				//   typescript: ts.parser
				// },

				// We recommend importing and specifying svelte.config.js.
				// By doing so, some rules in eslint-plugin-svelte will automatically read the configuration and adjust their behavior accordingly.
				// While certain Svelte settings may be statically loaded from svelte.config.js even if you don’t specify it,
				// explicitly specifying it ensures better compatibility and functionality.
				svelteConfig
			}
		}
	},
	{
		rules: {
			// Generic JS/TS rules
			'default-case': 'error',
			'default-case-last': 'error',
			'no-await-in-loop': 'error',
			'no-magic-numbers': [
				'error',
				{
					ignoreDefaultValues: true,
					ignoreArrayIndexes: true,
					ignoreClassFieldInitialValues: true,
					ignoreEnums: true,
					ignore: [0, 1]
				}
			],
			'no-multi-assign': 'error',
			'no-nested-ternary': 'error',
			complexity: ['error', { max: maximumComplexity, variant: 'modified' }],

			//Svelte-specific rules
			'svelte/no-dom-manipulating': 'error'
		}
	},
	{
		files: ['**/*.spec.ts', '**/*.stories.ts'],
		rules: {
			'no-magic-numbers': 'off'
		}
	}
);
