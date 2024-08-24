/** @type {import('tailwindcss').Config} */
export default {
  content: ['./src/**/*.{vue,js,ts,jsx,tsx}'],
  theme: {
    extend: {
      maxWidth: {
        100: '42rem',
        modal: '90dvw'
      },
      width: {
        100: '42rem'
      },
      maxHeight: {
        100: '42rem'
      },
      height: {
        100: '42rem'
      }
    }
  },
  plugins: []
}
