import js from "@eslint/js";
import globals from "globals";
import tseslint from "typescript-eslint";
import pluginReact from "eslint-plugin-react";
import pluginPrettier from "eslint-plugin-prettier";
import { defineConfig } from "eslint/config";

export default defineConfig([
  {
    files: ["src/app/**/*.{js,mjs,cjs,ts,mts,cts,jsx,tsx}"],
    ignores: ["**/.next/**", "**/node_modules/**"],
    languageOptions: {
      parser: tseslint.parser,
      parserOptions: {
        ecmaVersion: "latest",
        sourceType: "module",
        ecmaFeatures: { jsx: true },
      },
      globals: {
        ...globals.browser,
        ...globals.node,
        React: true,
        process: true,
      },
    },
    settings: { react: { version: "detect" } },
    plugins: {
      js,
      "@typescript-eslint": tseslint.plugin,
      react: pluginReact,
      prettier: pluginPrettier,
    },
    rules: {
      // ESLint + TS base
      ...js.configs.recommended.rules,
      ...tseslint.configs.recommended.rules,

      "no-unused-vars": "off",
      "@typescript-eslint/no-unused-vars": ["error"],
      ...pluginReact.configs.recommended.rules,
      ...pluginPrettier.configs.recommended.rules,

      // Style
      semi: ["error", "always"],
      quotes: ["error", "double"],
      "comma-dangle": ["error", "always-multiline"],

      // React 17+ JSX no longer needs React in scope
      "react/react-in-jsx-scope": "off",
    },
  },
]);
