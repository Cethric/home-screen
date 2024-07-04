import type { Meta, StoryObj } from '@storybook/vue3';
import { loadPicsumImage, picsumImages } from '@/stories/helpers';
import LargeImage from '@/components/LargeImage.vue';

const meta: Meta<typeof LargeImage> = {
  component: LargeImage,
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

type LargeImageStory = StoryObj<typeof LargeImage>;

export const Default: LargeImageStory = {
  render: (args) => ({
    template: '<LargeImage v-bind="args"  />',
    components: { LargeImage },
    setup() {
      return { args };
    },
  }),
};
