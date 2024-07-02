import type { Meta, StoryObj } from '@storybook/vue3';
import RollingSlideshow from '@/slideshows/RollingSlideshow.vue';
import { Directions } from '@components/properties';
import { picsumImages } from '@/stories/helpers';

const meta: Meta<typeof RollingSlideshow> = {
  component: RollingSlideshow,
  tags: ['autodocs'],
  args: {
    images: picsumImages(),
    weatherForecast: { feelsLikeTemperature: 0, weatherCode: 'Sunny' },
  },
  argTypes: {
    images: {
      type: {
        name: 'array',
        value: {
          name: 'object',
          value: {
            id: { type: 'string' },
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
    },
    weatherForecast: {
      type: {
        name: 'object',
        value: {
          feelsLikeTemperature: { type: 'number' },
          maxTemperature: { type: 'number' },
          minTemperature: { type: 'number' },
          chanceOfRain: { type: 'number' },
          amountOfRain: { type: 'number' },
          weatherCode: { type: 'string' },
        },
      },
    },
    direction: { type: { name: 'enum', value: Object.values(Directions) } },
    durationSeconds: { type: 'number', min: 1, max: 60, value: 8 },
    count: { type: 'number', min: 1, max: 8, value: 2 },
  },
};
export default meta;

type RollingSlideshowStory = StoryObj<typeof RollingSlideshow>;

export const Default: RollingSlideshowStory = {
  render: (args) => ({
    template: '<RollingSlideshow v-bind="args" />',
    components: { RollingSlideshow },
    setup() {
      return { args };
    },
  }),
};

export const Horizontal: RollingSlideshowStory = {
  render: (args) => ({
    template: '<RollingSlideshow v-bind="args" />',
    components: { RollingSlideshow },
    setup() {
      return { args };
    },
  }),
  args: {
    direction: Directions.horizontal,
  },
};

export const Vertical: RollingSlideshowStory = {
  render: (args) => ({
    template: '<RollingSlideshow v-bind="args" />',
    components: { RollingSlideshow },
    setup() {
      return { args };
    },
  }),
  args: {
    direction: Directions.vertical,
  },
};
