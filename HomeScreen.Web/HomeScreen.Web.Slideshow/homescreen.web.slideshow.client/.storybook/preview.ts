import type { Preview } from '@storybook/vue3';

import '@/styles/_root.scss';

const preview: Preview = {
  tags: ['autodocs'],
  parameters: {
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/i,
      },
    },
    layout: 'fullscreen',
    viewport: {
      viewports: {
        dashboard: {
          name: 'dashboard',
          styles: {
            width: '1080px',
            height: '1920px',
          },
        },
      },
    },
  },
};

export default preview;
