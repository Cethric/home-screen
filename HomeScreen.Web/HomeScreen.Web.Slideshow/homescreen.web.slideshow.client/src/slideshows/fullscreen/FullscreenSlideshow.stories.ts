import type { Image } from '@homescreen/web-common-components';
import type { Meta, StoryObj } from '@storybook/vue3';
import FullscreenSlideshow from '@/slideshows/fullscreen/FullscreenSlideshow.vue';
import { picsumImages } from '@/stories/helpers';

const meta: Meta<typeof FullscreenSlideshow> = {
  component: FullscreenSlideshow,
  tags: ['autodocs'],
  args: {
    images: picsumImages().reduce<Record<Image['id'], Image>>((p, c) => {
      p[c.id] = c;
      return p;
    }, {}),
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
    intervalSeconds: { type: 'number', min: 1, max: 60, value: 24 },
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

type FullscreenSlideshowStory = StoryObj<typeof FullscreenSlideshow>;

export const Default: FullscreenSlideshowStory = {
  render: (args) => ({
    template: '<FullscreenSlideshow v-bind="args" />',
    components: { FullscreenSlideshow },
    setup() {
      return { args };
    },
  }),
};
