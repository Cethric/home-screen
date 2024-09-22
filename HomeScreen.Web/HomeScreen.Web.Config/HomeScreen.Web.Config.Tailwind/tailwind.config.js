/** @type {import('tailwindcss').Config} */

import config from '@homescreen/tailwind-config';

export default {
  content: [
    './index.html',
    './src/**/*.{vue,js,ts,jsx,tsx}'
  ],
  presets: [
    config
  ]
};
