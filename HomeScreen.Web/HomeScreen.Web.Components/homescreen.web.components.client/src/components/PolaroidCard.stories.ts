import type { Meta, StoryObj } from '@storybook/vue3';
import { Directions } from './properties';
import PolaroidCard from './PolaroidCard.vue';
import { DateTime } from 'luxon';

const meta: Meta<typeof PolaroidCard> = {
  component: PolaroidCard,
  tags: ['autodocs'],
  args: {
    image: {
      images: ['https://picsum.photos/400/500'],
      thumbnail: 'https://picsum.photos/400/500/?blur=2',
      width: 400,
      height: 500,
      dateTime: DateTime.now(),
      location: { name: 'Sydney', longitude: 151.20732, latitude: -33.86785 },
    },
  },
  argTypes: {
    direction: {
      options: Object.values(Directions),
      type: { name: 'enum', value: Object.values(Directions) },
    },
    location: {
      type: {
        name: 'object',
        value: {
          top: { name: 'number' },
          left: { name: 'number' },
          rotation: { name: 'number' },
        },
      },
    },
    image: {
      type: {
        name: 'object',
        value: {
          src: {
            name: 'array',
            value: { name: 'string' },
          },
          loading: { name: 'string' },
          width: { name: 'number' },
          height: { name: 'number' },
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
    flat: {
      type: 'boolean',
    },
  },
};
export default meta;

type PolaroidCardStory = StoryObj<typeof PolaroidCard>;

export const Default: PolaroidCardStory = {
  render: (args) => ({
    template: '<PolaroidCard v-bind="args" />',
    components: { PolaroidCard },
    setup() {
      return { args };
    },
  }),
};

export const Horizontal: PolaroidCardStory = {
  render: (args) => ({
    template: '<PolaroidCard v-bind="args" />',
    components: { PolaroidCard },
    setup() {
      return { args };
    },
  }),
  args: { direction: Directions.horizontal },
};

export const Vertical: PolaroidCardStory = {
  render: (args) => ({
    template: '<PolaroidCard v-bind="args" />',
    components: { PolaroidCard },
    setup() {
      return { args };
    },
  }),
  args: { direction: Directions.vertical },
};

export const Absolute: PolaroidCardStory = {
  render: (args) => ({
    template: '<div class="w-dvw h-dvh"><PolaroidCard v-bind="args" /></div>',
    components: { PolaroidCard },
    setup() {
      return { args };
    },
  }),
  args: { location: { top: 10, left: 10, rotation: 15 } },
};
