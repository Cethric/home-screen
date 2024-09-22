/** @type {import('tailwindcss').Config} */

import config from '@homescreen/tailwind-config';

export default {
  content: [
    './src/**/*.{vue,js,ts,jsx,tsx}',
    './.storybook/**/*.{vue,js,ts,jsx,tsx}'
  ],
  presets: [
    config
  ]
};
