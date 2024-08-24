import type { Meta, StoryObj } from '@storybook/vue3';
import { loadPicsumImage, picsumImages } from '@/stories/helpers';
import FullscreenModal from '@/slideshows/fullscreen/FullscreenModal.vue';

const meta: Meta<typeof FullscreenModal> = {
  component: FullscreenModal,
  tags: ['autodocs'],
  args: {
    image: picsumImages()[0],
    loadImage: loadPicsumImage,
  },
  argTypes: {
    image: {
      type: {
        name: 'object',
        value: {
          id: { name: 'string' },
          dateTime: { name: 'object', value: {} },
          location: {
            name: 'object',
            value: {
              name: { name: 'string' },
              latitude: { name: 'string' },
              longitude: { name: 'string' },
            },
          },
        },
      },
    },
    loadImage: { type: 'function' },
  },
};
export default meta;

type FullscreenModalStory = StoryObj<typeof FullscreenModal>;

export const Default: FullscreenModalStory = {
  render: (args) => ({
    template: '<Suspense><FullscreenModal v-bind="args"  /></Suspense>',
    components: { FullscreenModal },
    setup() {
      return { args };
    },
  }),
};
