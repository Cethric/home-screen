import type { Preview } from '@storybook/vue3';
import { setup } from '@storybook/vue3';
import { ComponentApiPlugin } from '@homescreen/web-common-components-api';

import '@/styles/root.css';

setup((app) => {
  app.use(ComponentApiPlugin, {
    config: {
      async config() {
        return {
          otlpConfig: {},
          rumConfig: {},
        };
      },
    },
    client: {
      async get({
        path: { width, height },
      }: {
        path: { width: number; height: number };
      }) {
        console.log('get', width, height);
        await new Promise((resolve) => setTimeout(resolve, 20_000));
        return {
          response: new Response(new ReadableStream(), { status: 202 }),
          data: {
            url: `https://picsum.photos/${width}/${height}`,
          },
        };
      },
    },
  });
});

const preview: Preview = {
  parameters: {
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/i,
      },
    },
  },
};

export default preview;
