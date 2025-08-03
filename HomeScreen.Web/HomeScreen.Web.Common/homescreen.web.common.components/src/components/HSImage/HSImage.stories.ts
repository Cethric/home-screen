import type { Meta, StoryObj } from '@storybook/vue3';
import HSImage from '@/components/HSImage/HSImage.vue';
import { picsumImage } from '@/stories/helpers.ts';

const meta: Meta<typeof HSImage> = {
  component: HSImage,
  title: 'Components/HSImage',
  tags: ['autodocs'],
  args: {
    ...picsumImage(800, 600, false, 7),
    size: 128,
    rounded: true,
  },
  argTypes: {
    id: { type: 'string', control: 'text' },
    // dateTime: { type: 'object', control: 'date' },
    enabled: { type: 'boolean' },
    location: {
      // type: 'object',
      value: {
        name: { name: 'string' },
        latitude: { name: 'number' },
        longitude: { name: 'number' },
      },
    },
    aspectRatio: { type: 'number' },
    portrait: { type: 'boolean' },
    colour: {
      // type: 'object',
      value: {
        red: { name: 'number' },
        green: { name: 'number' },
        blue: { name: 'number' },
      },
    },
    size: { type: 'number' },
    rounded: { type: 'boolean' },
  },
};
export default meta;

type HSImageStory = StoryObj<typeof HSImage>;

export const Default: HSImageStory = {};

export const Portrait: HSImageStory = {
  args: {
    ...picsumImage(600, 800, true, 8),
  },
};
