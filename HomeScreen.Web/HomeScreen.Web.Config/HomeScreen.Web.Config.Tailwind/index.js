/** @type {import('tailwindcss').Config} */
import forms from '@tailwindcss/forms';
import typography from '@tailwindcss/typography';

console.log('[tailwind] Using Custom Tailwind Config');

export default {
  theme: {
    extend: {
      maxWidth: {
        100: '42rem',
        110: '45rem',
        modal: '95svw'
      },
      width: {
        100: '42rem',
        98: '28rem',
        '50dvw': '45dvw'
      },
      maxHeight: {
        100: '42rem'
      },
      height: {
        100: '42rem',
        '50dvh': '45dvh'
      }
    }
  },
  plugins: [
    forms,
    typography
  ]
};
