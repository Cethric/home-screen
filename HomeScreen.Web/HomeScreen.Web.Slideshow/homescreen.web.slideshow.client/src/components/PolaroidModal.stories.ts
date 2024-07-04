import type { Meta, StoryObj } from '@storybook/vue3';
import { loadPicsumImage, picsumImages } from '@/stories/helpers';
import PolaroidModal from '@/components/PolaroidModal.vue';

const meta: Meta<typeof PolaroidModal> = {
  component: PolaroidModal,
  tags: ['autodocs'],
  args: {
    item: {
      image: picsumImages()[0],
      top: 0,
      left: 0,
      rotation: 0,
    },
    loadImage: loadPicsumImage,
  },
  argTypes: {
    item: {
      type: {
        name: 'object',
        value: {
          image: {
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
          top: { name: 'number' },
          left: { name: 'number' },
          rotation: { name: 'number' },
        },
      },
    },
    loadImage: { type: 'function' },
  },
};
export default meta;

type PolaroidModalStory = StoryObj<typeof PolaroidModal>;

export const Default: PolaroidModalStory = {
  render: (args) => ({
    template: '<Suspense><PolaroidModal v-bind="args"  /></Suspense>',
    components: { PolaroidModal },
    setup() {
      return { args };
    },
  }),
};
