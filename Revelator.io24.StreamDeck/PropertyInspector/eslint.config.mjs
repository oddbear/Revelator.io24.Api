// eslint.config.js
import babelEslint from '@babel/eslint-parser';
import globals from "globals";

export default {
	languageOptions: {
		parser: babelEslint,
		globals: {
			...globals.browser,
		},
		parserOptions: {
			requireConfigFile: false,
		}
	},
	files: ["**/*.js"],
	ignores: ["sdpi-components.js"],
	rules: {
		semi: "error",
		"prefer-const": "error",
		"no-unused-vars": ["warn", { args: "none" }], // Ignore unused function arguments
		"no-undef": "error",
		"quotes": "warn",
		"no-multiple-empty-lines": "warn",
		"no-console": "warn",
		"no-duplicate-imports": "warn"
	}
};
