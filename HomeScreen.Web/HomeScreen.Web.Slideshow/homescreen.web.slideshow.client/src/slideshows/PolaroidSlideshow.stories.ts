import type { Meta, StoryObj } from '@storybook/vue3';
import PolaroidSlideshow from '@/slideshows/PolaroidSlideshow.vue';
import { picsumImages } from '@/stories/helpers';

const meta: Meta<typeof PolaroidSlideshow> = {
  component: PolaroidSlideshow,
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
            id: { name: 'string' },
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
    intervalSeconds: { type: 'number', min: 1, max: 60, value: 8 },
    visibleCount: { type: 'number', min: 1, max: 150, value: 40 },
    weatherForecast: {
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
};
export default meta;

type PolaroidSlideshowStory = StoryObj<typeof PolaroidSlideshow>;

export const Default: PolaroidSlideshowStory = {
  render: (args) => ({
    template: '<PolaroidSlideshow v-bind="args" />',
    components: { PolaroidSlideshow },
    setup() {
      return { args };
    },
  }),
};
