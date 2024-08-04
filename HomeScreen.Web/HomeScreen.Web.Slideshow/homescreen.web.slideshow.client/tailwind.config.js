/** @type {import('tailwindcss').Config} */
export default {
  content: [
    './index.html',
    './src/**/*.{vue,js,ts,jsx,tsx}',
    './node_modules/@homescreen/web-components-client/src/**/*.{vue,js,ts,jsx,tsx}',
  ],
  theme: {
    extend: {
      maxWidth: {
        100: '42rem',
        110: '45rem',
        modal: '90dvw',
      },
      width: {
        100: '42rem',
        98: '28rem',
        '50dvw': '45dvw',
      },
      maxHeight: {
        100: '42rem',
      },
      height: {
        100: '42rem',
        '50dvh': '45dvh',
      },
    },
  },
  plugins: [],
};
