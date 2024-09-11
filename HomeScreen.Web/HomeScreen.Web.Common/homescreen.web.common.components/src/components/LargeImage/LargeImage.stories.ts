import type { Meta, StoryObj } from '@storybook/vue3';
import LargeImage from '@/components/LargeImage/LargeImage.vue';
import { picsumImages } from '@/stories/helpers';

const meta: Meta<typeof LargeImage> = {
  component: LargeImage,
  tags: ['autodocs'],
  args: {
    image: picsumImages()[0],
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
