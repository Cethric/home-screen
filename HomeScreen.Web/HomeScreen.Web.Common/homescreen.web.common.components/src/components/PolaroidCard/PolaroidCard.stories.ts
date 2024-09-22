import type { Meta, StoryObj } from '@storybook/vue3';
import { Directions } from '../properties';
import PolaroidCard from './PolaroidCard.vue';
import { DateTime } from 'luxon';

const meta: Meta<typeof PolaroidCard> = {
  component: PolaroidCard,
  tags: ['autodocs'],
  args: {
    image: {
      id: 'https://picsum.photos/400/500',
      dateTime: DateTime.now(),
      enabled: true,
      location: { name: 'Sydney', longitude: 151.20732, latitude: -33.86785 },
      aspectWidth: 16.0 / 9.0,
      aspectHeight: 9.0 / 16.0,
      colour: { red: 0, green: 0, blue: 0 },
    },
  },
  argTypes: {
    direction: {
      options: Object.values(Directions),
      type: { name: 'enum', value: Object.values(Directions) },
    },
    image: {
      type: {
        name: 'object',
        value: {
          id: { name: 'string' },
          dateTime: { name: 'object', value: {} },
          enabled: { name: 'boolean' },
          location: {
            name: 'object',
            value: {
              name: { name: 'string' },
              latitude: { name: 'number' },
              longitude: { name: 'number' },
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
    template: '<Suspense><PolaroidCard v-bind="args" /></Suspense>',
    components: { PolaroidCard },
    setup() {
      return { args };
    },
  }),
};

export const Horizontal: PolaroidCardStory = {
  render: (args) => ({
    template: '<Suspense><PolaroidCard v-bind="args" /></Suspense>',
    components: { PolaroidCard },
    setup() {
      return { args };
    },
  }),
  args: { direction: Directions.horizontal },
};

export const Vertical: PolaroidCardStory = {
  render: (args) => ({
    template: '<Suspense><PolaroidCard v-bind="args" /></Suspense>',
    components: { PolaroidCard },
    setup() {
      return { args };
    },
  }),
  args: { direction: Directions.vertical },
};
