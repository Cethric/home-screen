/* eslint-env node */
require('@rushstack/eslint-patch/modern-module-resolution');

module.exports = {
    extends: [
        'plugin:vue/vue3-essential',
        'eslint:recommended',
        '@vue/eslint-config-typescript',
        '@vue/eslint-config-prettier/skip-formatting',
        'prettier',
        'plugin:storybook/recommended',
        'plugin:tailwindcss/recommended',
    ],
    parserOptions: {
        ecmaVersion: 'latest',
    },
};
