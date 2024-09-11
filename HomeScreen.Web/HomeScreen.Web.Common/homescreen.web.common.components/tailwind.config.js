/** @type {import('tailwindcss').Config} */
export default {
    content: ['./src/**/*.{vue,js,ts,jsx,tsx}'],
    theme: {
        extend: {
            maxWidth: {
                100: '42rem',
                110: '45rem',
                modal: '95svw',
            },
            width: {
                100: '42rem',
                98: '28rem',
                '50dvw': '45dvw',
            },
            minWidth: {
                modal: '95svw',
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
